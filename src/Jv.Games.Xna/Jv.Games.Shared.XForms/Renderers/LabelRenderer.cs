[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Label),
    typeof(Jv.Games.Xna.XForms.Renderers.LabelRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;

    public class LabelRenderer : ViewRenderer
    {
        public new Label Model { get { return (Label)base.Model; } }

        public static SpriteFont DefaultFont;
        public static Microsoft.Xna.Framework.Color DefaultTextColor = Microsoft.Xna.Framework.Color.Black;

        ContentManager _content;
        SpriteFont _font;
        Microsoft.Xna.Framework.Color TextColor;

        Vector2 _textOffset;

        public override void Initialize(Game game)
        {
            _content = game.Content;
            HandleProperty(Label.FontProperty, HandleFont);
            HandleProperty(Label.TextColorProperty, HandleTextColor);
            HandleProperty(Label.TextProperty, MeasurePropertyChanged);
            HandleProperty(Label.XAlignProperty, HandleArrangeChange);
            HandleProperty(Label.YAlignProperty, HandleArrangeChange);
            base.Initialize(game);
        }

        protected virtual bool HandleFont(BindableProperty prop)
        {
            if (Model.Font.FontFamily != null)
                _font = _content.Load<SpriteFont>(Model.Font.FontFamily);
            else
                _font = null;
            InvalidateMeasure();
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

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_font == null)
                _font = LabelRenderer.DefaultFont;
            if (_font == null)
                return base.MeasureOverride(availableSize);
            var textMeasure = _font.MeasureString(Model.Text);

            return new Size(textMeasure.X, textMeasure.Y);
        }

        protected override Xamarin.Forms.Rectangle ArrangeOverride(Xamarin.Forms.Rectangle finalRect)
        {
            var renderArea = base.ArrangeOverride(finalRect);

            _textOffset = new Vector2(
                (float)GetAlignOffset(Model.XAlign, (float)MeasuredSize.Width, (float)renderArea.Width),
                (float)GetAlignOffset(Model.YAlign, (float)MeasuredSize.Height, (float)renderArea.Height));

            return renderArea;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(_font, Model.Text, _textOffset, TextColor, 0, Vector2.Zero, 1, 0, 0);
        }

        static float GetAlignOffset(TextAlignment alignment, float textSize, float renderSize)
        {
            switch (alignment)
            {
                case TextAlignment.Start:
                    return 0;
                case TextAlignment.Center:
                    return (renderSize - textSize) * 0.5f;
                case TextAlignment.End:
                    return (renderSize - textSize);
            }
            throw new System.NotImplementedException("Unsupported TextAlignment");
        }
    }
}
