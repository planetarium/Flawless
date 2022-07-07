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

        [SerializeField]
        private TextAsset skillDescriptionAsset;

        private Dictionary<string, string> _skillNameMap;

        private Dictionary<string, string> _skillDescriptionMap;

        public bool Initialized { get; set; }

        protected void Initialize()
        {
            SetMap(ref _skillNameMap, skillNameAsset);
            SetMap(ref _skillDescriptionMap, skillDescriptionAsset);
            Initialized = true;
        }

        private void SetMap(ref Dictionary<string, string> map, TextAsset asset)
        {
            if (!asset)
            {
                return;
            }

            map = new Dictionary<string, string>();
            var csv = asset.text;
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

                map[fields[0]] = fields[1].Replace("[newline]", "\n"); ;
            }
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

        public string GetSkillDescription(string key)
        {
            if (_skillDescriptionMap.TryGetValue(key, out var value))
            {
                return value;
            }

            Debug.LogError($"Skill description [{key}] not found.");
            return $"!{key}!";
        }
    }
}
