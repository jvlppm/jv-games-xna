namespace Jv.Games.Xna.XForms
{
    using Jv.Games.Xna.XForms.Renderers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ElementView
    {
        public readonly IVisualElementRenderer Renderer;
        public readonly Xamarin.Forms.VisualElement Element;

        public ElementView(Game game, Xamarin.Forms.VisualElement element)
        {
            Renderer = RendererFactory.Create(element);
        }

        public void Draw(GameTime gameTime)
        {
            if (Renderer == null)
                return;
            Renderer.Draw(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            if (Renderer == null)
                return;
            Renderer.Update(gameTime);
        }
    }
}
