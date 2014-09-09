namespace Jv.Games.Xna.XForms
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xamarin.Forms;

    public delegate bool PropertyHandler(BindableProperty property);

    public class PropertyTracker
    {
        public void SetTarget(BindableObject obj)
        {
            throw new NotImplementedException();
        }

        public void AddHandler(BindableProperty property, PropertyHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}
