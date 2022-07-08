using Flawless.Battle;
using Flawless.Battle.Skill;
using Flawless.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Flawless.UI
{
    public class StatView : MonoBehaviour
    {
        [SerializeField]
        private Slider hpBar;

        [SerializeField]
        private Text hpText;

        [SerializeField]
        private Text attackText;

        [SerializeField]
        private Text defenseText;

        [SerializeField]
        private Text speedText;

        [SerializeField]
        private Text lifestealText;

        public void UpdateView(SkillLog.CharacterStatus stat)
        {
            var hp = Mathf.Clamp(stat.hp, 0, stat.baseHp);
            hpBar.value = (float) hp / stat.baseHp;
            hpText.text = $"{hp}/{stat.baseHp} - {stat.pose.PoseToString()}";
            attackText.text = $"공격력 : {stat.atk}";
            defenseText.text = $"방어력 : {stat.def}";
            speedText.text = $"스피드 : {stat.spd}";
            lifestealText.text = $"흡혈 : {stat.lifesteal}%";
        }

        public void UpdateView(Character character)
        {
            var stat = character.Stat;

            var hp = Mathf.Clamp(stat.HP, 0, stat.BaseHP);
            hpBar.value = (float)hp / stat.BaseHP;
            hpText.text = $"{hp}/{stat.BaseHP} - {character.Pose.PoseToString()}";
            attackText.text = $"공격력 : {stat.ATK}";
            defenseText.text = $"방어력 : {stat.DEF}";
            speedText.text = $"스피드 : {stat.SPD}";
            lifestealText.text = $"흡혈 : {stat.LifestealPercentage}%";
        }
    }
}

