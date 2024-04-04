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
            int indentationToFractionalPart = 2;
            int wholePart = (int)Math.Truncate(time);

            float fractionalPart = (float)Math.Round(time - wholePart, indentationToFractionalPart);
            string fractionalPartText = fractionalPart.ToString().Substring(indentationToFractionalPart);

            Text = PrefixText + $"{wholePart}:{fractionalPartText}s";
        }
    }
}