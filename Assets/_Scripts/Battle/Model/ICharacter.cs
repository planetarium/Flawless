using Flawless.Battle.Skill;
using System.Collections.Generic;

namespace Flawless.Battle
{
    public interface ICharacter
    {
        public CharacterStat Stat { get; }
        public List<string> Skills { get; }
        public PoseType Pose { get; set; }

        public SkillLog UseSkill(int turnCount, SkillBase skill, ICharacter target, CounterSkill counter);
        public int DealDamage(int damage, double multiplier = 1.0);
    }
}
