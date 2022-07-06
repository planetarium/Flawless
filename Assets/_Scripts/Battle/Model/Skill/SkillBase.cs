
namespace Flawless.Battle.Skill
{
    public abstract class SkillBase
    {
        public int Speed { get; private set; }
        public double ATKCoefficient { get; private set; }
        public double DEXCoefficient { get; private set; }
        public double INTCoefficient { get; private set; }
        public PoseType FinishPose { get; private set; }
        public PoseType[] AvailablePoses { get; private set; }

        public SkillBase(
            int speed,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            params PoseType[] availablePoses)
        {
            Speed = speed;
            ATKCoefficient = atkCoefficient;
            DEXCoefficient = dexCoefficient;
            INTCoefficient = intCoefficient;
            FinishPose = finishPose;
            AvailablePoses = availablePoses;
        }

        public abstract int Use(ICharacter caster, ICharacter target);
    }
}
