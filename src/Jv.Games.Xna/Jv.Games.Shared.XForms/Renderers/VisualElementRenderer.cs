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
        Microsoft.Xna.Framework.Rectangle _originalClipArea;
        public bool Clip;
        #endregion

        #region Properties
        public new VisualElement Model { get { return (VisualElement)base.Model; } }
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

            RasterizerState state= null;
            if(Clip)
            {
#if IOS || ANDROID
                Game.GraphicsDevice.RasterizerState.ScissorTestEnable = Clip;
#endif
                state = new RasterizerState { ScissorTestEnable = Clip };
                _originalClipArea = spriteBatch.GraphicsDevice.ScissorRectangle;
                spriteBatch.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle((int)RenderArea.X, (int)RenderArea.Y, (int)RenderArea.Width, (int)RenderArea.Height);
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
