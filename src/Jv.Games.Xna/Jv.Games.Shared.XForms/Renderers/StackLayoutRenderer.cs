[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.StackLayout),
    typeof(Jv.Games.Xna.XForms.Renderers.StackLayoutRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.Linq;
    using Xamarin.Forms;

    public class StackLayoutRenderer : StackLayoutRenderer<StackLayout>
    {
    }

    public class StackLayoutRenderer<TModel> : LayoutRenderer<TModel>
        where TModel : StackLayout
    {
        public IEnumerable<IControlRenderer> Children
        {
            get
            {
                if (Model == null)
                    return Enumerable.Empty<IControlRenderer>();

                return from c in Model.Children
                       where c.IsVisible
                       select ChildrenRenderers[c];
            }
        }

        public StackLayoutRenderer()
        {
            HandleProperty(StackLayout.SpacingProperty, HandleMeasureChange);
            HandleProperty(StackLayout.OrientationProperty, HandleMeasureChange);
        }

        protected override Size MeasureOverride(Size availableSize)
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

        protected override Xamarin.Forms.Rectangle ArrangeOverride(Xamarin.Forms.Rectangle finalRect)
        {
            if (Model.Orientation == StackOrientation.Horizontal)
                return ArrangeHorizontally(ref finalRect);
            return ArrangeVertically(ref finalRect);
        }

        private Xamarin.Forms.Rectangle ArrangeVertically(ref Xamarin.Forms.Rectangle finalRect)
        {
            var expandableChildrenCount = Model.Children.Count(c => c.VerticalOptions.Expands);
            var extraSpace = finalRect.Height - MeasuredSize.Height;
            var itemExtraSpace = expandableChildrenCount == 0 ? 0 : extraSpace / expandableChildrenCount;

            double x = 0, y = 0;

            foreach (var child in Model.Children)
            {
                var rend = ChildrenRenderers[child];
                rend.Arrange(new Xamarin.Forms.Rectangle(
                    x,
                    y,
                    finalRect.Width,
                    rend.MeasuredSize.Height + (child.VerticalOptions.Expands ? itemExtraSpace : 0)
                ));
                y += rend.RenderArea.Height + Model.Spacing;
            }

            return base.ArrangeOverride(finalRect);
        }

        private Xamarin.Forms.Rectangle ArrangeHorizontally(ref Xamarin.Forms.Rectangle finalRect)
        {
            var expandableChildrenCount = Model.Children.Count(c => c.HorizontalOptions.Expands);
            var extraSpace = finalRect.Width - MeasuredSize.Width;
            var itemExtraSpace = expandableChildrenCount == 0 ? 0 : extraSpace / expandableChildrenCount;

            double x = 0, y = 0;

            foreach (var child in Model.Children)
            {
                var rend = ChildrenRenderers[child];
                rend.Arrange(new Xamarin.Forms.Rectangle(
                    x,
                    y,
                    rend.MeasuredSize.Width + (child.HorizontalOptions.Expands ? itemExtraSpace : 0),
                    finalRect.Height
                ));
                x += rend.RenderArea.Width + Model.Spacing;
            }

            return base.ArrangeOverride(finalRect);
        }
    }
}
