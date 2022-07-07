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

                var playerSpeed = playerSkill == null ? int.MinValue : playerSkill.Speed + player.Stat.SPD;
                var enemySpeed = enemySkill == null ? int.MinValue : enemySkill.Speed + enemy.Stat.SPD;

                if (playerSpeed >= enemySpeed)
                {
                    var playerLog = player.UseSkill(turn, playerSkill, enemy);
                    playerLog.CasterName = "Player";
                    playerLog.Speed = playerSpeed;

                    playerLog.CasterStatus = new SkillLog.CharacterStatus(
                        player.Stat.HP,
                        player.Stat.BaseHP,
                        player.Stat.ATK,
                        player.Stat.DEF,
                        player.Stat.SPD,
                        player.Stat.LifestealPercentage,
                        player.Pose);
                    playerLog.TargetStatus = new SkillLog.CharacterStatus(
                        enemy.Stat.HP,
                        enemy.Stat.BaseHP,
                        enemy.Stat.ATK,
                        enemy.Stat.DEF,
                        enemy.Stat.SPD,
                        enemy.Stat.LifestealPercentage,
                        enemy.Pose);


                    actionLogs.Add(playerLog);
                    if (enemy.Stat.HP <= 0)
                    {
                        break;
                    }

                    var enemyLog = enemy.UseSkill(turn, enemySkill, player, playerSkill as CounterSkill);
                    enemyLog.CasterName = "Enemy";
                    enemyLog.Speed = enemySpeed;

                    enemyLog.TargetStatus = new SkillLog.CharacterStatus(
                        player.Stat.HP,
                        player.Stat.BaseHP,
                        player.Stat.ATK,
                        player.Stat.DEF,
                        player.Stat.SPD,
                        player.Stat.LifestealPercentage,
                        player.Pose);
                    enemyLog.CasterStatus = new SkillLog.CharacterStatus(
                        enemy.Stat.HP,
                        enemy.Stat.BaseHP,
                        enemy.Stat.ATK,
                        enemy.Stat.DEF,
                        enemy.Stat.SPD,
                        enemy.Stat.LifestealPercentage,
                        enemy.Pose);

                    actionLogs.Add(enemyLog);
                    if (player.Stat.HP <= 0)
                    {
                        break;
                    }
                }
                else
                {
                    var enemyLog = enemy.UseSkill(turn, enemySkill, player);
                    enemyLog.CasterName = "Enemy";
                    enemyLog.Speed = enemySpeed;

                    enemyLog.TargetStatus = new SkillLog.CharacterStatus(
                        player.Stat.HP,
                        player.Stat.BaseHP,
                        player.Stat.ATK,
                        player.Stat.DEF,
                        player.Stat.SPD,
                        player.Stat.LifestealPercentage,
                        player.Pose);
                    enemyLog.CasterStatus = new SkillLog.CharacterStatus(
                        enemy.Stat.HP,
                        enemy.Stat.BaseHP,
                        enemy.Stat.ATK,
                        enemy.Stat.DEF,
                        enemy.Stat.SPD,
                        enemy.Stat.LifestealPercentage,
                        enemy.Pose);

                    actionLogs.Add(enemyLog);
                    if (player.Stat.HP <= 0)
                    {
                        break;
                    }

                    var playerLog = player.UseSkill(turn, playerSkill, enemy, enemySkill as CounterSkill);
                    playerLog.CasterName = "Player";
                    playerLog.Speed = playerSpeed;

                    playerLog.CasterStatus = new SkillLog.CharacterStatus(
                        player.Stat.HP,
                        player.Stat.BaseHP,
                        player.Stat.ATK,
                        player.Stat.DEF,
                        player.Stat.SPD,
                        player.Stat.LifestealPercentage,
                        player.Pose);
                    playerLog.TargetStatus = new SkillLog.CharacterStatus(
                        enemy.Stat.HP,
                        enemy.Stat.BaseHP,
                        enemy.Stat.ATK,
                        enemy.Stat.DEF,
                        enemy.Stat.SPD,
                        enemy.Stat.LifestealPercentage,
                        enemy.Pose);

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
