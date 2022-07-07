using Flawless.Battle;
using Flawless.Data;
using System;
using System.Collections;
using System.Collections.Generic;
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

        Action<List<string>> _onDecided;

        private void Awake()
        {
            previewButton.onClick.AddListener(Confirm);
        }

        public void Show(
            CharacterStat enemyStat,
            List<string> enemySkills,
            Action<List<string>> onDecided)
        {
            _onDecided = onDecided;
            for (int i = 0; i < turnViews.Count; ++i)
            {
                turnViews[i].SetData(i, enemyStat, enemySkills[i]);
            }

            Show();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Confirm()
        {
            gameObject.SetActive(false);
            _onDecided?.Invoke(new List<string>());
        }
    }
}
