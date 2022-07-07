using Flawless.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.UI
{
    public class SkillSelectPopup : MonoBehaviour
    {
        [SerializeField]
        private TextButton prefab;

        private List<GameObject> _objects = new List<GameObject>();

        public void Show(List<string> skills, Action<string> onSelected)
        {
            foreach (var obj in _objects)
            {
                Destroy(obj);
            }

            foreach (var skill in skills)
            {
                var box = Instantiate(prefab, transform);
                _objects.Add(box.gameObject);

                var desc = LogExtension.GetSkillDescription(skill);
                box.Set(desc, () => onSelected?.Invoke(skill));
            }

            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
