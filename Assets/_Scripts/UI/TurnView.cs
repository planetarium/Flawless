using Flawless.Battle;
using Flawless.Battle.Skill;
using Flawless.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Flawless.UI
{
    public class TurnView : MonoBehaviour
    {
        [SerializeField]
        private Text turnCountText = null;

        [SerializeField]
        private Text enemySkillDescriptionText = null;

        public void SetData(int turnCount, CharacterStat enemyStat, string skillName)
        {
            turnCountText.text = $"Turn\n{turnCount + 1}";
            var name = LocalizationManager.Instance.GetSkillName(skillName);
            var desc = LocalizationManager.Instance.GetSkillDescription(skillName);
            enemySkillDescriptionText.text = $"{name} : {desc}";
        }
    }
}

