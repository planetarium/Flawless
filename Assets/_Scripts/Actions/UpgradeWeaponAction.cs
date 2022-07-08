using System;
using System.Collections.Generic;
using Flawless.States;
using Flawless.Models.Encounters;
using Libplanet;
using Libplanet.Action;
using Libplanet.Unity;
using UnityEngine;
using Bencodex.Types;

namespace Flawless.Actions
{
    [ActionType("upgrade_weapon_action")]
    public class UpgradeWeaponAction : ActionBase
    {
        private long _attack;
        private long _defense;
        private long _health;
        private long _speed;

        // Used for reflection when deserializing a stored action.
        public UpgradeWeaponAction()
        {
        }

        public UpgradeWeaponAction(
            long attack,
            long defense,
            long health,
            long speed)
        {
            _attack = attack;
            _defense = defense;
            _health = health;
            _speed = speed;
        }

        // Used for serialzing an action.
        public override IValue PlainValue
        {
            get
            {
                IEnumerable<KeyValuePair<IKey, IValue>> pairs = new[]
                {
                    new KeyValuePair<IKey, IValue>(
                        (Text) nameof(_attack),
                        (Integer) _attack
                    ),
                    new KeyValuePair<IKey, IValue>(
                        (Text) nameof(_defense),
                        (Integer) _defense
                    ),
                    new KeyValuePair<IKey, IValue>(
                        (Text) nameof(_health),
                        (Integer) _health
                    ),
                    new KeyValuePair<IKey, IValue>(
                        (Text) nameof(_speed),
                        (Integer) _speed
                    ),
                };

                return new Dictionary(pairs);
            }
        }
        // Used for deserializing a stored action.
        public override void LoadPlainValue(IValue plainValue)
        {
            var asDict = (Dictionary) plainValue;

            _attack = (Integer) asDict[nameof(_attack)];
            _defense = (Integer) asDict[nameof(_defense)];
            _health = (Integer) asDict[nameof(_health)];
            _speed = (Integer) asDict[nameof(_speed)];
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
            if (!(encounter is SmithEncounter))
            {
                throw new Exception($"Not in smith now. actual: {encounter}");
            }
            else if (playerState.Gold < cost)
            {
                throw new Exception(
                    $"Not enough gold; balance: {playerState.Gold}, cost: {cost}");
            }

            weaponState = weaponState.UpgradeWeapon(
                health: _health,
                attack: _attack,
                defense: _defense,
                speed: _speed);
            playerState = playerState.SubtractGold(cost);

            return states
                .SetState(playerState.WeaponAddress, weaponState.Encode())
                .SetState(context.Signer, playerState.Encode());
        }
    }
}
