using UnityEngine;

namespace IJunior.UI
{
    public class VersionText : TextInfo
    {
        public override void Initialize()
        {
            base.Initialize();

            Text = PrefixText + Application.version;
        }
    }
}