using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Flawless.Battle;
using Flawless.Data;
using Flawless.Models.Encounters;
using Flawless.States;
using Libplanet.Action;
using Libplanet.Unity;

namespace Flawless.Actions
{
    // Used for reflection when deserializing a stored action.
    [ActionType("battle_action")]
    public class BattleAction : ActionBase
    {
        private const string SkillSheetCsv = "skill_name,speed,cooldown,atk_coeff,dex_coeff,int_coeff,finish_pose,available_poses\r\nDownwardSlash,10,0,1.0,0.25,0,Low,High\r\nUpwardSlash,10,0,1.0,0.25,0,High,Low\r\nDownwardThrust,20,2,1.25,0.5,0,Special,High\r\nUpwardThrust,20,2,1.25,0.5,0,Special,Low\r\nHorizontalSlash,25,2,1.0,0.25,0,High,Low\r\nAnkleCut,25,2,0.75,0.5,0,Low,Low\r\nHeal,0,2,0,0,0.4,Low,High,Low,Low,High,Special\r\nSideStep,999,2,0,0,0,Low,High,Low,Special";
        private ImmutableList<string> _skills;

        public BattleAction()
        {
        }

        public BattleAction(ImmutableList<string> skills)
        {
            _skills = skills;
        }

        public override Bencodex.Types.IValue PlainValue => new Bencodex.Types.List(
            _skills.Select(s => (Bencodex.Types.Text) s).Cast<Bencodex.Types.IValue>()
        );

        public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
        {
            _skills = ((Bencodex.Types.List) plainValue).Select(v => v.ToString()).ToImmutableList();
        }

        public override IAccountStateDelta Execute(IActionContext context)
        {
            // Retrieves the previously stored state.
            IAccountStateDelta states = context.PreviousStates;
            EnvironmentState environmentState =
                states.GetState(EnvironmentState.EnvironmentAddress) is Bencodex.Types.Dictionary environmentStateEncoded
                    ? new EnvironmentState(environmentStateEncoded)
                    : throw new ArgumentException("No environment found; please run InitalizeStatesAction first.");
            PlayerState playerState =
                states.GetState(context.Signer) is Bencodex.Types.Dictionary playerStateEncoded
                    ? new PlayerState(playerStateEncoded)
                    : throw new ArgumentException($"Invalid player state at {context.Signer}.");

            var playerCharacter = playerState.GetCharacter();
            if (playerState.WeaponAddress != default)
            {
                WeaponState weaponState =
                    states.GetState(playerState.WeaponAddress) is Bencodex.Types.Dictionary weaponStateEncoded
                        ? new WeaponState(weaponStateEncoded)
                        : throw new ArgumentException($"Can't find weapon state at {playerState.WeaponAddress}");
                playerCharacter.Stat.Weapon = weaponState.GetWeapon();
            }

            Encounter encounter = playerState.SceneState.GetEncounter(environmentState);
            if (!(encounter is BattleEncounter))
            {
                throw new Exception("Not in BattleEncounter.");
            }
            Character enemyCharacter = ((BattleEncounter) encounter).Enemy;
            List<string> enemySkills = enemyCharacter.Skills;

            var simulator = new BattleSimulator();
            var skillSheet = new SkillSheet();
            skillSheet.Set(SkillSheetCsv);
            (bool victory, _) = simulator.Simulate(
                playerCharacter,
                enemyCharacter,
                _skills.ToList(),
                enemySkills,
                skillSheet
            );

            if (victory)
            {
                playerState = playerState
                    .AddExperience(25 + playerCharacter.Stat.INT * 5)
                    .AddGold(25)
                    .Heal((long)(playerCharacter.Stat.BaseHP * 0.1f))
                    .Proceed(context.Random.Seed);
            }
            else
            {
                playerState = playerState.ResetPlayer(context.Random.Seed);
            }

            return states.SetState(context.Signer, playerState.Encode());
        }
    }
}