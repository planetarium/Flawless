
namespace Flawless.Battle.Skill
{
    public class DownwardSlash : AttackSkill
    {
        public DownwardSlash(
            int speed,
            int atkCoefficient,
            int dexCoefficient,
            int intCoefficient) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient)
        {

        }

        public override int Use(ICharacter caster, ICharacter target)
        {
            return base.Use(caster, target);
        }
    }
}
