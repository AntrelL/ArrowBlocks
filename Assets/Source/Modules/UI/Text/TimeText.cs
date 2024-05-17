using System;

namespace IJunior.UI
{
    public class TimeText : DigitalText
    {
        public override void SetValue(float time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            string textTime = timeSpan.Minutes + ":" + timeSpan.ToString(@"ss\.ff");

            Value = time;
            SetText(textTime);
        }
    }
}