using Flawless.Battle;
using Flawless.Battle.Skill;
using Flawless.Data;
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
            var skillTable = Resources.Load<TextAsset>("TableSheets/SkillSheet");
            var skillSheet = new SkillSheet();
            skillSheet.Set(skillTable.text);

            var weaponTable = Resources.Load<TextAsset>("TableSheets/WeaponSheet");
            var weaponSheet = new WeaponSheet();
            weaponSheet.Set(weaponTable.text);

            var player = new Character(5, 4, 0);
            player.Skills.Add("UpwardSlash");
            player.Skills.Add("DownwardSlash");
            player.Skills.Add("UpwardThrust");
            player.Skills.Add("DownwardThrust");
            var enemy = new Character(5, 4, 0);
            enemy.Skills.Add("UpwardSlash");
            enemy.Skills.Add("DownwardSlash");
            enemy.Skills.Add("UpwardThrust");
            enemy.Skills.Add("DownwardThrust");

            var playerSkills = new List<string>()
            {
                "DownwardSlash",
                "UpwardThrust",
                "DownwardSlash",
                "UpwardThrust",
                "DownwardSlash",
                "UpwardThrust",
                "DownwardSlash",
                "UpwardThrust",
                "DownwardSlash",
                "UpwardThrust",
            };
            var enemySkills = new List<string>()
            {
                "DownwardSlash",
                "",
                "UpwardThrust",
                "DownwardSlash",
                "UpwardThrust",
                "DownwardSlash",
                "UpwardThrust",
                "DownwardSlash",
                "UpwardThrust",
                "DownwardSlash",
            };

            player.Pose = PoseType.High;
            enemy.Pose = PoseType.Low;

            var simulator = new BattleSimulator();
            var result = simulator.Simulate(player, enemy, playerSkills, enemySkills, skillSheet);
            Assert.True(result);
        }
    }
}
