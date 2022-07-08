using Flawless.Battle;
using Flawless.Battle.Skill;
using Flawless.Util;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Flawless.UI
{
    public class TurnView : MonoBehaviour
    {
        [SerializeField]
        private Text turnCountText = null;

        [SerializeField]
        private TextButton playerSkillButton = null;

        [SerializeField]
        private Text enemySkillDescriptionText = null;

        public void SetData(int turnCount, string skillName, Action onEdit)
        {
            turnCountText.text = $"Turn\n{turnCount + 1}";
            playerSkillButton.Set("클릭해서 스킬 설정 (이전 턴 스킬이 설정되어야 함)", onEdit);
            enemySkillDescriptionText.text = LogExtension.GetSkillDescription(skillName);
        }

        public void SetPlayerSkill(string skillName)
        {
            playerSkillButton.Text = LogExtension.GetSkillDescription(skillName);
        }
    }
}

