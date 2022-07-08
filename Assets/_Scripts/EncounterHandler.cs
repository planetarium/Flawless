using Flawless.Data;
using Flawless.Models.Encounters;
using Flawless.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Battle
{
    public class EncounterHandler : MonoBehaviour
    {
        [SerializeField]
        private BattleSkillDecision skillSelection = null;

        [SerializeField]
        private BattleUI battleUI = null;

        public Character Player { get; private set; }
        public Character Enemy { get; private set; }

        private List<string> _enemySkills;

        private int _battleCount = 0;


        private void Awake()
        {
            Player = new Character(4, 4, 0);
            Player.Skills.Add("UpwardSlash");
            Player.Skills.Add("DownwardSlash");
            Player.Skills.Add("UpwardThrust");
            Player.Skills.Add("DownwardThrust");
            Player.Skills.Add("HorizontalSlash");
            Player.Skills.Add("AnkleCut");
            Player.Skills.Add("SpinningSlash");
            Player.Skills.Add("ColossusSmash");
            Player.Skills.Add("CounterAttack");
            Player.Skills.Add("Heal");
            Player.Skills.Add("SideStep");
            Player.Pose = PoseType.High;

            //Enemy = new Character(4, 3, 0);
            //Enemy.Skills.Add("UpwardSlash");
            //Enemy.Skills.Add("DownwardSlash");
            //Enemy.Skills.Add("UpwardThrust");
            //Enemy.Skills.Add("DownwardThrust");
            //Enemy.Skills.Add("HorizontalSlash");
            //Enemy.Skills.Add("AnkleCut");
            //Enemy.Skills.Add("SpinningSlash");
            //Enemy.Skills.Add("ColossusSmash");
            //Enemy.Skills.Add("CounterAttack");
            //Enemy.Skills.Add("Heal");
            //Enemy.Skills.Add("SideStep");
            //Enemy.Pose = PoseType.High;

            //var rnd = Random.Range(1, 4);
            //_enemySkills = TableManager.Instance.SkillPresetSheet[rnd].Skills;
            StartBattleEncounter();
        }

        public void StartBattleEncounter()
        {
            var rnd = new System.Random();
            var stageSeed = rnd.Next();

            var stageNumber = 1 + _battleCount / 4;
            var presetId = rnd.Next((stageNumber - 1) * 3, stageNumber * 3);

            var encounter = Encounter.GenerateBattleOnlyEncounter(
                stageNumber,
                stageSeed);
            Enemy = (encounter as BattleEncounter).Enemy;
            battleUI.SetStatView(Player, Enemy);
            _enemySkills = TableManager.Instance.SkillPresetSheet[Enemy.SkillPresetID].Skills;
            skillSelection.Show(_enemySkills, Preview, Confirm);
            ++_battleCount;
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
            var clonedPlayer = Player.Clone();
            var clonedEnemy = Enemy.Clone();

            var simulator = new BattleSimulator();
            var (victory, skillLogs) = simulator.Simulate(
                clonedPlayer,
                clonedEnemy,
                playerSkills,
                _enemySkills,
                TableManager.Instance.SkillSheet);

            if (victory)
            {
                battleUI.PreviewBattle(Player, Enemy, skillLogs, StartBattleEncounter);
            }
            else
            {
                battleUI.PreviewBattle(clonedPlayer, clonedEnemy, skillLogs, skillSelection.Show);
            }
        }
    }
}
