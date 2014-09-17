namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public interface IRenderer : IRegisterable
    {
        IRenderer Parent { get; set; }
        IEnumerable<IRenderer> Children { get; }

        bool IsVisible { get; set; }

        SizeRequest Measure(Size availableSize);
        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
    }

    public interface IVisualElementRenderer : IRenderer
    {
        VisualElement Model { get; set; }
        void InvalidateTransformations();
        void InvalidateAlpha();
    }
}
