using IJunior.CompositeRoot;
using IJunior.UI;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class LeaderboardLine : Script
    {
        [SerializeField] private DigitalText _rankText;
        [SerializeField] private PlainText _nameText;
        [SerializeField] private TimeText _timeText;

        public void Initialize()
        {
            _rankText.Initialize();
            _nameText.Initialize();
            _timeText.Initialize();
        }

        public void Activate() => gameObject.SetActive(true);

        public void Deactivate() => gameObject.SetActive(false);

        public void SetData(int position, string playerName, float time)
        {
            _rankText.SetValue(position);
            _nameText.SetValue(playerName);
            _timeText.SetValue(time);
        }
    }
}