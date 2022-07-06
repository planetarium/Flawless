using Flawless.Battle.Skill;
using System;
using System.Collections.Generic;

namespace Flawless.Battle
{
    public class Character : ICharacter
    {
        public List<SkillBase> Skills { get; private set; }
        public CharacterStat Stat { get; private set; }
        public PoseType Pose { get; set; }


        public Character(int str, int dex, int @int)
        {
            Skills = new List<SkillBase>();
            Stat = new CharacterStat(str, dex, @int);
        }

        public int UseSkill(SkillBase skill, ICharacter target)
        {
            if (skill == null)
            {
                return default;
            }

            return skill.Use(this, target);
        }

        public int GetDamage(int damage, double multiplier)
        {
            var reducedDamage = Math.Round((damage - Stat.DEF) * multiplier, MidpointRounding.AwayFromZero);
            var actualDamage = (int) Math.Clamp(reducedDamage, 0, damage * multiplier);
            Stat.HP -= actualDamage;
            return actualDamage;
        }
    }
}
