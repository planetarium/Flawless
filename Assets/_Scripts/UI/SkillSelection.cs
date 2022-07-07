using Flawless.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.UI
{
    public class SkillSelection : MonoBehaviour
    {
        [SerializeField]
        private List<TurnView> turnViews = null;

        public void Show(CharacterStat enemyStat, List<string> enemySkillNames)
        {
            for (int i = 0; i < turnViews.Count; ++i)
            {
                turnViews[i].SetData(i, enemyStat, enemySkillNames[i]);
            }
        }
    }
}
