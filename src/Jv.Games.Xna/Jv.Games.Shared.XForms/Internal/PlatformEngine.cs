namespace Jv.Games.Xna.XForms
{
    using Jv.Games.Xna.XForms.Renderers;
    using Xamarin.Forms;

    class PlatformEngine : IPlatformEngine
    {
        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            var renderer = VisualElementRenderer.GetRenderer(view);
            return renderer.Measure(new Size(widthConstraint, heightConstraint));
        }

        public bool Supports3D
        {
            get { return false; }
        }
    }
}
