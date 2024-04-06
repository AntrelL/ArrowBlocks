using System;

namespace IJunior.UI
{
    public class TimeText : TextInfo
    {
        private float _value;

        public float Value
        {
            get => _value;
            set => SetTime(value);
        }

        private void SetTime(float time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            string textTime = timeSpan.Minutes + ":" + timeSpan.ToString(@"ss\.ff");

            Text = PrefixText + textTime;
        }
    }
}