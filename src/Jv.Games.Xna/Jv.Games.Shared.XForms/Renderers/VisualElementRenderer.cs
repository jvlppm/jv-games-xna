[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.VisualElement),
    typeof(Jv.Games.Xna.XForms.Renderers.VisualElementRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;
    using Rectangle=Xamarin.Forms.Rectangle;

    public interface I3DRenderer
    {
        Matrix GetControlTransformation();
        Matrix GetControlProjection();
        Matrix GetGUITransformation();
    }

    public class VisualElementRenderer : ElementRenderer, IControlRenderer, I3DRenderer
    {
        #region Attributes
        bool _validTransformationMatrix;
        public Matrix TransformationMatrix { get; private set; }
        Rectangle _transformationMatrixLastArea;
        #endregion

        #region Properties
        public new VisualElement Model { get { return (VisualElement)base.Model; } }
        #endregion

        #region Constructors
        public VisualElementRenderer()
        {
            HandleProperty(VisualElement.IsVisibleProperty, HandleMeasurePropertyChanged);
            HandleProperty(VisualElement.TranslationXProperty, HandleArrangePropertyChanged);
            HandleProperty(VisualElement.TranslationYProperty, HandleArrangePropertyChanged);
            HandleProperty(VisualElement.RotationYProperty, HandleTransformationChange);
            HandleProperty(VisualElement.RotationXProperty, HandleTransformationChange);
            HandleProperty(VisualElement.RotationProperty, HandleTransformationChange);
            HandleProperty(VisualElement.ScaleProperty, HandleTransformationChange);
            HandleProperty(VisualElement.AnchorXProperty, HandleTransformationChange);
            HandleProperty(VisualElement.AnchorYProperty, HandleTransformationChange);
            HandleProperty(VisualElement.WidthProperty, HandleMeasurePropertyChanged);
            HandleProperty(VisualElement.HeightProperty, HandleMeasurePropertyChanged);
        }
        #endregion

        #region Property Handlers
        protected virtual bool HandleTransformationChange(BindableProperty prop)
        {
            _validTransformationMatrix = false;
            return true;
        }

        protected virtual bool HandleArrangePropertyChanged(BindableProperty prop)
        {
            InvalidateArrange();
            return true;
        }

        protected virtual bool HandleMeasurePropertyChanged(BindableProperty prop)
        {
            InvalidateMeasure();
            return true;
        }
        #endregion

        #region Overrides
        public override void InvalidateArrange()
        {
            _validTransformationMatrix = false;
            base.InvalidateArrange();
        }

        protected override void BeginDraw(SpriteBatch spriteBatch)
        {
            if (!_validTransformationMatrix || _transformationMatrixLastArea != RenderArea)
                UpdateTransformationMatrix();

            spriteBatch.Begin(0, null, null, DepthStencilState.None, RasterizerState.CullNone, null, TransformationMatrix);
        }

        void IControlRenderer.Measure(Size availableSize)
        {
            if (!Model.IsVisible)
                return;
            Measure(availableSize);
        }

        void IControlRenderer.Arrange(Rectangle finalRect)
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
        #endregion

        #region I3DRenderer
        public Matrix GetControlTransformation()
        {
            return Matrix.CreateRotationX(MathHelper.ToRadians((float)Model.RotationX))
                 * Matrix.CreateRotationY(MathHelper.ToRadians((float)Model.RotationY))
                 * Matrix.CreateRotationZ(MathHelper.ToRadians((float)Model.Rotation))
                 * Matrix.CreateScale((float)Model.Scale);
        }

        public Matrix GetControlProjection()
        {
            var viewport = Game.GraphicsDevice.Viewport;

            const float dist = 160;
            var angle = (float)System.Math.Atan(((float)RenderArea.Height / 2) / dist) * 2;

            return Matrix.CreateTranslation(-(float)RenderArea.Width / 2, -(float)RenderArea.Height / 2, -dist)
                 * Matrix.CreatePerspectiveFieldOfView(angle, (float)(RenderArea.Width / RenderArea.Height), 0.001f, dist + MathHelper.Max((float)RenderArea.Width, (float)RenderArea.Height))
                 * Matrix.CreateTranslation(1, 1, 0)
                 * Matrix.CreateScale(viewport.Width / 2, viewport.Height / 2, 1)
                 * Matrix.CreateScale((float)RenderArea.Width / viewport.Width, (float)RenderArea.Height / viewport.Height, 1)
                 * Matrix.CreateTranslation(new Vector3((float)RenderArea.X, (float)RenderArea.Y, 0))
                 * Matrix.CreateTranslation((float)Model.TranslationX, (float)Model.TranslationY, 0);
        }

        public Matrix GetGUITransformation()
        {
            return Matrix.CreateTranslation(new Vector3((float)Model.TranslationX, (float)Model.TranslationY, 0));
        }

        void UpdateTransformationMatrix()
        {
            _transformationMatrixLastArea = RenderArea;
            TransformationMatrix = ComputeTransformationMatrix();
            _validTransformationMatrix = true;
        }

        Matrix ComputeTransformationMatrix()
        {
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.Identity;
            var parent = (IControlRenderer)this;
            while (parent != null)
            {
                var worldParent = parent as I3DRenderer;
                if (worldParent != null)
                {
                    world = worldParent.GetControlTransformation() * world;
                    view = worldParent.GetGUITransformation() * view;
                }

                parent = parent.Parent;
            }

            var absAnchorX = (float)(RenderArea.Width * Model.AnchorX);
            var absAnchorY = (float)(RenderArea.Height * Model.AnchorY);

            return Matrix.CreateTranslation(-absAnchorX, -absAnchorY, 0f)
                * world
                * Matrix.CreateTranslation(absAnchorX, absAnchorY, 0f)
                * GetControlProjection()
                * view;
        }
        #endregion
    }
}
