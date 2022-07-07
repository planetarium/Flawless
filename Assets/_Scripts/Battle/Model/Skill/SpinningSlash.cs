using System;
using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class SpinningSlash : CounterSkill
    {
        public SpinningSlash(
            int speed,
            int cooldown,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            List<PoseType> availablePoses) :
            base(speed, cooldown, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {

        }

        public override SkillLog Use(int turnCount, ICharacter caster, ICharacter target, CounterSkill counter)
        {
            var skillLog = base.Use(turnCount, caster, target, counter);

            var damage = CalculatePower(caster);
            var multiplier = GetPowerMultiplier(caster, target);
            var actualDamage = target.DealDamage(damage, multiplier);
            var lifestealAmount = (int)Math.Round(actualDamage * (caster.Stat.LifestealPercentage / 100.0),
                MidpointRounding.AwayFromZero);

            skillLog.DealtDamage = actualDamage;
            skillLog.LifestealAmount = lifestealAmount;
            skillLog.DamageMultiplier = multiplier;
            return skillLog;
        }

        public override Result Counter(ICharacter caster, ICharacter target, SkillBase targetSkill)
        {
            var result = new Result();
            if (targetSkill is AttackSkill)
            {
                result.BlockSkill = true;
                result.DealtDamage = (int) Math.Round(
                    CalculatePower(caster) * 0.5,
                    MidpointRounding.AwayFromZero);
            }

            return result;
        }

        public override void ProcessEffect(SkillLog skillLog, ICharacter caster, ICharacter target, double counterMultiplier = 1)
        {
            
        }
    }
}

