
using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class UpwardSlash : AttackSkill
    {
        public UpwardSlash(
            int speed,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            List<PoseType> availablePoses) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {

        }
    }
}
