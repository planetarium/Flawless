using Flawless.Battle;
using Flawless.Battle.Skill;
using Flawless.Data;
using Flawless.Util;
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
        private SkillSelection skillSelection = null;

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

        public void Awake()
        {
            var player = new Character(4, 4, 0);
            player.Skills.Add("UpwardSlash");
            player.Skills.Add("DownwardSlash");
            player.Skills.Add("UpwardThrust");
            player.Skills.Add("DownwardThrust");
            player.Skills.Add("Heal");
            player.Skills.Add("SideStep");
            var enemy = new Character(4, 3, 0);
            enemy.Skills.Add("UpwardSlash");
            enemy.Skills.Add("DownwardSlash");
            enemy.Skills.Add("UpwardThrust");
            enemy.Skills.Add("DownwardThrust");
            enemy.Skills.Add("Heal");
            enemy.Skills.Add("SideStep");

            var presetTable = Resources.Load<TextAsset>("TableSheets/SkillPresetSheet");
            var presetSheet = new SkillPresetSheet();
            presetSheet.Set(presetTable.text);

            var playerSkills = new List<string>()
            {
                "DownwardSlash",
                "UpwardSlash",
                "DownwardThrust",
                "SideStep",
                "UpwardSlash",
                "DownwardSlash",
                "UpwardThrust",
                "SideStep",
                "UpwardSlash",
                "DownwardThrust",
            };
            var rnd = Random.Range(1, 4);
            var enemySkills = presetSheet[rnd].Skills;
            skillSelection.Show(enemy.Stat, enemySkills);

            player.Pose = PoseType.High;
            enemy.Pose = PoseType.High;

            var skillTable = Resources.Load<TextAsset>("TableSheets/SkillSheet");
            var skillSheet = new SkillSheet();
            skillSheet.Set(skillTable.text);

            var weaponTable = Resources.Load<TextAsset>("TableSheets/WeaponSheet");
            var weaponSheet = new WeaponSheet();
            weaponSheet.Set(weaponTable.text);

            var simulator = new BattleSimulator();
            var (victory, skillLogs) = simulator.Simulate(player, enemy, playerSkills, enemySkills, skillSheet);
            WriteLogs(skillLogs);
        }

        public void WriteLogs(IEnumerable<SkillLog> logs)
        {
            StartCoroutine(CoWriteLogs(logs));
        }

        public IEnumerator CoWriteLogs(IEnumerable<SkillLog> logs)
        {
            var sb = new StringBuilder();
            var turnCount = -1;
            foreach (var log in logs)
            {
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
            }
        }
    }
}

