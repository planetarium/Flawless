
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
            params PoseType[] availablePoses) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {

        }

        public override int Use(ICharacter caster, ICharacter target)
        {
            return base.Use(caster, target);
        }
    }
}
