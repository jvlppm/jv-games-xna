namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;
    using Rectangle=Xamarin.Forms.Rectangle;

    public interface IRenderer : IRegisterable
    {
        //Element Model { get; set; }

        SizeRequest Measure(Size availableSize);
        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
    }

    public interface IVisualElementRenderer : IRenderer
    {
        VisualElement Model { get; set; }
        void InvalidateTransformations();
    }
}
