
using System;

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
            params PoseType[] availablePoses) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {

        }

        public override double GetDamageMultiplier(ICharacter caster, ICharacter target)
        {
            return target.Pose == PoseType.Low ? 1.5 : 1.0;
        }
    }
}
