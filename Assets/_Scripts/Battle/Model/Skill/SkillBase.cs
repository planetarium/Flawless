
using System;
using System.Collections.Generic;

namespace Flawless.Battle.Skill
{
    public abstract class SkillBase
    {
        public int Speed { get; private set; }
        public int Cooldown { get; private set; }
        public double ATKCoefficient { get; private set; }
        public double DEXCoefficient { get; private set; }
        public double INTCoefficient { get; private set; }
        public PoseType FinishPose { get; private set; }
        public List<PoseType> AvailablePoses { get; private set; }

        public SkillBase(
            int speed,
            int cooldown,
            double atkCoefficient,
            double dexCoefficient,
            double intCoefficient,
            PoseType finishPose,
            List<PoseType> availablePoses)
        {
            Speed = speed;
            Cooldown = cooldown;
            ATKCoefficient = atkCoefficient;
            DEXCoefficient = dexCoefficient;
            INTCoefficient = intCoefficient;
            FinishPose = finishPose;
            AvailablePoses = availablePoses;
        }

        public virtual SkillLog Use(int turnCount, ICharacter caster, ICharacter target, CounterSkill counter)
        {

            var skillLog = new SkillLog()
            {
                TurnCount = turnCount,
                Skill = this,
            };

            if (!AvailablePoses.Contains(caster.Pose))
            {
                skillLog.BlockedByPose = true;
                return skillLog;
            }

            var counterResult = counter?.Counter(target, caster, this) ?? null;
            if (counterResult == null)
            {
                ProcessEffect(skillLog, caster, target);
                caster.Pose = FinishPose;
            }
            else
            {
                if (counterResult.DealtDamage > 0)
                {
                    skillLog.CounteredDamage = caster.DealDamage(counterResult.DealtDamage);
                }

                if (counterResult.BlockSkill)
                {
                    skillLog.BlockedByCounter = true;
                    return skillLog;
                }

                ProcessEffect(skillLog, caster, target, counterResult.DamageMultiplier);
                caster.Pose = FinishPose;
            }

            return skillLog;
        }

        public abstract void ProcessEffect(SkillLog skillLog, ICharacter caster, ICharacter target, double counterMultiplier = 1.0);

        public virtual int CalculatePower(ICharacter caster)
        {
            return (int)Math.Round(
                caster.Stat.ATK * ATKCoefficient +
                caster.Stat.DEX * DEXCoefficient +
                caster.Stat.INT * INTCoefficient,
                MidpointRounding.AwayFromZero);
        }

        public virtual double GetPowerMultiplier(ICharacter caster, ICharacter target)
        {
            return 1.0;
        }
    }
}
