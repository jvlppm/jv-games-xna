[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.View),
    typeof(Jv.Games.Xna.XForms.Renderers.ViewRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Xamarin.Forms;

    public class ViewRenderer : VisualElementRenderer
    {
        public new View Model { get { return (View)base.Model; } }

        public ViewRenderer()
        {
            HandleProperty(View.HorizontalOptionsProperty, HandleArrangeChange);
            HandleProperty(View.VerticalOptionsProperty, HandleArrangeChange);
        }

        protected virtual bool HandleArrangeChange(BindableProperty prop)
        {
            InvalidateArrange();
            return true;
        }

        protected override Xamarin.Forms.Rectangle ArrangeOverride(Xamarin.Forms.Rectangle finalRect)
        {
            var xDiff = finalRect.Width - MeasuredSize.Width;
            switch (Model.HorizontalOptions.Alignment)
            {
                case LayoutAlignment.Start:
                    finalRect.Width = MeasuredSize.Width;
                    break;
                case LayoutAlignment.Center:
                    finalRect.X += xDiff / 2;
                    finalRect.Width = MeasuredSize.Width;
                    break;
                case LayoutAlignment.End:
                    finalRect.X += xDiff;
                    finalRect.Width = MeasuredSize.Width;
                    break;
            }

            var yDiff = finalRect.Height - MeasuredSize.Height;
            switch (Model.VerticalOptions.Alignment)
            {
                case LayoutAlignment.Start:
                    finalRect.Height = MeasuredSize.Height;
                    break;
                case LayoutAlignment.Center:
                    finalRect.Y += yDiff / 2;
                    finalRect.Height = MeasuredSize.Height;
                    break;
                case LayoutAlignment.End:
                    finalRect.Y += yDiff;
                    finalRect.Height = MeasuredSize.Height;
                    break;
            }

            return finalRect;
        }
    }
}
