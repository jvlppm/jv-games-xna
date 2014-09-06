[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.StackLayout),
    typeof(Jv.Games.Xna.XForms.Renderers.StackLayoutRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using System.Linq;
    using Xamarin.Forms;

    public class StackLayoutRenderer : LayoutRenderer
    {
        public new StackLayout Model { get { return (StackLayout)base.Model; } }

        public StackLayoutRenderer()
        {
            HandleProperty(StackLayout.SpacingProperty, HandleMeasurePropertyChanged);
            HandleProperty(StackLayout.OrientationProperty, HandleMeasurePropertyChanged);
        }

        protected override Size MeasureContentOverride(Size availableSize)
        {
            Size measuredSize = Size.Zero;
            if (Model.Orientation == StackOrientation.Horizontal)
            {
                foreach (var child in Children)
                {
                    child.Measure(availableSize);
                    measuredSize.Width += child.MeasuredSize.Width + Model.Spacing;
                    availableSize.Width -= child.MeasuredSize.Width;
                    if (measuredSize.Height < child.MeasuredSize.Height)
                        measuredSize.Height = child.MeasuredSize.Height;
                }
                measuredSize.Width -= Model.Spacing;
            }
            else
            {
                foreach (var child in Children)
                {
                    child.Measure(availableSize);
                    measuredSize.Height += child.MeasuredSize.Height + Model.Spacing;
                    availableSize.Height -= child.MeasuredSize.Height;
                    if (measuredSize.Width < child.MeasuredSize.Width)
                        measuredSize.Width = child.MeasuredSize.Width;
                }
                measuredSize.Height -= Model.Spacing;
            }

            return measuredSize;
        }

        protected override Rectangle ArrangeOverride(Rectangle finalRect)
        {
            if (Model.Orientation == StackOrientation.Horizontal)
                return ArrangeHorizontally(ref finalRect);
            return ArrangeVertically(ref finalRect);
        }

        Rectangle ArrangeVertically(ref Rectangle finalRect)
        {
            var expandableChildrenCount = Model.Children.Count(c => c.VerticalOptions.Expands);
            var extraSpace = finalRect.Height - MeasuredSize.Height;
            var itemExtraSpace = expandableChildrenCount == 0 || extraSpace < 0 ? 0 : extraSpace / expandableChildrenCount;

            var containerArea = base.ArrangeOverride(finalRect);

            double x = containerArea.X;
            double y = 0;

            foreach (var child in Model.Children)
            {
                var childExtraSpace = child.VerticalOptions.Expands ? itemExtraSpace : 0;
                var rend = ChildrenRenderers[child];
                rend.Arrange(new Rectangle(
                    x,
                    y,
                    containerArea.Width,
                    rend.MeasuredSize.Height + childExtraSpace
                ));
                y += rend.RenderArea.Height + childExtraSpace + Model.Spacing;
            }

            return containerArea;
        }

        Rectangle ArrangeHorizontally(ref Rectangle finalRect)
        {
            var expandableChildrenCount = Model.Children.Count(c => c.HorizontalOptions.Expands);
            var extraSpace = finalRect.Width - MeasuredSize.Width;
            var itemExtraSpace = expandableChildrenCount == 0 || extraSpace < 0 ? 0 : extraSpace / expandableChildrenCount;

            var containerArea = base.ArrangeOverride(finalRect);

            double x = 0;
            double y = containerArea.Y;

            foreach (var child in Model.Children)
            {
                var childExtraSpace = child.VerticalOptions.Expands ? itemExtraSpace : 0;
                var rend = ChildrenRenderers[child];
                rend.Arrange(new Rectangle(
                    x,
                    y,
                    rend.MeasuredSize.Width + childExtraSpace,
                    containerArea.Height
                ));
                x += rend.RenderArea.Width + childExtraSpace + Model.Spacing;
            }

            return containerArea;
        }
    }
}
