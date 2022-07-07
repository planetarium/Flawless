using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Flawless.Battle.Skill
{
    public class SkillLog
    {
        public string Caster { get; set; }
        public SkillBase Skill { get; set; }
        public int TurnCount { get; set; }
        public bool Blocked { get; set; }
        public bool BlockedByPose { get; set; }
        public bool BlockedByCounter { get; set; }
        public bool BlockedByCooldown { get; set; }
        public int CounteredDamage { get; set; }
        public int DealtDamage { get; set; }
        public bool DamageBlocked { get; set; }
        public double DamageMultiplier { get; set; }
        public int LifestealAmount { get; set; }
        public int HealAmount { get; set; }
    }
}
