using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Screen : Script
    {
        [SerializeField] private bool _isActiveByDefault;

        private CanvasGroup _canvasGroup;

        public void Initialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_isActiveByDefault)
                Open();
            else
                Close();
        }

        public void SwitchTo(Screen targetScreen)
        {
            Close();
            targetScreen.Open();
        }

        private void Open()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void Close()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}