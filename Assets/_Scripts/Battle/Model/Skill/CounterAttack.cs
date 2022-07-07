using System;
using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class CounterAttack : CounterSkill
    {
        public CounterAttack(
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

        public override Result Counter(ICharacter caster, ICharacter target, SkillBase targetSkill)
        {
            var result = new Result();
            if (targetSkill is AttackSkill)
            {
                result.BlockSkill = true;
                result.DealtDamage = CalculatePower(caster);
            }

            return result;
        }

        public override void ProcessEffect(SkillLog skillLog, ICharacter caster, ICharacter target, double counterMultiplier = 1)
        {
            
        }
    }
}

