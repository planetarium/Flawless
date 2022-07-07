using System;
using Flawless.States;
using Libplanet;
using Libplanet.Action;
using Libplanet.Unity;
using UnityEngine;

namespace Flawless.Actions
{
    // Used for reflection when deserializing a stored action.
    [ActionType("sell_weapon_action")]
    public class SellWeaponAction : ActionBase
    {
        private Address _weaponAddress;

        // Used for reflection when deserializing a stored action.
        public SellWeaponAction()
        {
        }

        public SellWeaponAction(Address weaponAddress)
        {
            _weaponAddress = weaponAddress;
        }

        // Used for serialzing an action.
        public override Bencodex.Types.IValue PlainValue =>
            (Bencodex.Types.Binary) _weaponAddress.ToByteArray();

        // Used for deserializing a stored action.
        public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
        {
            if (plainValue is Bencodex.Types.Binary asBinary)
            {
                _weaponAddress = new Address(asBinary);
            }
            else
            {
                throw new ArgumentException(
                    $"Invalid {nameof(plainValue)} type: {plainValue.GetType()}"
                );
            }
        }

        // Executes an action.
        // This is what gets called when a block containing an action is mined
        // or appended to a blockchain.
        public override IAccountStateDelta Execute(IActionContext context)
        {
            // Retrieves the previously stored state.
            IAccountStateDelta states = context.PreviousStates;
            PlayerState playerState =
                states.GetState(context.Signer) is Bencodex.Types.Dictionary playerStateEncoded
                    ? new PlayerState(playerStateEncoded)
                    : throw new ArgumentException($"Invalid player state at {context.Signer}.");
            WeaponState weaponState =
                states.GetState(_weaponAddress) is Bencodex.Types.Dictionary weaponStateEncoded
                    ? new WeaponState(weaponStateEncoded)
                    : throw new ArgumentException($"Can't find weapon state at {_weaponAddress}");
            playerState = playerState
                    .RemoveWeapon(weaponState)
                    .AddGold(weaponState.Price);

            // Adjust health.
            Address weaponAddress = playerState.EquippedWeaponAddress;
            WeaponState equippedWeaponState =
                weaponAddress == default
                    ? new WeaponState()
                    : states.GetState(playerState.EquippedWeaponAddress) is Bencodex.Types.Dictionary equippedWeaponStateEncoded
                        ? new WeaponState(equippedWeaponStateEncoded)
                        : throw new ArgumentException($"Invalid weapon state at {weaponAddress}");
            long maxHealth = playerState.GetMaxHealth(equippedWeaponState);
            long diff = playerState.StatsState.Damages - maxHealth;
            if (diff > 0)
            {
                playerState = playerState.Heal(diff + 1);
            }
            
            return states.SetState(context.Signer, playerState.Encode());
        }
    }
}
