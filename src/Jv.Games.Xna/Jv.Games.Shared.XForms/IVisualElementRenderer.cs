namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public interface ICellRenderer : IRegisterable
    {
        Cell Model { get; set; }
        VisualElement CreateVisual(object item);
    }

    public interface IVisualElementRenderer : IRegisterable
    {
        VisualElement Model { get; set; }

        bool IsVisible { get; set; }

        IVisualElementRenderer Parent { get; set; }
        IEnumerable<IVisualElementRenderer> Children { get; }


        SizeRequest Measure(Size availableSize);
        void Layout(Xamarin.Forms.Rectangle bounds);

        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);

        void Appeared();
        void Disappeared();

        void InvalidateTransformations();
        void InvalidateAlpha();
    }
}
