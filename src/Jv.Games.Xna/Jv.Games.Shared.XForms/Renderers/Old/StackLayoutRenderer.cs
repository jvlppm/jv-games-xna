/*[assembly: Jv.Games.Xna.XForms.ExportRenderer(
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

        protected override SizeRequest MeasureContentOverride(Size availableSize)
        {
            Size measuredSize = Size.Zero;
            if (Model.Orientation == StackOrientation.Horizontal)
            {
                foreach (var child in Model.Children.OrderBy(c => c.HorizontalOptions.Expands? 1 : 0))
                {
                    var rend = ChildrenRenderers[child];
                    rend.Measure(availableSize);
                    measuredSize.Width += rend.MeasuredSize.Width + Model.Spacing;
                    availableSize.Width -= rend.MeasuredSize.Width;
                    if (measuredSize.Height < rend.MeasuredSize.Height)
                        measuredSize.Height = rend.MeasuredSize.Height;
                }
                measuredSize.Width -= Model.Spacing;
            }
            else
            {
                foreach (var child in Model.Children.OrderBy(c => c.VerticalOptions.Expands ? 1 : 0))
                {
                    var rend = ChildrenRenderers[child];
                    rend.Measure(availableSize);
                    measuredSize.Height += rend.MeasuredSize.Height + Model.Spacing;
                    availableSize.Height -= rend.MeasuredSize.Height;
                    if (measuredSize.Width < rend.MeasuredSize.Width)
                        measuredSize.Width = rend.MeasuredSize.Width;
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
            var containerArea = base.ArrangeOverride(finalRect);

            double x = 0;
            double y = 0;

            var spacingTotal = Model.Spacing * (Model.Children.Count - 1);
            var nonExpandableHeight = Model.Children.Where(c => !c.VerticalOptions.Expands).Sum(c => ChildrenRenderers[c].MeasuredSize.Height);
            var expandableItemHeight = (finalRect.Height - nonExpandableHeight - spacingTotal) / expandableChildrenCount;

            foreach (var child in Model.Children)
            {
                var rend = ChildrenRenderers[child];
                var itemHeight = child.VerticalOptions.Expands ? expandableItemHeight : rend.MeasuredSize.Height;
                rend.Arrange(new Rectangle(
                    x,
                    y,
                    containerArea.Width,
                    itemHeight
                ));
                y += itemHeight + Model.Spacing;
            }

            return containerArea;
        }

        Rectangle ArrangeHorizontally(ref Rectangle finalRect)
        {
            var expandableChildrenCount = Model.Children.Count(c => c.HorizontalOptions.Expands);
            var containerArea = base.ArrangeOverride(finalRect);

            double x = 0;
            double y = 0;

            var spacingTotal = Model.Spacing * (Model.Children.Count - 1);
            var nonExpandableHeight = Model.Children.Where(c => !c.HorizontalOptions.Expands).Sum(c => ChildrenRenderers[c].MeasuredSize.Width);
            var expandableItemWidth = (finalRect.Width - nonExpandableHeight - spacingTotal) / expandableChildrenCount;

            foreach (var child in Model.Children)
            {
                var rend = ChildrenRenderers[child];
                var itemWidth = child.HorizontalOptions.Expands ? expandableItemWidth : rend.MeasuredSize.Width;
                rend.Arrange(new Rectangle(
                    x,
                    y,
                    itemWidth,
                    containerArea.Height
                ));
                x += itemWidth + Model.Spacing;
            }

            return containerArea;
        }
    }
}
*/