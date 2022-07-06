using Flawless.Battle.Skill;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Flawless.Battle
{
    public class BattleSimulator
    {
        public Character Player { get; private set; }
        public Character Enemy { get; private set; }

        public const int TurnLimit = 10;

        public bool Simulate(
            Character player,
            Character enemy,
            List<SkillBase> playerSkills,
            List<SkillBase> enemySkills)
        {
            var turn = 0;
            while (turn < TurnLimit)
            {
                var playerSkill = turn < playerSkills.Count ? playerSkills[turn] : null;
                var enemySkill = turn < enemySkills.Count ? enemySkills[turn] : null;

                if (player.Skills.FirstOrDefault(x => x.Equals(playerSkill)) == default)
                {
                    throw new ArgumentException($"Player skill {playerSkill.GetType().Name} not found.");
                }
                if (enemy.Skills.FirstOrDefault(x => x.Equals(enemySkill)) == default)
                {
                    throw new ArgumentException($"Enemy skill {enemySkill.GetType().Name} not found.");
                }

                var playerSpeed = playerSkill.Speed + player.Stat.SPD;
                var enemySpeed = enemySkill.Speed + enemy.Stat.SPD;

                if (playerSpeed >= enemySpeed)
                {
                    var playerDamage = player.UseSkill(playerSkill, enemy);
                    Debug.Log($"[Turn {turn}] Enemy got {playerDamage} damage. Remaining : {enemy.Stat.HP}");
                    if (enemy.Stat.HP <= 0)
                    {
                        Debug.Log("Enemy defeated.");
                        break;
                    }

                    var enemyDamage = enemy.UseSkill(enemySkill, player);
                    Debug.Log($"Player got {enemyDamage} damage. Remaining : {player.Stat.HP}");
                    if (player.Stat.HP <= 0)
                    {
                        Debug.Log("Player defeated.");
                        break;
                    }
                }
                else
                {
                    var enemyDamage = enemy.UseSkill(enemySkill, player);
                    Debug.Log($"[Turn {turn}] Player got {enemyDamage} damage. Remaining : {player.Stat.HP}");
                    if (player.Stat.HP <= 0)
                    {
                        Debug.Log("Player defeated.");
                        break;
                    }

                    var playerDamage = player.UseSkill(playerSkill, enemy);
                    Debug.Log($"Enemy got {playerDamage} damage. Remaining : {enemy.Stat.HP}");
                    if (enemy.Stat.HP <= 0)
                    {
                        Debug.Log("Enemy defeated.");
                        break;
                    }
                }

                ++turn;
            }

            return player.Stat.HP > 0 && turn < TurnLimit;
        }
    }
}
