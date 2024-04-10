using UnityEngine;

namespace IJunior.UI
{
    public class ColorIndicatingIcon : Icon
    {
        [SerializeField] private Color _activatedColor;

        private Color _normalColor;

        public override void Initialize()
        {
            base.Initialize();
            _normalColor = Image.color;
        }

        public void Activate() => Image.color = _activatedColor;

        public void Deactivate() => Image.color = _normalColor;
    }
}