[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.VisualElement),
    typeof(Jv.Games.Xna.XForms.Renderers.VisualElementRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using Xamarin.Forms;
    using Rectangle = Xamarin.Forms.Rectangle;

    public class VisualElementRenderer : ElementRenderer, IControlRenderer
    {
        #region Attributes
        bool _validTransformationMatrix;
        Matrix TransformationMatrix;
        Rectangle _transformationMatrixLastArea;
        Microsoft.Xna.Framework.Rectangle _originalClipArea;
        public bool Clip;
        #endregion

        #region Properties
        public new VisualElement Model { get { return (VisualElement)base.Model; } }
        protected Microsoft.Xna.Framework.Rectangle RenderArea { get; private set; }
        protected Size ContentMeasuredSize { get; private set; }
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
            InvalidateTransform();
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
            if (!_validTransformationMatrix || _transformationMatrixLastArea != ArrangedArea)
                UpdateTransformationMatrix();

            RasterizerState state = new RasterizerState { CullMode = CullMode.None }; ;
            if (Clip)
            {
#if IOS || ANDROID
                Game.GraphicsDevice.RasterizerState.ScissorTestEnable = Clip;
#endif
                state.ScissorTestEnable = Clip;
                _originalClipArea = spriteBatch.GraphicsDevice.ScissorRectangle;
                spriteBatch.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle((int)ArrangedArea.X, (int)ArrangedArea.Y, (int)ArrangedArea.Width, (int)ArrangedArea.Height);
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.None, state, null, TransformationMatrix);
        }

        protected override void EndDraw(SpriteBatch spriteBatch)
        {
            base.EndDraw(spriteBatch);

            if (Clip)
            {
#if IOS || ANDROID
                Game.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
#endif
                spriteBatch.GraphicsDevice.ScissorRectangle = _originalClipArea;
            }
        }

        protected virtual Size MeasureContentOverride(Size availableSize)
        {
            return Size.Zero;
        }

        sealed protected override Size MeasureOverride(Size availableSize)
        {
            if (!Model.IsVisible)
                return MeasuredSize;

            var ignore = new Size(-1, -1);
            var minRequestSize = new Size(Model.MinimumWidthRequest, Model.MinimumHeightRequest);
            var requestSize = new Size(Model.WidthRequest, Model.HeightRequest);

            var constrainedAvailableSize = RespectSize(availableSize, minRequestSize, requestSize, availableSize);

            ContentMeasuredSize = MeasureContentOverride(constrainedAvailableSize);

            return RespectSize(ContentMeasuredSize, minRequestSize, requestSize, availableSize);
        }

        static Size RespectSize(Size original, Size minimum, Size specified, Size maximum)
        {
            original = new Size(
                specified.Width < 0 ? original.Width : specified.Width,
                specified.Height < 0 ? original.Height : specified.Height
            );

            original = new Size(
                minimum.Width < 0 ? original.Width : Math.Max(minimum.Width, original.Width),
                minimum.Height < 0 ? original.Height : Math.Max(minimum.Height, original.Height)
            );

            original = new Size(
                maximum.Width < 0 ? original.Width : Math.Min(maximum.Width, original.Width),
                maximum.Height < 0 ? original.Height : Math.Min(maximum.Height, original.Height)
            );

            return original;
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
        public virtual void InvalidateTransform()
        {
            _validTransformationMatrix = false;
        }

        Matrix GetWorldTransformation()
        {
            Matrix world = Matrix.Identity;
            var currentRenderer = (IControlRenderer)this;
            while (currentRenderer != null)
            {
                var currentVisual = currentRenderer as VisualElementRenderer;
                if (currentVisual != null)
                    world *= currentVisual.GetControlTransformation();

                currentRenderer = currentRenderer.Parent;
            }

            return world;
        }

        static Matrix GetProjectionMatrix()
        {
            var viewport = Forms.Game.GraphicsDevice.Viewport;

            float dist = (float)Math.Max(viewport.Width, viewport.Height);
            var angle = (float)System.Math.Atan(((float)viewport.Height / 2) / dist) * 2;

            return Matrix.CreateTranslation(-(float)viewport.Width / 2, -(float)viewport.Height / 2, -dist)
                 * Matrix.CreatePerspectiveFieldOfView(angle, ((float)viewport.Width / viewport.Height), 0.001f, dist * 2)
                 * Matrix.CreateTranslation(1, 1, 0)
                 * Matrix.CreateScale(viewport.Width / 2, viewport.Height / 2, 1);
        }

        public Matrix GetControlTransformation()
        {
            var absAnchorX = (float)(ArrangedArea.Width * Model.AnchorX);
            var absAnchorY = (float)(ArrangedArea.Height * Model.AnchorY);

            var offset = new Vector2(
                (float)(ArrangedArea.X + Model.TranslationX - (absAnchorX * Model.Scale - absAnchorX)),
                (float)(ArrangedArea.Y + Model.TranslationY - (absAnchorY * Model.Scale - absAnchorY))
            );

            return Matrix.CreateTranslation(-absAnchorX, -absAnchorY, 0f)
                 * Matrix.CreateRotationX(MathHelper.ToRadians((float)Model.RotationX))
                 * Matrix.CreateRotationY(MathHelper.ToRadians((float)Model.RotationY))
                 * Matrix.CreateRotationZ(MathHelper.ToRadians((float)Model.Rotation))
                 * Matrix.CreateScale((float)Model.Scale)
                 * Matrix.CreateTranslation(absAnchorX * (float)Model.Scale, absAnchorY * (float)Model.Scale, 0f)
                 * Matrix.CreateTranslation(new Vector3(offset, 0));
        }

        void UpdateTransformationMatrix()
        {
            _transformationMatrixLastArea = ArrangedArea;
            TransformationMatrix = GetWorldTransformation() * GetProjectionMatrix();
            RenderArea = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)ArrangedArea.Width, (int)ArrangedArea.Height);
            _validTransformationMatrix = true;
        }
        #endregion
    }
}
