using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Flawless.UI
{
    public class TextButton : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        [SerializeField]
        private Button button;

        private Action _onClick;

        public string Text
        {
            set => text.text = value;
        }

        private void Awake()
        {
            button.onClick.AddListener(() => _onClick?.Invoke());
        }

        public void Set(string str, Action onClick)
        {
            _onClick = onClick;
            text.text = str;
        }
    }
}
