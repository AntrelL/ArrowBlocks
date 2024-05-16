using IJunior.CompositeRoot;
using UnityEngine.UI;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class SoundSwitch : Script, IActivatable
    {
        private static bool IsSoundActive = true;

        [SerializeField] Color _disabledColor;

        private Color _normalColor;
        private Button _button;
        private Image _image;

        public void Initialize()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _normalColor = _image.color;

            SetSoundActive(IsSoundActive);
        }

        public void OnActivate()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        public void OnDeactivate()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        public void OnButtonClick()
        {
            SetSoundActive(IsSoundActive == false);
        }

        private void SetSoundActive(bool value)
        {
            if (value)
                ActivateSound();
            else
                DeactivateSound();
        }

        private void ActivateSound()
        {
            AudioListener.volume = 1f;
            _image.color = _normalColor;
            IsSoundActive = true;
        }

        private void DeactivateSound()
        {
            AudioListener.volume = 0f;
            _image.color = _disabledColor;
            IsSoundActive = false;
        }
    }
}