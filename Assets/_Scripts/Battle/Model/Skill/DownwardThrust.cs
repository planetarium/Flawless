using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class DownwardThrust : AttackSkill
    {
        public DownwardThrust(
            int speed,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            List<PoseType> availablePoses) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {

        }

        public override double GetPowerMultiplier(ICharacter caster, ICharacter target)
        {
            return target.Pose == PoseType.Low ? 1.4 : 1.0;
        }
    }
}
