using System;
using System.Collections.Generic;
using System.Linq;
using Flawless.States;
using Flawless.Models.Encounters;
using Libplanet.Action;
using Libplanet.Unity;

namespace Flawless.Actions
{
    [ActionType("upgrade_weapon_action")]
    public class UpgradeWeaponAction : ActionBase
    {
        // Used for reflection when deserializing a stored action.
        public UpgradeWeaponAction()
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
            WeaponState weaponState =
                states.GetState(playerState.WeaponAddress) is Bencodex.Types.Dictionary weaponStateEncoded
                    ? new WeaponState(weaponStateEncoded)
                    : throw new ArgumentException($"Can't find weapon state at {playerState.WeaponAddress}");
            SceneState sceneState = playerState.SceneState;
            long cost = weaponState.Grade * 5;

            Encounter encounter = sceneState.GetEncounter();
            if (!(encounter is SmithEncounter smithEncounter))
            {
                throw new Exception($"Not in smith now. actual: {encounter}");
            }
            else if (playerState.Gold < cost)
            {
                throw new Exception(
                    $"Not enough gold; balance: {playerState.Gold}, cost: {cost}");
            }
            else
            {
                long salt = weaponState.Health + weaponState.Attack + weaponState.Defense + weaponState.Speed + weaponState.Lifesteal;
                int randomValue = Utils.Random(100, playerState.SceneState.Seed, salt);

                List<long> points = new List<long>() { 0, 0, 0, 0 };
                points[randomValue / 25] = 1;
                int index = randomValue / 25;
                points[index] = 1;
                points = (randomValue % 10 == 0)
                    ? points.Select(x => x * 2).ToList()
                    : points;

                weaponState = weaponState.UpgradeWeapon(
                    health: points[0],
                    attack: points[1],
                    defense: points[2],
                    speed: points[3]);
                playerState = playerState.SubtractGold(cost);

                return states
                    .SetState(playerState.WeaponAddress, weaponState.Encode())
                    .SetState(context.Signer, playerState.Encode());
            }
        }
    }
}
