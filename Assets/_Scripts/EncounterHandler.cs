using Flawless.Data;
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

        private void Awake()
        {
            Player = new Character(4, 4, 0);
            Player.Skills.Add("UpwardSlash");
            Player.Skills.Add("DownwardSlash");
            Player.Skills.Add("UpwardThrust");
            Player.Skills.Add("DownwardThrust");
            Player.Skills.Add("Heal");
            Player.Skills.Add("SideStep");
            Enemy = new Character(4, 3, 0);
            Enemy.Skills.Add("UpwardSlash");
            Enemy.Skills.Add("DownwardSlash");
            Enemy.Skills.Add("UpwardThrust");
            Enemy.Skills.Add("DownwardThrust");
            Enemy.Skills.Add("Heal");
            Enemy.Skills.Add("SideStep");

            Player.Pose = PoseType.High;
            Enemy.Pose = PoseType.High;

            var presetTable = Resources.Load<TextAsset>("TableSheets/SkillPresetSheet");
            var presetSheet = new SkillPresetSheet();
            presetSheet.Set(presetTable.text);
            var rnd = Random.Range(1, 4);
            _enemySkills = presetSheet[rnd].Skills;
            StartBattleEncounter();
        }

        public void StartBattleEncounter()
        {
            skillSelection.Show(Enemy.Stat, _enemySkills, Preview);
        }

        public void Preview(List<string> playerSkills)
        {
            battleUI.PreviewBattle(Player, Enemy, playerSkills, _enemySkills, skillSelection.Show);
        }
    }
}
