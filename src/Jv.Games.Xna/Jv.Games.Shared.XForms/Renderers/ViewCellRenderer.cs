[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.ViewCell),
    typeof(Jv.Games.Xna.XForms.Renderers.ViewCellRenderer))]

namespace Jv.Games.Xna.XForms.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xamarin.Forms;

    public class ViewCellRenderer : CellRenderer
    {
        public ViewCellRenderer()
        {
        }

        public override Xamarin.Forms.VisualElement CreateVisual(object item)
        {
            return ((ViewCell)Model).View;
        }
    }
}
