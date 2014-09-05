[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.VisualElement),
    typeof(Jv.Games.Xna.XForms.Renderers.VisualElementRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public interface I3DRenderer
    {
        Matrix CreateWorldMatrix();
        Matrix CreateViewMatrix();
    }

    public class VisualElementRenderer : VisualElementRenderer<VisualElement>
    {

    }

    public class VisualElementRenderer<TModel> : ElementRenderer<TModel>, IControlRenderer, I3DRenderer
        where TModel : VisualElement
    {
        bool _validTransformationMatrix;
        Matrix _transformationMatrix;
        Xamarin.Forms.Rectangle _transformationMatrixLastArea;

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

        public override void InvalidateArrange()
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
            _transformationMatrixLastArea = RenderArea;
            _transformationMatrix = ComputeTransformationMatrix();
            _validTransformationMatrix = true;
        }

        Matrix ComputeTransformationMatrix()
        {
            Stack<Matrix> parentMatrices = new Stack<Matrix>();
            var parent = Parent;
            while (parent != null)
            {
                var visualParent = parent as I3DRenderer;
                if (visualParent != null)
                    parentMatrices.Push(visualParent.CreateWorldMatrix());

                parent = parent.Parent;
            }

            Matrix baseTransformations = Matrix.Identity;
            while (parentMatrices.Count > 0)
                baseTransformations = baseTransformations * parentMatrices.Pop();

            var absAnchorX = (float)(RenderArea.Width * Model.AnchorX);
            var absAnchorY = (float)(RenderArea.Height * Model.AnchorY);

            // Aplicando transformações do Model
            return Matrix.CreateTranslation(-absAnchorX, -absAnchorY, 0f)
                 * baseTransformations * CreateWorldMatrix()
                 * Matrix.CreateTranslation(absAnchorX, absAnchorY, 0f)

                 * CreateViewMatrix();
        }

        public Matrix CreateWorldMatrix()
        {
            return Matrix.CreateRotationX(MathHelper.ToRadians((float)Model.RotationX))
                 * Matrix.CreateRotationY(MathHelper.ToRadians((float)Model.RotationY))
                 * Matrix.CreateRotationZ(MathHelper.ToRadians((float)Model.Rotation))
                 * Matrix.CreateScale((float)Model.Scale);
        }

        public Matrix CreateViewMatrix()
        {
            var viewport = Game.GraphicsDevice.Viewport;

            const float dist = 160;
            var angle = (float)System.Math.Atan(((float)RenderArea.Height / 2) / dist) * 2;

            return Matrix.CreateTranslation(-(float)RenderArea.Width / 2, -(float)RenderArea.Height / 2, -dist)
                 * Matrix.CreatePerspectiveFieldOfView(angle, (float)(RenderArea.Width / RenderArea.Height), 0.001f, dist + MathHelper.Max((float)RenderArea.Width, (float)RenderArea.Height))
                 * Matrix.CreateTranslation(1, 1, 0)
                 * Matrix.CreateScale(viewport.Width / 2, viewport.Height / 2, 0)
                 * Matrix.CreateScale((float)RenderArea.Width / viewport.Width, (float)RenderArea.Height / viewport.Height, 1)
                 * Matrix.CreateTranslation((float)Model.TranslationX, (float)Model.TranslationY, 0)
                 * Matrix.CreateTranslation(new Vector3((float)RenderArea.X, (float)RenderArea.Y, 0));
        }

        protected override void BeginDraw(SpriteBatch spriteBatch)
        {
            if (!_validTransformationMatrix || _transformationMatrixLastArea != RenderArea)
                UpdateTransformationMatrix();

            spriteBatch.Begin(0, null, null, DepthStencilState.None, RasterizerState.CullNone, null, _transformationMatrix);
        }
    }
}
