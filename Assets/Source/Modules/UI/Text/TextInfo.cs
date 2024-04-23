using IJunior.CompositeRoot;
using UnityEngine;
using TMPro;

namespace IJunior.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextInfo : Script
    {
        [SerializeField] private string _prefixText;
        [SerializeField] private string _undefinedValueText;

        private TMP_Text _tmpText;

        protected string Text
        {
            get => _tmpText.text;
            set
            {
                _tmpText.text = _prefixText + value;
            }
        }

        public void SetUndefinedValue()
        {
            Text = _undefinedValueText;
        }

        public virtual void Initialize()
        {
            _tmpText = GetComponent<TMP_Text>();
        }
    }
}