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

        public override void ProcessEffect(SkillLog skillLog, ICharacter caster, ICharacter target, double counterMultiplier = 1.0)
        {
            if (counterMultiplier == 0.0)
            {
                skillLog.DamageMultiplier = counterMultiplier;
                skillLog.DealtDamage = 0;
                skillLog.DamageBlocked = true;
                return;
            }

            var damage = CalculatePower(caster);
            var multiplier = GetPowerMultiplier(caster, target) * counterMultiplier;
            var actualDamage = target.DealDamage(damage, multiplier);
            var lifestealAmount = (int)Math.Round(actualDamage * (caster.Stat.LifestealPercentage / 100.0),
                MidpointRounding.AwayFromZero);

            skillLog.DealtDamage = actualDamage;
            skillLog.LifestealAmount = lifestealAmount;
            skillLog.DamageMultiplier = multiplier;
        }
    }
}
