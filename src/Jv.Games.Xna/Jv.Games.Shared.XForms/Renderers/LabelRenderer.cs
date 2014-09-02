[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Label),
    typeof(Jv.Games.Xna.XForms.Renderers.LabelRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;

    public class LabelRenderer : LabelRenderer<Label>
    {
        public static SpriteFont DefaultFont;
        public static Microsoft.Xna.Framework.Color DefaultTextColor = Microsoft.Xna.Framework.Color.Black;
    }

    public class LabelRenderer<TModel> : ViewRenderer<TModel>
        where TModel : Label
    {
        ContentManager _content;
        SpriteFont _font;
        Microsoft.Xna.Framework.Color TextColor;

        public override void Initialize(Game game)
        {
            _content = game.Content;
            HandleProperty(Label.FontProperty, HandleFont);
            HandleProperty(Label.TextColorProperty, HandleTextColor);
            base.Initialize(game);
        }

        protected override Vector2 MeasureOverride(Vector2 availableSize)
        {
            if (_font != null)
                return _font.MeasureString(Model.Text);
            return base.MeasureOverride(availableSize);
        }

        protected virtual bool HandleFont(BindableProperty prop)
        {
            if (Model.Font.FontFamily == null)
                _font = LabelRenderer.DefaultFont;
            else
                _font = _content.Load<SpriteFont>(Model.Font.FontFamily) ?? LabelRenderer.DefaultFont;
            return true;
        }

        protected virtual bool HandleTextColor(BindableProperty prop)
        {
            if (Model.TextColor == default(Xamarin.Forms.Color))
                TextColor = LabelRenderer.DefaultTextColor;
            else
                TextColor = Model.TextColor.ToXnaColor();
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Microsoft.Xna.Framework.Rectangle area)
        {
            spriteBatch.DrawString(_font, Model.Text, Translation, TextColor);
        }
    }
}
