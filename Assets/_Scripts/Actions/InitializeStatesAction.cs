using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flawless.Models.Encounters;
using Flawless.Data;
using Flawless.States;
using Libplanet;
using Libplanet.Action;
using Libplanet.Unity;

namespace Flawless.Actions
{    // Used for reflection when deserializing a stored action.
    [ActionType("initialize_states_action")]
    public class InitalizeStatesAction : ActionBase
    {
        private InitializeStatesActionPlainValue _plainValue;

        // Used for reflection when deserializing a stored action.
        public InitalizeStatesAction()
        {
        }

        // Used for creating a new action.
        public InitalizeStatesAction(
            string skillPresetSheetCsv
        )
        {
            _plainValue = new InitializeStatesActionPlainValue(
                skillPresetSheetCsv
            );
        }


        // Used for serialzing an action.
        public override Bencodex.Types.IValue PlainValue => _plainValue.Encode();

        // Used for deserializing a stored action.
        public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
        {
            _plainValue = plainValue is Bencodex.Types.Dictionary bdict
                ? new InitializeStatesActionPlainValue(bdict)
                : throw new ArgumentException(
                    $"Invalid {nameof(plainValue)} type: {plainValue.GetType()}");
        }

        // Executes an action.
        // This is what gets called when a block containing an action is mined
        // or appended to a blockchain.
        public override IAccountStateDelta Execute(IActionContext context)
        {
            // FIXME this action can be created by **anybody**
            // We should add condition check.

            // Retrieves the previously stored state.
            IAccountStateDelta states = context.PreviousStates;
            var environmentState = new EnvironmentState(
                _plainValue.SkillPresetSheetCsv
            );

            return states.SetState(
                EnvironmentState.EnvironmentAddress,
                environmentState.Encode()
            );
        }
    }
}
