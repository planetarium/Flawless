
namespace Flawless.Battle.Skill
{
    public abstract class SkillBase
    {
        public int Speed { get; private set; }
        public int ATKCoefficient { get; private set; }
        public int DEXCoefficient { get; private set; }
        public int INTCoefficient { get; private set; }

        public SkillBase(
            int speed,
            int atkCoefficient,
            int dexCoefficient,
            int intCoefficient)
        {
            Speed = speed;
            ATKCoefficient = atkCoefficient;
            DEXCoefficient = dexCoefficient;
            INTCoefficient = intCoefficient;
        }

        public abstract int Use(ICharacter caster, ICharacter target);
    }
}
