using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public class SideStep : CounterSkill
    {
        public SideStep(
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

