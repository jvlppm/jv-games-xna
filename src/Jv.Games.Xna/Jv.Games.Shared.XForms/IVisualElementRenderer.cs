namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;
    using Rectangle=Xamarin.Forms.Rectangle;

    public interface IVisualElementRenderer : IRegisterable
    {
        VisualElement Model { get; set; }

        SizeRequest Measure(Size availableSize);
        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
    }
}
