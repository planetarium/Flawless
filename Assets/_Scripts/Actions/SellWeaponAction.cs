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
        // Used for reflection when deserializing a stored action.
        public SellWeaponAction()
        {
        }

        // Used for serialzing an action.
        public override Bencodex.Types.IValue PlainValue => Bencodex.Types.Null.Value;

        // Used for deserializing a stored action.
        public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
        {
            if (!(plainValue is Bencodex.Types.Null))
            {
                throw new ArgumentException(
                    $"Invalid {nameof(plainValue)} type: {plainValue.GetType()}");
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
                    : throw new ArgumentException($"Invalid player state at {context.Signer}");
            Address weaponAddress = playerState.WeaponAddress;

            WeaponState weaponState =
                states.GetState(playerState.WeaponAddress) is Bencodex.Types.Dictionary weaponStateEncoded
                    ? new WeaponState(weaponStateEncoded)
                    : throw new ArgumentException($"Can't find weapon state at {weaponAddress}");
            playerState = playerState
                .RemoveWeapon(weaponState)
                .AddGold(weaponState.Price);

            // Adjust health.
            WeaponState newWeaponState =
                states.GetState(playerState.WeaponAddress) is Bencodex.Types.Dictionary newWeaponStateEncoded
                    ? new WeaponState(newWeaponStateEncoded)
                    : throw new ArgumentException($"Invalid weapon state at {weaponAddress}");
            long maxHealth = playerState.GetMaxHealth(newWeaponState);
            long diff = playerState.StatsState.Damages - maxHealth;
            if (diff > 0)
            {
                playerState = playerState.Heal(diff + 1);
            }

            return states
                .SetState(playerState.WeaponAddress, newWeaponState.Encode())
                .SetState(context.Signer, playerState.Encode());
        }
    }
}
