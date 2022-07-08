using Flawless.Battle.Skill;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Flawless.Battle
{
    [Serializable]
    public class Character : ICharacter
    {
        public List<string> Skills { get; private set; }
        public CharacterStat Stat { get; private set; }
        public PoseType Pose { get; set; }

        private readonly Dictionary<string, int> _skillCooldownMap;
        public int SkillPresetID { get; set; }

        public Character(int str, int dex, int @int)
            : this(str, dex, @int, new List<string>())
        {
        }

        public Character(int str, int dex, int @int, List<string> skills)
        {
            Pose = PoseType.High;
            Skills = skills;
            Stat = new CharacterStat(str, dex, @int);
            _skillCooldownMap = new();
        }

        public Character Clone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (Character) formatter.Deserialize(ms);
            }
        }

        public SkillLog UseSkill(int turnCount, SkillBase skill, ICharacter target, CounterSkill counter = null)
        {
            if (skill == null)
            {
                var log = new SkillLog()
                {
                    TurnCount = turnCount,
                    Blocked = true,
                };
                return log;
            }

            var skillName = skill.GetType().Name;
            if (_skillCooldownMap.TryGetValue(skillName, out var cooldown) && cooldown > 0)
            {
                var log = new SkillLog()
                {
                    Skill = skill,
                    TurnCount = turnCount,
                    BlockedByCooldown = true,
                };
                return log;
            }

            _skillCooldownMap[skillName] = skill.Cooldown + 1;
            var skillLog = skill.Use(turnCount, this, target, counter);
            return skillLog;
        }

        public void ReduceCooldowns()
        {
            foreach (var key in _skillCooldownMap.Keys.ToList())
            {
                --_skillCooldownMap[key];
            }
        }

        public int DealDamage(int damage, double multiplier = 1.0)
        {
            var reducedDamage = Math.Round((damage - Stat.DEF) * multiplier, MidpointRounding.AwayFromZero);
            var actualDamage = (int) Math.Clamp(reducedDamage, 0, damage * multiplier);
            Stat.HP -= actualDamage;
            return actualDamage;
        }

        public static Character GenerateEnemy(long stage, long encounter, long seed, bool hard = false)
        {
            var stageNumber = (int)stage;
            if (hard)
            {
                ++stageNumber;
            }

            var rnd = new Random((int)seed);
            var rndStr = rnd.Next(-stageNumber, stageNumber + 1);
            var rndDex = rnd.Next(-stageNumber, stageNumber + 1);
            var presetId = rnd.Next(((int)stage - 1) * 3, (int)stage * 3) + 1;

            Character enemy = new(
                3 + (stageNumber - 1) * 3 + rndStr,
                2 + (stageNumber - 1) * 2 + rndDex,
                (stageNumber - 1) * 2);
            enemy.Skills.Add("UpwardSlash");
            enemy.Skills.Add("DownwardSlash");
            enemy.Skills.Add("UpwardThrust");
            enemy.Skills.Add("DownwardThrust");
            enemy.Skills.Add("HorizontalSlash");
            enemy.Skills.Add("AnkleCut");
            enemy.Skills.Add("SpinningSlash");
            enemy.Skills.Add("ColossusSmash");
            enemy.Skills.Add("CounterAttack");
            enemy.Skills.Add("Heal");
            enemy.Skills.Add("SideStep");
            enemy.SkillPresetID = presetId;
            return enemy;
        }
    }
}
