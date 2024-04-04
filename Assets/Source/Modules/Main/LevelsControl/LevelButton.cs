using IJunior.CompositeRoot;
using UnityEngine.UI;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(Button))]
    public class LevelButton : Script
    {
        [SerializeField] private Sprite _passed;

        public Button Button { get; private set; }

        public void Initialize()
        {
            Button = GetComponent<Button>();
        }

        public void UpdateState(LevelState levelState)
        {
            switch (levelState)
            {
                case LevelState.Passed:
                    Button.image.sprite = _passed;
                    break;
                case LevelState.Opened:
                    Button.interactable = true;
                    break;
                case LevelState.Blocked:
                    Button.interactable = false;
                    break;
            }
        }
    }
}
