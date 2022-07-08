using Flawless.Actions;
using Flawless.Data;
using Flawless.Models.Encounters;
using Flawless.States;
using Flawless.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.Action;
using Libplanet.Unity;
using UnityEngine;

namespace Flawless.Battle
{
    [RequireComponent(typeof(Game))]
    public class EncounterHandler : MonoBehaviour
    {
        [SerializeField]
        private BattleSkillDecision skillSelection = null;

        [SerializeField]
        private BattleUI battleUI = null;

        public Character Player { get; private set; }
        public Character Enemy { get; private set; }

        private List<string> _enemySkills;

        private void Start()
        {
            var playerState = GetPlayerState();

            if (playerState is { })
            {
                Player = playerState.GetCharacter();
                StartBattleEncounter();
            }
        }

        public void OnCreateAccount()
        {
            Player = GetPlayerState().GetCharacter();
            StartBattleEncounter();
        }

        public void StartBattleEncounter()
        {
            var playerState = GetPlayerState();
            var sceneState = playerState.SceneState;
            var encounter = sceneState.GetEncounter();

            switch (encounter)
            {
                case BattleEncounter be:
                    Enemy = (encounter as BattleEncounter).Enemy;
                    battleUI.SetStatView(Player, Enemy);
                    _enemySkills = TableManager.Instance.SkillPresetSheet[Enemy.SkillPresetID].Skills;
                    skillSelection.Show(_enemySkills, Preview, Confirm);
                    break;
                default:
                    break;
            }
        }

        public void Preview(List<string> playerSkills)
        {
            var clonedPlayer = Player.Clone();
            var clonedEnemy = Enemy.Clone();

            var simulator = new BattleSimulator();
            var (victory, skillLogs) = simulator.Simulate(
                clonedPlayer,
                clonedEnemy,
                playerSkills,
                _enemySkills,
                TableManager.Instance.SkillSheet);
            battleUI.SetStatView(Player, Enemy);
            battleUI.PreviewBattle(clonedPlayer, clonedEnemy, skillLogs, () =>
            {
                skillSelection.Show();
                battleUI.SetStatView(Player, Enemy);
            });
        }

        public void Confirm(List<string> playerSkills)
        {
            UnityEngine.Debug.Log($"@@ {skillSelection.DecidedSkills.Count} @@");
            UnityEngine.Debug.Log($"@@ {skillSelection.DecidedSkills.ToImmutableList().Count} @@");
            Game.instance.Agent.MakeTransaction(
                new PolymorphicAction<ActionBase>[]
                {
                    new BattleAction(skillSelection.DecidedSkills.ToImmutableList()),
                }
            );
        }

        private PlayerState GetPlayerState()
        {
            var agent = Game.instance.Agent;
            var raw = agent.GetState(agent.Address);

            if (raw is Bencodex.Types.Dictionary bdict)
            {
                return new PlayerState(bdict);
            }
            else
            {
                return null;
            }
        }
    }
}
