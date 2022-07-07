using System;
using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class AnkleCut : CounterSkill
    {
        public AnkleCut(
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

        public override Result Counter(ICharacter caster, SkillBase target)
        {
            var result = new Result();
            if (target is AttackSkill)
            {
                result.BlockSkill = true;
            }

            return result;
        }

        public override void ProcessEffect(SkillLog skillLog, ICharacter caster, ICharacter target, double counterMultiplier = 1)
        {
            
        }
    }
}

