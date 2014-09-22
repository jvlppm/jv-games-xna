namespace Jv.Games.Xna.XForms
{
    using System;
    using Xamarin.Forms;

    public static class RendererFactory
    {
        public static UIGameComponent AsGameComponent(this VisualElement visual)
        {
            var component = new UIGameComponent();
            var page = visual as Page;

            if (page != null)
            {
                component.SetPage(page);
                return component;
            }

            var view = visual as View;
            if(view != null)
            {
                component.SetPage(new ContentPage { Content = view });
                return component;
            }

            throw new ArgumentException("Not supported visual element type", "visual");
        }
    }
}
