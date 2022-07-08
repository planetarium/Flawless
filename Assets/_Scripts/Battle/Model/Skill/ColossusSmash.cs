
using System;
using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class ColossusSmash : AttackSkill
    {
        public ColossusSmash(
            int speed,
            int cooldown,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            List<PoseType> availablePoses) :
            base(speed, cooldown, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {

        }

        public override SkillLog Use(int turnCount, ICharacter caster, ICharacter target, CounterSkill counter)
        {
            return base.Use(turnCount, caster, target, null);
        }
    }
}
