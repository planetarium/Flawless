using Flawless.Battle.Skill;
using System.Collections.Generic;
using Flawless.Data;

namespace Flawless.Battle
{
    public class BattleSimulator
    {
        public Character Player { get; private set; }
        public Character Enemy { get; private set; }

        public const int TurnLimit = 10;

        public (bool victory, List<SkillLog> skillLogs) Simulate(
            Character player,
            Character enemy,
            List<string> playerSkills,
            List<string> enemySkills,
            SkillSheet skillSheet)
        {
            Player = player;
            Enemy = enemy;

            var turn = 0;
            var actionLogs = new List<SkillLog>();
            while (turn < TurnLimit)
            {
                player.ReduceCooldowns();
                enemy.ReduceCooldowns();

                var playerSkillName = turn < playerSkills.Count ? playerSkills[turn] : null;
                var enemySkillName = turn < enemySkills.Count ? enemySkills[turn] : null;

                SkillBase playerSkill = null;
                SkillBase enemySkill = null;

                if (player.Skills.Exists(x => x.Equals(playerSkillName)))
                {
                    playerSkill = skillSheet.TryGetValue(playerSkillName, out var playerRow) ?
                        SkillFactory.Instance.CreateSkill(playerRow) : null;
                }

                if (enemy.Skills.Exists(x => x.Equals(enemySkillName)))
                {
                    enemySkill = skillSheet.TryGetValue(enemySkillName, out var enemyRow) ?
                        SkillFactory.Instance.CreateSkill(enemyRow) : null;
                }

                var playerSpeed = playerSkill?.Speed ?? 0 + player.Stat.SPD;
                var enemySpeed = enemySkill?.Speed ?? 0 + enemy.Stat.SPD;

                if (playerSpeed >= enemySpeed)
                {
                    var playerLog = player.UseSkill(turn, playerSkill, enemy);
                    playerLog.Caster = "Player";
                    actionLogs.Add(playerLog);
                    if (enemy.Stat.HP <= 0)
                    {
                        break;
                    }
                    
                    var enemyLog = enemy.UseSkill(turn, enemySkill, player, playerSkill as CounterSkill);
                    enemyLog.Caster = "Enemy";
                    actionLogs.Add(enemyLog);
                    if (player.Stat.HP <= 0)
                    {
                        break;
                    }
                }
                else
                {
                    var enemyLog = enemy.UseSkill(turn, enemySkill, player);
                    enemyLog.Caster = "Enemy";
                    actionLogs.Add(enemyLog);
                    if (player.Stat.HP <= 0)
                    {
                        break;
                    }

                    var playerLog = player.UseSkill(turn, playerSkill, enemy, enemySkill as CounterSkill);
                    playerLog.Caster = "Player";
                    actionLogs.Add(playerLog);
                    if (enemy.Stat.HP <= 0)
                    {
                        break;
                    }
                }
                ++turn;
            }

            var victory = player.Stat.HP > 0 && turn < TurnLimit;
            return (victory, actionLogs);
        }
    }
}
