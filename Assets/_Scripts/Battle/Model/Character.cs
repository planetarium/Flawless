using Flawless.Battle.Skill;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flawless.Battle
{
    public class Character : ICharacter
    {
        public List<string> Skills { get; private set; }
        public CharacterStat Stat { get; private set; }
        public PoseType Pose { get; set; }

        private readonly Dictionary<string, int> _skillCooldownMap;

        public Character(int str, int dex, int @int)
        {
            Skills = new List<string>();
            Stat = new CharacterStat(str, dex, @int);
            _skillCooldownMap = new();
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

            var skillName = skill.GetType().Name;
            if (_skillCooldownMap.TryGetValue(skillName, out var cooldown) && cooldown > 0)
            {
                var skillLog = new SkillLog()
                {
                    Skill = skill,
                    TurnCount = turnCount,
                    BlockedByCooldown = true,
                };
                return skillLog;
            }

            _skillCooldownMap[skillName] = skill.Cooldown + 1;
            return skill.Use(turnCount, this, target, counter);
        }

        public void ReduceCooldowns()
        {
            foreach (var key in _skillCooldownMap.Keys.ToList())
            {
                --_skillCooldownMap[key];
            }
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
