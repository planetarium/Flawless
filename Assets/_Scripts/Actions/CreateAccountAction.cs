using System;
using Flawless.Models.Encounters;
using Flawless.States;
using Libplanet;
using Libplanet.Action;
using Libplanet.Unity;

namespace Flawless.Actions
{
    // Used for reflection when deserializing a stored action.
    [ActionType("create_account_action")]
    public class CreateAccountAction : ActionBase
    {
        private CreateAccountActionPlainValue _plainValue;

        // Used for reflection when deserializing a stored action.
        public CreateAccountAction()
        {
        }

        // Used for creating a new action.
        public CreateAccountAction(Address account, string name)
        {
            _plainValue = new CreateAccountActionPlainValue(account, name);
        }


        // Used for serialzing an action.
        public override Bencodex.Types.IValue PlainValue => _plainValue.Encode();

        // Used for deserializing a stored action.
        public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
        {
            _plainValue = plainValue is Bencodex.Types.Dictionary bdict
                ? new CreateAccountActionPlainValue(bdict)
                : throw new ArgumentException(
                    $"Invalid {nameof(plainValue)} type: {plainValue.GetType()}");
        }

        // Executes an action.
        // This is what gets called when a block containing an action is mined
        // or appended to a blockchain.
        public override IAccountStateDelta Execute(IActionContext context)
        {
            // Retrieves the previously stored state.
            IAccountStateDelta states = context.PreviousStates;
            PlayerState playerState =
                states.GetState(context.Signer) is null
                    ? new PlayerState(_plainValue.Address, _plainValue.Name, context.Random.Seed)
                    : throw new ArgumentException($"Invalid player state at {context.Signer}.");
            WeaponState weaponState = new WeaponState();
            long maxHealth = playerState.GetMaxHealth(weaponState);
            playerState = playerState.EditHealth(maxHealth);

            return states.SetState(context.Signer, playerState.Encode());
        }
    }
}
