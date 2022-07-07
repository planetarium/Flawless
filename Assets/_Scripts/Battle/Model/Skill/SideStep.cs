using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Battle.Skill
{
    public class SideStep : CounterSkill
    {
        public SideStep(
            int speed,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            List<PoseType> availablePoses) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {

        }

        public override Result Counter(ICharacter caster, SkillBase target)
        {
            var result = new Result();
            if (target is AttackSkill attackSkill)
            {
                result.DamageMultiplier = 0;
                result.DealtDamage = CalculatePower(caster);
            }

            return result;
        }

        public override void ProcessEffect(SkillLog skillLog, ICharacter caster, ICharacter target, double counterMultiplier = 1)
        {
            
        }
    }
}

