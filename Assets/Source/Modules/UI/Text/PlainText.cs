namespace IJunior.UI
{
    public class PlainText : TextInfo
    {
        public string Value => Text;

        public void SetValue(string value) => SetText(value);
    }
}