using IJunior.CompositeRoot;
using TMPro;
using UnityEngine;

namespace IJunior.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextInfo : Script
    {
        [SerializeField] private bool _useDefaultTextAsPrefix = true;
        [SerializeField] private string _undefinedValueText;

        private string _prefixText;
        private TMP_Text _tmpText;

        protected string Text
        {
            get => _tmpText.text;
            set
            {
                _tmpText.text = _prefixText + value;
            }
        }

        public virtual void Initialize()
        {
            _tmpText = GetComponent<TMP_Text>();
            _prefixText = _useDefaultTextAsPrefix ? _tmpText.text : string.Empty;
        }

        public void SetUndefinedValue()
        {
            Text = _undefinedValueText;
        }
    }
}