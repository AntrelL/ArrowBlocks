using IJunior.CompositeRoot;
using UnityEngine;
using TMPro;

namespace IJunior.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextInfo : Script
    {
        [SerializeField] protected string PrefixText;

        private TMP_Text _tmpText;

        protected string Text
        {
            get => _tmpText.text;
            set
            {
                _tmpText.text = value;
            }
        }

        public virtual void Initialize()
        {
            _tmpText = GetComponent<TMP_Text>();
        }
    }
}