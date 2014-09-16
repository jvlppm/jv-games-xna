namespace Jv.Games.Xna.XForms
{
    using System;
    using Xamarin.Forms;

    public static class RendererFactory
    {
        static BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(IRenderer), typeof(RendererFactory), null);

        public static IRenderer GetRenderer(BindableObject obj)
        {
            return (IRenderer)obj.GetValue(RendererProperty);
        }
        public static void SetRenderer(Element obj, IRenderer renderer)
        {
            obj.SetValue(RendererProperty, renderer);
        }

        internal static IRenderer Create(Element element)
        {
            if (element is VisualElement)
                return Create((VisualElement)element);
            throw new NotImplementedException();
        }

        internal static IVisualElementRenderer Create(VisualElement element)
        {
            if (!Forms.IsInitialized)
                throw new InvalidOperationException("Xamarin.Forms not initialized");

            if (element == null)
                throw new NotImplementedException();

            var renderer = Registrar.Registered.GetHandler<IVisualElementRenderer>(element.GetType());
            if (renderer != null)
            {
                element.IsPlatformEnabled = true;
                renderer.Model = element;
                SetRenderer(element, renderer);
            }
            return renderer;
        }

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
