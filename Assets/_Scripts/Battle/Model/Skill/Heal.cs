using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Battle.Skill
{
    public class Heal : SkillBase
    {
        public Heal(
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
            // 10% of hp + skill power
            var healAmount = (caster.Stat.BaseHP * 0.1 + CalculatePower(caster)) * counterMultiplier;
            var roundedAmount = (int)Math.Round(healAmount, MidpointRounding.AwayFromZero);
            var impactedHp = Math.Clamp(caster.Stat.HP + roundedAmount, caster.Stat.HP, caster.Stat.BaseHP);
            skillLog.HealAmount = impactedHp - caster.Stat.HP;

            caster.Stat.HP = impactedHp;
        }
    }
}
