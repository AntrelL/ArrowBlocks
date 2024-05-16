using IJunior.CompositeRoot;
using UnityEngine.UI;

namespace IJunior.UI
{
    public class Icon : Script
    {
        protected Image Image;

        public virtual void Initialize()
        {
            Image = GetComponent<Image>();
        }
    }
}