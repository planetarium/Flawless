using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flawless.Util
{
    public class LocalizationManager : MonoSingleton<LocalizationManager>
    {
        [SerializeField]
        private TextAsset skillNameAsset;

        private readonly Dictionary<string, string> _skillNameMap = new Dictionary<string, string>();

        protected override void Awake()
        {
            base.Awake();

            if (skillNameAsset)
            {
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
