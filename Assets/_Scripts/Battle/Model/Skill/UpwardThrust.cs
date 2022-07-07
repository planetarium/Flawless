using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class UpwardThrust : AttackSkill
    {
        public UpwardThrust(
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
            return target.Pose == PoseType.High ? 1.4 : 1.0;
        }
    }
}
