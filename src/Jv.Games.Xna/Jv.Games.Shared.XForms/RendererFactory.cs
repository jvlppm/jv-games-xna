namespace Jv.Games.Xna.XForms
{
    using System;
    using Xamarin.Forms;

    public static class RendererFactory
    {
        public static IControlRenderer Create(Element element)
        {
#if PORTABLE
            throw new NotImplementedException();
#else
            if (!Forms.IsInitialized)
                throw new InvalidOperationException("Xamarin.Forms not initialized");

            if (element == null)
                throw new NotImplementedException();

            var renderer = Registrar.Registered.GetHandler<IControlRenderer>(element.GetType());
            if (renderer != null)
                renderer.Model = element;
            return renderer;
#endif
        }

        internal static void ScanForRenderers()
        {
            Registrar.RegisterAll(new []{
                typeof(ExportRendererAttribute)
            });
        }
    }
}
