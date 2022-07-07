using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Data
{
    public class TableManager : MonoBehaviour
    {
        public static TableManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TableManager>();
                }

                if (_instance == null)
                {
                    var go = new GameObject();
                    _instance = go.AddComponent<TableManager>();
                }

                if (!_instance.Initialized)
                {
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public static TableManager _instance = null;

        [SerializeField]
        private TextAsset skillSheet;

        [SerializeField]
        private TextAsset skillPresetSheet;

        [SerializeField]
        private TextAsset weaponSheet;

        public SkillSheet SkillSheet { get; set; }
        public SkillPresetSheet SkillPresetSheet { get; set; }
        public WeaponSheet WeaponSheet { get; set; }

        public bool Initialized { get; set; }

        public void Initialize()
        {
            var presetTable = Resources.Load<TextAsset>("TableSheets/SkillPresetSheet");
            var presetSheet = new SkillPresetSheet();
            presetSheet.Set(presetTable.text);
            SkillPresetSheet = presetSheet;

            var skillTable = Resources.Load<TextAsset>("TableSheets/SkillSheet");
            var skillSheet = new SkillSheet();
            skillSheet.Set(skillTable.text);
            SkillSheet = skillSheet;

            var weaponTable = Resources.Load<TextAsset>("TableSheets/WeaponSheet");
            var weaponSheet = new WeaponSheet();
            weaponSheet.Set(weaponTable.text);
            WeaponSheet = weaponSheet;
        }

    }
}