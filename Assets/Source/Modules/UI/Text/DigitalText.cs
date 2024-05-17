namespace IJunior.UI
{
    public class DigitalText : TextInfo
    {
        public float Value { get; protected set; }

        public virtual void SetValue(float value)
        {
            Value = value;
            SetText(value.ToString());
        }
    }
}