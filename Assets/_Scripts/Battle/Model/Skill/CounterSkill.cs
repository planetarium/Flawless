using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Battle.Skill
{
    public abstract class CounterSkill : SkillBase
    {
        public class Result
        {
            public bool BlockSkill { get; set; }
            public double DamageMultiplier { get; set; } = 1.0;
            public int DealtDamage { get; set; }
        }

        public CounterSkill(
            int speed,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            List<PoseType> availablePoses) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient, finishPose, availablePoses)
        {
        }

        public abstract Result Counter(ICharacter caster, SkillBase target);
    }
}
