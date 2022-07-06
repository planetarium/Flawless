using System;
using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public abstract class AttackSkill : SkillBase
    {
        public AttackSkill(
            int speed,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            List<PoseType> availablePoses) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {

        }

        public override int Use(ICharacter caster, ICharacter target)
        {
            var damage = CalculateDamage(caster, target);
            var multiplier = GetDamageMultiplier(caster, target);
            var actualDamage = target.GetDamage(damage, multiplier);
            caster.Pose = FinishPose;
            return actualDamage;
        }

        public virtual int CalculateDamage(ICharacter caster, ICharacter target)
        {
            return (int) Math.Round(
                caster.Stat.ATK * ATKCoefficient +
                caster.Stat.DEX * DEXCoefficient +
                caster.Stat.INT * INTCoefficient,
                MidpointRounding.AwayFromZero);
        }

        public virtual double GetDamageMultiplier(ICharacter caster, ICharacter target)
        {
            return 1.0;
        }
    }
}
