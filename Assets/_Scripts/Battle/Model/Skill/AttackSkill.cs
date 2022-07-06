using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Battle.Skill
{
    public abstract class AttackSkill : SkillBase
    {
        public AttackSkill(
            int speed,
            int atkCoefficient,
            int dexCoefficient,
            int intCoefficient) :
            base(speed, atkCoefficient, dexCoefficient, intCoefficient)
        {

        }

        public override int Use(ICharacter caster, ICharacter target)
        {
            var damage =
                caster.Stat.ATK * ATKCoefficient +
                caster.Stat.DEX * DEXCoefficient +
                caster.Stat.INT * INTCoefficient;

            return target.GetDamage(damage);
        }
    }
}
