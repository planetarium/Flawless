using Flawless.Battle;
using Flawless.Battle.Skill;
using Flawless.Data;
using Flawless.Util;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Test
{
    public class BattleTest
    {
        [Test]
        public void Test()
        {
            var player = new Character(5, 4, 0);
            player.Skills.Add("UpwardSlash");
            player.Skills.Add("DownwardSlash");
            player.Skills.Add("UpwardThrust");
            player.Skills.Add("DownwardThrust");
            player.Skills.Add("Heal");
            player.Skills.Add("SideStep");
            var enemy = new Character(3, 2, 0);
            enemy.Skills.Add("UpwardSlash");
            enemy.Skills.Add("DownwardSlash");
            enemy.Skills.Add("UpwardThrust");
            enemy.Skills.Add("DownwardThrust");
            enemy.Skills.Add("Heal");
            enemy.Skills.Add("SideStep");

            var presetTable = Resources.Load<TextAsset>("TableSheets/SkillPresetSheet");
            var presetSheet = new SkillPresetSheet();
            presetSheet.Set(presetTable.text);

            var playerSkills = new List<string>()
            {
                "DownwardSlash",
                "UpwardSlash",
                "DownwardThrust",
                "SideStep",
                "UpwardSlash",
                "DownwardSlash",
                "UpwardThrust",
                "SideStep",
                "UpwardSlash",
                "DownwardThrust",
            };
            var rnd = Random.Range(1, 4);
            var enemySkills = presetSheet[rnd].Skills;

            player.Pose = PoseType.High;
            enemy.Pose = PoseType.High;

            var skillTable = Resources.Load<TextAsset>("TableSheets/SkillSheet");
            var skillSheet = new SkillSheet();
            skillSheet.Set(skillTable.text);

            var weaponTable = Resources.Load<TextAsset>("TableSheets/WeaponSheet");
            var weaponSheet = new WeaponSheet();
            weaponSheet.Set(weaponTable.text);

            var simulator = new BattleSimulator();
            var (victory, skillLogs) = simulator.Simulate(player, enemy, playerSkills, enemySkills, skillSheet);
            foreach (var log in skillLogs)
            {
                Debug.Log(log.SkillLogToString());
            }

            Assert.True(victory);
        }
    }
}
