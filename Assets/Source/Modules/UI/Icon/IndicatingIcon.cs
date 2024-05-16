using UnityEngine.UI;
using UnityEngine;

namespace IJunior.UI
{
    public class IndicatingIcon : Icon
    {
        [SerializeField] private Sprite _activatedIcon;

        private Sprite _normalIcon;

        public override void Initialize()
        {
            base.Initialize();
            _normalIcon = Image.sprite;
        }

        public void Activate() => Image.sprite = _activatedIcon;

        public void Deactivate() => Image.sprite = _normalIcon;
    }
}