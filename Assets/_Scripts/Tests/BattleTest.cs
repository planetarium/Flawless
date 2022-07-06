using Flawless.Battle;
using Flawless.Battle.Skill;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Test
{
    public class BattleTest
    {
        [Test]
        public void Test()
        {
            var testSkill = new UpwardSlash(2, 1, 0, 0);

            var player = new Character(10, 4, 0);
            player.Skills.Add(testSkill);
            var enemy = new Character(5, 4, 0);
            enemy.Skills.Add(testSkill);

            var playerSkills = new List<SkillBase>()
            {
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
            };
            var enemySkills = new List<SkillBase>()
            {
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
                testSkill,
            };

            var simulator = new BattleSimulator();
            var result = simulator.Simulate(player, enemy, playerSkills, enemySkills);
            Assert.True(result);
        }
    }
}
