using Flawless.Battle;
using Flawless.Battle.Skill;
using Flawless.Data;
using Flawless.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Flawless.UI
{
    public class BattleUI : MonoBehaviour
    {
        [SerializeField]
        private Text battleLogText = null;
        
        [SerializeField]
        private RectTransform contentRect = null;

        [SerializeField]
        private StatView playerStatView = null;

        [SerializeField]
        private StatView enemyStatView = null;

        [SerializeField]
        private float _logDelay = 2f;

        [SerializeField]
        private Action _onClose;

        private Character _player;

        private Character _enemy;

        public void PreviewBattle(
            Character player,
            Character enemy,
            List<string> playerSkills,
            List<string> enemySkills,
            Action onClose)
        {
            _player = player;
            _enemy = enemy;
            var clonedPlayer = player.Clone();
            var clonedEnemy = enemy.Clone();

            _onClose = onClose;
            var skillTable = Resources.Load<TextAsset>("TableSheets/SkillSheet");
            var skillSheet = new SkillSheet();
            skillSheet.Set(skillTable.text);

            var weaponTable = Resources.Load<TextAsset>("TableSheets/WeaponSheet");
            var weaponSheet = new WeaponSheet();
            weaponSheet.Set(weaponTable.text);

            var simulator = new BattleSimulator();
            var (victory, skillLogs) = simulator.Simulate(
                clonedPlayer,
                clonedEnemy,
                playerSkills,
                enemySkills,
                skillSheet);
            WriteLogs(skillLogs);
        }

        public void WriteLogs(IEnumerable<SkillLog> logs)
        {
            StartCoroutine(CoWriteLogs(logs));
        }

        public IEnumerator CoWriteLogs(IEnumerable<SkillLog> logs)
        {
            playerStatView.UpdateView(_player);
            enemyStatView.UpdateView(_enemy);

            var sb = new StringBuilder();
            var turnCount = -1;
            foreach (var log in logs)
            {
                var elapsed = 0f;

                while (!Input.GetKeyDown(KeyCode.Space))
                {
                    if (elapsed >= _logDelay)
                    {
                        break;
                    }
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                yield return null;

                if (turnCount != log.TurnCount)
                {
                    turnCount = log.TurnCount;
                    sb.AppendLine($"====== [Turn {turnCount + 1}] ======");
                }
                
                sb.AppendLine(log.SkillLogToString());
                battleLogText.text = sb.ToString();
                if (string.Compare(log.CasterName, "Player") == 0)
                {
                    playerStatView.UpdateView(log.CasterStatus);
                    enemyStatView.UpdateView(log.TargetStatus);
                }
                else
                {
                    playerStatView.UpdateView(log.TargetStatus);
                    enemyStatView.UpdateView(log.CasterStatus);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
                contentRect.localPosition = new Vector2(
                    contentRect.localPosition.x,
                    contentRect.sizeDelta.y);
            }

            sb.Append("====== Enter 키를 눌러 로그 재결정 ======");
            battleLogText.text = sb.ToString();
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
            _onClose?.Invoke();
            playerStatView.UpdateView(_player);
            enemyStatView.UpdateView(_enemy);
        }
    }
}

