[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.VisualElement),
    typeof(Jv.Games.Xna.XForms.Renderers.VisualElementRenderer<Xamarin.Forms.VisualElement>))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using Xamarin.Forms;
    using Vector2 = Microsoft.Xna.Framework.Vector2;
    using Vector3 = Microsoft.Xna.Framework.Vector3;
    using Matrix = Microsoft.Xna.Framework.Matrix;
    using MathHelper = Microsoft.Xna.Framework.MathHelper;
    using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

    public class VisualElementRenderer<TModel> : VisualElementRenderer
        where TModel : VisualElement
    {
        public new TModel Model
        {
            get { return (TModel)base.Model; }
            set { base.Model = value; }
        }
    }

    public class VisualElementRenderer : IVisualElementRenderer
    {
        #region Attributes
        Rectangle _transformationBounds;
        Matrix TransformationMatrix;
        VisualElement _model;
        Dictionary<Element, IRenderer> _childrenRenderers;

        protected readonly PropertyTracker PropertyTracker;
        protected readonly SpriteBatch SpriteBatch;
        #endregion

        #region Properties
        public VisualElement Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;

                if (_model != null)
                {
                    _model.ChildAdded -= Model_ChildAdded;
                    _model.ChildRemoved -= Model_ChildRemoved;
                }
                _model = value;

                if (_model != null)
                {
                    _childrenRenderers = value.LogicalChildren.ToDictionary(c => c, RendererFactory.Create);
                    _model.ChildAdded += Model_ChildAdded;
                    _model.ChildRemoved += Model_ChildRemoved;
                }
                else
                {
                    _childrenRenderers = null;
                }
                PropertyTracker.SetTarget(value);
            }
        }
        #endregion

        #region Constructors
        public VisualElementRenderer()
        {
            if (!Forms.IsInitialized)
                throw new InvalidOperationException("Xamarin.Forms not initialized");

            SpriteBatch = new SpriteBatch(Forms.Game.GraphicsDevice);
            PropertyTracker = new PropertyTracker();

            PropertyTracker.AddHandler(VisualElement.AnchorXProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.AnchorYProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationXProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationYProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.ScaleProperty, Handle_Transformation);
        }
        #endregion

        #region IRenderer
        public virtual SizeRequest Measure(Size availableSize)
        {
            return default(SizeRequest);
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Render(gameTime);

            if (_childrenRenderers != null)
            {
                foreach (var childRenderer in _childrenRenderers.Values)
                    childRenderer.Draw(gameTime);
            }
        }

        protected void Render(Microsoft.Xna.Framework.GameTime gameTime)
        {
            BeginDraw();
            LocalDraw(gameTime);
            EndDraw();
        }

        protected virtual void BeginDraw()
        {
            if (Model.Bounds != _transformationBounds)
                Arrange();

            var state = new Microsoft.Xna.Framework.Graphics.RasterizerState
            {
                CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None
            };
            /*if (Clip)
            {
#if IOS || ANDROID
                Game.GraphicsDevice.RasterizerState.ScissorTestEnable = Clip;
#endif
                state.ScissorTestEnable = Clip;
                _originalClipArea = spriteBatch.GraphicsDevice.ScissorRectangle;
                spriteBatch.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle((int)ArrangedArea.X, (int)ArrangedArea.Y, (int)ArrangedArea.Width, (int)ArrangedArea.Height);
            }*/

            SpriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Immediate, Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend, null, Microsoft.Xna.Framework.Graphics.DepthStencilState.None, state, null, TransformationMatrix);
        }

        protected virtual void LocalDraw(Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        protected virtual void EndDraw()
        {
            SpriteBatch.End();
            /*if (Clip)
            {
#if IOS || ANDROID
                Game.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
#endif
                spriteBatch.GraphicsDevice.ScissorRectangle = _originalClipArea;
            }*/
        }

        public virtual void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_childrenRenderers != null)
            {
                foreach (var childRenderer in _childrenRenderers.Values)
                    childRenderer.Update(gameTime);
            }
        }
        #endregion

        #region 3D Transformations
        void Handle_Transformation(BindableProperty prop)
        {
            InvalidateTransformations();
        }

        public void InvalidateTransformations()
        {
            _transformationBounds = default(Rectangle);
            foreach (var child in _childrenRenderers)
            {
                var visualRenderer = child.Value as IVisualElementRenderer;
                visualRenderer.InvalidateTransformations();
            }
        }

        protected virtual void Arrange()
        {
            TransformationMatrix = GetWorldTransformation(Model) * GetProjectionMatrix(Model);
            _transformationBounds = Model.Bounds;
        }

        static Matrix GetWorldTransformation(Element element)
        {
            Matrix world = Matrix.Identity;
            var currentElement = element;
            while (currentElement != null)
            {
                var currentVisual = currentElement as VisualElement;
                if (currentVisual != null)
                    world *= GetControlTransformation(currentVisual);

                currentElement = currentElement.Parent;
            }

            return world;
        }

        static Matrix GetProjectionMatrix(VisualElement element)
        {
            if (element.Bounds.Width <= 0 && element.Bounds.Height <= 0)
                return Matrix.Identity;

            float dist = (float)160;
            var angle = (float)System.Math.Atan(((float)element.Bounds.Height / 2) / dist) * 2;

            var centerX = element.Bounds.Left + element.Bounds.Width / 2;
            var centerY = element.Bounds.Top + element.Bounds.Height / 2;

            return Matrix.CreateTranslation(-(float)centerX, -(float)centerY, -dist)
                 * Matrix.CreatePerspectiveFieldOfView(angle, (float)(element.Bounds.Width / element.Bounds.Height), 0.001f, dist * 2)
                 * Matrix.CreateTranslation(1, 1, 0)
                 * Matrix.CreateScale((float)element.Bounds.Width / 2, (float)element.Bounds.Height / 2, 1)
                 * Matrix.CreateTranslation((float)element.Bounds.Left, (float)element.Bounds.Top, 0);
        }

        static Matrix GetControlTransformation(VisualElement element)
        {
            var absAnchorX = (float)(element.Bounds.Width * element.AnchorX);
            var absAnchorY = (float)(element.Bounds.Height * element.AnchorY);

            var offset = new Vector2(
                (float)(element.Bounds.X + element.TranslationX - (absAnchorX * element.Scale - absAnchorX)),
                (float)(element.Bounds.Y + element.TranslationY - (absAnchorY * element.Scale - absAnchorY))
            );

            return Matrix.CreateTranslation(-absAnchorX, -absAnchorY, 0f)
                 * Matrix.CreateRotationX(MathHelper.ToRadians((float)element.RotationX))
                 * Matrix.CreateRotationY(MathHelper.ToRadians((float)element.RotationY))
                 * Matrix.CreateRotationZ(MathHelper.ToRadians((float)element.Rotation))
                 * Matrix.CreateScale((float)element.Scale)
                 * Matrix.CreateTranslation(absAnchorX * (float)element.Scale, absAnchorY * (float)element.Scale, 0f)
                 * Matrix.CreateTranslation(new Vector3(offset, 0));
        }
        #endregion

        #region Child track
        void Model_ChildAdded(object sender, ElementEventArgs e)
        {
            _childrenRenderers.Add(e.Element, RendererFactory.Create(e.Element));
        }

        void Model_ChildRemoved(object sender, ElementEventArgs e)
        {
            _childrenRenderers.Remove(e.Element);
        }
        #endregion

        #region Protected Methods
        protected void InvalidateMeasure()
        {
            Model.NativeSizeChanged();
        }
        #endregion
    }
}
