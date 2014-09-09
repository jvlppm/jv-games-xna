namespace Jv.Games.Xna.XForms
{
    using Xamarin.Forms;

    class PlatformEngine : IPlatformEngine
    {
        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            var renderer = RendererFactory.GetRenderer(view);
            return renderer.Measure(new Size(widthConstraint, heightConstraint));
        }

        public bool Supports3D
        {
            get { return false; }
        }
    }
}
