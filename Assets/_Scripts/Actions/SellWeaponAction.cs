using System;
using Flawless.States;
using Libplanet.Action;
using Libplanet.Unity;
using UnityEngine;

namespace Flawless.Actions
{
    // Used for reflection when deserializing a stored action.
    [ActionType("sell_weapon_action")]
    public class SellWeaponAction : ActionBase
    {
        private Bencodex.Types.IValue _plainValue;

        // Used for reflection when deserializing a stored action.
        public SellWeaponAction()
        {
            _plainValue = Bencodex.Types.Null.Value;
        }

        // Used for serialzing an action.
        public override Bencodex.Types.IValue PlainValue => Bencodex.Types.Null.Value;

        // Used for deserializing a stored action.
        public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
        {
            if (plainValue is Bencodex.Types.Null bnull)
            {
                _plainValue = bnull;
            }
            else
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
                    : throw new ArgumentException($"Invalid player state at {context.Signer}.");

            // Mutates the loaded state, logs the result, and stores the resulting state.
            Debug.Log($"Trying to sell player {playerState.Name} {playerState.Address}'s weapon...");
            Debug.Log(
                $"Player gold: {playerState.Gold}, " +
                $"weapon: {playerState.WeaponState.Encode()}, " +
                $"weapon price: {playerState.WeaponState.GetPrice()}");
            playerState = playerState.SellWeapon();
            Debug.Log(
                $"Player gold: {playerState.Gold}, " +
                $"weapon: {playerState.WeaponState.Encode()}");

            return states
                .SetState(context.Signer, playerState.Encode());
        }
    }
}
