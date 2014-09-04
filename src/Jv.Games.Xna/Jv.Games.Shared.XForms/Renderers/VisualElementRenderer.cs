[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.VisualElement),
    typeof(Jv.Games.Xna.XForms.Renderers.VisualElementRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;

    public class VisualElementRenderer : VisualElementRenderer<VisualElement>
    {

    }

    public class VisualElementRenderer<TModel> : ElementRenderer<TModel>, IControlRenderer
        where TModel : VisualElement
    {
        bool _validTransformationMatrix;
        Xamarin.Forms.Rectangle _transformationMatrixLastArea;
        BasicEffect _basicEffect;

        public VisualElementRenderer()
        {
            HandleProperty(VisualElement.IsVisibleProperty, HandleMeasureChange);
            HandleProperty(VisualElement.TranslationXProperty, HandleTranslationChange);
            HandleProperty(VisualElement.TranslationYProperty, HandleTranslationChange);
            HandleProperty(VisualElement.RotationYProperty, HandleTransformationChange);
            HandleProperty(VisualElement.RotationXProperty, HandleTransformationChange);
            HandleProperty(VisualElement.RotationProperty, HandleTransformationChange);
            HandleProperty(VisualElement.ScaleProperty, HandleTransformationChange);
            HandleProperty(VisualElement.AnchorXProperty, HandleTransformationChange);
            HandleProperty(VisualElement.AnchorYProperty, HandleTransformationChange);
            HandleProperty(VisualElement.WidthProperty, HandleMeasureChange);
            HandleProperty(VisualElement.HeightProperty, HandleMeasureChange);
        }

        public override void Initialize(Game game)
        {
            _basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };
            base.Initialize(game);
        }

        protected virtual bool HandleTransformationChange(BindableProperty prop)
        {
            _validTransformationMatrix = false;
            return true;
        }

        protected virtual bool HandleTranslationChange(BindableProperty prop)
        {
            InvalidateArrange();
            return true;
        }

        protected virtual bool HandleMeasureChange(BindableProperty arg)
        {
            InvalidateMeasure();
            return true;
        }

        protected override void InvalidateArrange()
        {
            _validTransformationMatrix = false;
            base.InvalidateArrange();
        }

        void IControlRenderer.Measure(Size availableSize)
        {
            if (!Model.IsVisible)
                return;
            Measure(availableSize);
        }

        void IControlRenderer.Arrange(Xamarin.Forms.Rectangle finalRect)
        {
            if (!Model.IsVisible)
                return;

            Arrange(finalRect);
        }

        void IControlRenderer.Update(GameTime gameTime)
        {
            if (!Model.IsVisible)
                return;
            Update(gameTime);
        }

        void IControlRenderer.Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Model.IsVisible)
                return;

            BeginDraw(spriteBatch);
            Draw(spriteBatch, gameTime);
            EndDraw(spriteBatch);
        }

        void UpdateTransformationMatrix()
        {
            var viewport = Game.GraphicsDevice.Viewport;

            var absAnchorX = (float)(RenderArea.Width * Model.AnchorX);
            var absAnchorY = (float)(RenderArea.Height * Model.AnchorY);

            _basicEffect.World = Matrix.CreateTranslation(-absAnchorX, -absAnchorY, 0f)
                               * Matrix.CreateRotationX(MathHelper.ToRadians((float)Model.RotationX))
                               * Matrix.CreateRotationY(MathHelper.ToRadians((float)Model.RotationY))
                               * Matrix.CreateRotationZ(MathHelper.ToRadians((float)Model.Rotation))
                               * Matrix.CreateScale((float)Model.Scale)
                               * Matrix.CreateTranslation(absAnchorX, absAnchorY, 0f)
                               * Matrix.CreateScale(1, -1, 1);

            const float dist = 160;
            var angle = (float)System.Math.Atan(((float)RenderArea.Height / 2) / dist) * 2;

            _basicEffect.View = Matrix.CreateTranslation(-(float)RenderArea.Width / 2, (float)RenderArea.Height / 2, -dist);
            _basicEffect.Projection
                = Matrix.CreatePerspectiveFieldOfView(angle, (float)(RenderArea.Width / RenderArea.Height), 0.001f, dist + MathHelper.Max((float)RenderArea.Width, (float)RenderArea.Height))
                * Matrix.CreateScale((float)RenderArea.Width / viewport.Width, (float)RenderArea.Height / viewport.Height, 1)
                * Matrix.CreateTranslation(-1, 1, 0)
                * Matrix.CreateTranslation((float)(RenderArea.Width / 2 + RenderArea.X + Model.TranslationX - 0.5f) * 2 / viewport.Width, -(float)(RenderArea.Height / 2 + RenderArea.Y + Model.TranslationY - 0.5f) * 2 / viewport.Height, 0);

            _transformationMatrixLastArea = RenderArea;
            _validTransformationMatrix = true;
        }

        protected override void BeginDraw(SpriteBatch spriteBatch)
        {
            if (!_validTransformationMatrix || _transformationMatrixLastArea != RenderArea)
                UpdateTransformationMatrix();

            spriteBatch.Begin(0, null, null, DepthStencilState.None, RasterizerState.CullNone, _basicEffect);
        }
    }
}
