using IJunior.CompositeRoot;
using UnityEngine.UI;
using UnityEngine;

namespace IJunior.UI
{
    public class IndicatingIcon : Script
    {
        [SerializeField] private Sprite _activatedIcon;

        private Sprite _normalIcon;
        private Image _image;

        public void Initialize()
        {
            _image = GetComponent<Image>();
            _normalIcon = _image.sprite;
        }

        public void Activate()
        {
            _image.sprite = _activatedIcon;
        }

        public void Deactivate()
        {
            _image.sprite = _normalIcon;
        }
    }
}