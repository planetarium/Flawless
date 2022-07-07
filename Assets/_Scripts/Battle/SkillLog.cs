using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Flawless.Battle.Skill
{
    public class SkillLog
    {
        public struct CharacterStatus
        {
            public int hp;
            public int baseHp;
            public int atk;
            public int def;
            public int spd;
            public int lifesteal;
            public PoseType pose;


            public CharacterStatus(int hp, int baseHp, int atk, int def, int spd, int lifesteal, PoseType pose)
            {
                this.hp = hp;
                this.baseHp = baseHp;
                this.atk = atk;
                this.def = def;
                this.spd = spd;
                this.lifesteal = lifesteal;
                this.pose = pose;
            }
        }

        public CharacterStatus CasterStatus { get; set; }
        public CharacterStatus TargetStatus { get; set; }
        public string CasterName { get; set; }
        public SkillBase Skill { get; set; }
        public int TurnCount { get; set; }
        public bool Blocked { get; set; }
        public bool BlockedByPose { get; set; }
        public bool BlockedByCounter { get; set; }
        public bool BlockedByCooldown { get; set; }
        public int Speed { get; set; }
        public int CounteredDamage { get; set; }
        public int DealtDamage { get; set; }
        public bool DamageBlocked { get; set; }
        public double DamageMultiplier { get; set; }
        public int LifestealAmount { get; set; }
        public int HealAmount { get; set; }
    }
}
