using IJunior.CompositeRoot;

namespace IJunior.UI
{
    public class LinkedDigitalText : TextInfo, IActivatable
    {
        private ILinkedDigitalTextSource _source;

        public void Initialize(ILinkedDigitalTextSource source)
        {
            _source = source;
            base.Initialize();

            OnValueChanged(_source.Value);
        }

        public void OnActivate()
        {
            _source.ValueChanged += OnValueChanged;
        }

        public void OnDeactivate()
        {
            _source.ValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(float value)
        {
            Text = PrefixText + value;
        }
    }
}