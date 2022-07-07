using Flawless.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Flawless.UI
{
    public class BattleSkillDecision : MonoBehaviour
    {
        [SerializeField]
        private List<TurnView> turnViews = null;

        [SerializeField]
        private Button previewButton;

        [SerializeField]
        private Button confirmButton;

        [SerializeField]
        private SkillSelectPopup skillSelectPopup;

        private List<string> _decidedSkills;

        Action<List<string>> _onPreview;

        Action<List<string>> _onConfirm;

        private void Awake()
        {
            previewButton.onClick.AddListener(Preview);
            confirmButton.onClick.AddListener(Confirm);
        }

        public void Show(
            List<string> enemySkills,
            Action<List<string>> onPreview,
            Action<List<string>> onConfirm)
        {
            _decidedSkills = new List<string>();
            _onPreview = onPreview;
            _onConfirm = onConfirm;

            for (int i = 0; i < turnViews.Count; ++i)
            {
                var index = i;
                turnViews[index].SetData(index, enemySkills[index], () => OnEditSkill(index));
            }

            Show();
        }

        private void OnEditSkill(int index)
        {
            if (index > _decidedSkills.Count)
            {
                return;
            }

            ShowSkillSelectPopup(index);
        }

        private void ShowSkillSelectPopup(int index)
        {
            var skillSheet = TableManager.Instance.SkillSheet;
            var prevPose = PoseType.High;
            var prevIndex = index - 1;
            if (prevIndex >= 0 && _decidedSkills.Count > prevIndex)
            {
                var skillName = _decidedSkills[prevIndex];
                var skill = skillSheet[skillName];
                prevPose = skill.FinishPose;
            }

            var availableSkills = skillSheet.Values
                .Where(x => x.AvailablePoses.Contains(prevPose))
                .Select(x => x.Id)
                .ToList();
            skillSelectPopup.Show(availableSkills, skill =>
            {
                if (index >= _decidedSkills.Count)
                {
                    _decidedSkills.Add(skill);
                }
                else
                {
                    _decidedSkills[index] = skill;
                }
                turnViews[index].SetPlayerSkill(skill);
                skillSelectPopup.Close();
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Preview()
        {
            gameObject.SetActive(false);
            _onPreview?.Invoke(_decidedSkills);
        }

        private void Confirm()
        {
            gameObject.SetActive(false);
            _onConfirm?.Invoke(_decidedSkills);
        }
    }
}
