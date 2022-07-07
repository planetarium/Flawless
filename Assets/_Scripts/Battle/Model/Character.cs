using Flawless.Battle.Skill;
using System;
using System.Collections.Generic;

namespace Flawless.Battle
{
    public class Character : ICharacter
    {
        public List<string> Skills { get; private set; }
        public CharacterStat Stat { get; private set; }
        public PoseType Pose { get; set; }


        public Character(int str, int dex, int @int)
        {
            Skills = new List<string>();
            Stat = new CharacterStat(str, dex, @int);
        }

        public SkillLog UseSkill(int turnCount, SkillBase skill, ICharacter target, CounterSkill counter = null)
        {
            if (skill == null)
            {
                var skillLog = new SkillLog()
                {
                    TurnCount = turnCount,
                    Blocked = true,
                };
                return skillLog;
            }

            return skill.Use(turnCount, this, target, counter);
        }

        public int DealDamage(int damage, double multiplier = 1.0)
        {
            var reducedDamage = Math.Round((damage - Stat.DEF) * multiplier, MidpointRounding.AwayFromZero);
            var actualDamage = (int) Math.Clamp(reducedDamage, 0, damage * multiplier);
            Stat.HP -= actualDamage;
            return actualDamage;
        }
    }
}
