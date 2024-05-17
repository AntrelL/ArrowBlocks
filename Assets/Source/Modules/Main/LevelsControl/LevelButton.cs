using IJunior.CompositeRoot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(Button))]
    public class LevelButton : Script
    {
        [SerializeField] private Sprite _passed;
        [SerializeField] private TMP_Text _tmpText;

        public Button Button { get; private set; }

        public void Initialize(int number)
        {
            Button = GetComponent<Button>();
            _tmpText.text = number.ToString();
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
