using Flawless.Battle.Skill;
using System;
using System.Collections.Generic;

namespace Flawless.Battle
{
    public class Character : ICharacter
    {
        #region Base Stat

        public List<SkillBase> Skills { get; private set; }
        public CharacterStat Stat { get; private set; }

        #endregion

        public Character(int str, int dex, int @int)
        {
            Skills = new List<SkillBase>();
            Stat = new CharacterStat(str, dex, @int);
        }

        public int UseSkill(SkillBase skill, ICharacter target)
        {
            return skill.Use(this, target);
        }

        public int GetDamage(int damage)
        {
            var actualDamage = Math.Clamp(damage - Stat.DEF, 0, int.MaxValue);
            Stat.HP -= actualDamage;
            return actualDamage;
        }
    }
}
