
using System;
using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class HorizontalSlash : AttackSkill
    {
        public HorizontalSlash(
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
    }
}
