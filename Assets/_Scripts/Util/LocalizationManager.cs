using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flawless.Util
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LocalizationManager>();
                }

                if (_instance == null)
                {
                    var go = new GameObject();
                    _instance = go.AddComponent<LocalizationManager>();
                }
                
                if (!_instance.Initialized)
                {
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public static LocalizationManager _instance = null;

        [SerializeField]
        private TextAsset skillNameAsset;

        private Dictionary<string, string> _skillNameMap;

        public bool Initialized { get; set; }

        protected void Initialize()
        {
            if (skillNameAsset)
            {
                _skillNameMap = new Dictionary<string, string>();
                var csv = skillNameAsset.text;
                var rows = csv.Split(
                    new string[] { Environment.NewLine },
                    StringSplitOptions.None)
                    .Skip(1);

                foreach (var rowString in rows)
                {
                    var fields = rowString.Split('\u002C');
                    if (fields.Length < 2)
                    {
                        continue;
                    }

                    _skillNameMap[fields[0]] = fields[1];
                }
            }

            Initialized = true;
        }

        public string GetSkillName(string key)
        {
            if (_skillNameMap.TryGetValue(key, out var value))
            {
                return value;
            }

            Debug.LogError($"Skill name [{key}] not found.");
            return $"!{key}!";
        }
    }
}
