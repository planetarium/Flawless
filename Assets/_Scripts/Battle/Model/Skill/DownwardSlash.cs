
using System;
using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class DownwardSlash : AttackSkill
    {
        public DownwardSlash(
            int speed,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            List<PoseType> availablePoses) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {

        }

        public override double GetDamageMultiplier(ICharacter caster, ICharacter target)
        {
            return target.Pose == PoseType.Low ? 2.0 : 1.0;
        }
    }
}
