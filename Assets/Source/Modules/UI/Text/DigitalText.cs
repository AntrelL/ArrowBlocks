namespace IJunior.UI
{
    public class DigitalText : TextInfo
    {
        private float _value;

        public float Value
        {
            get => _value;
            set => Text = PrefixText + value;
        }
    }
}