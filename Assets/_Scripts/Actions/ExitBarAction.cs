using System;
using Flawless.Models.Encounters;
using Flawless.States;
using Libplanet.Action;
using Libplanet.Unity;

namespace Flawless.Actions
{
    // Used for reflection when deserializing a stored action.
    [ActionType("exit_bar_action")]
    public class ExitBarAction : ActionBase
    {
        // Used for reflection when deserializing a stored action.
        public ExitBarAction()
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
                    : throw new ArgumentException($"Invalid player state at {context.Signer}.");

            if (!playerState.SceneState.InEncounter || !(playerState.SceneState.GetNextEncounter() is BarEncounter))
            {
                throw new ArgumentException($"Invalid scene state at {context.Signer}.");
            }

            playerState = playerState.Proceed(context.Random.Seed);
            return states.SetState(context.Signer, playerState.Encode());
        }
    }
}
