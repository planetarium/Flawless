using Flawless.Battle.Skill;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Flawless.Data;

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
            List<string> playerSkills,
            List<string> enemySkills,
            SkillSheet skillSheet)
        {
            var turn = 0;
            while (turn < TurnLimit)
            {
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
