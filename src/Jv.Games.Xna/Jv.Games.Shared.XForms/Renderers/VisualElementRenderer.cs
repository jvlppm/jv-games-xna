[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.VisualElement),
    typeof(Jv.Games.Xna.XForms.Renderers.VisualElementRenderer<Xamarin.Forms.VisualElement>))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using System;
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
        // TODO: track children.

        #region Attributes
        Rectangle _transformationBounds;
        Matrix TransformationMatrix;
        VisualElement _model;

        protected readonly PropertyTracker PropertyTracker;
        protected readonly SpriteBatch SpriteBatch;
        #endregion

        #region Properties
        public VisualElement Model
        {
            get { return _model; }
            set
            {
                _model = value;
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
        }
        #endregion

        #region IRenderer
        public SizeRequest Measure(Size availableSize)
        {
            return default(SizeRequest);
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Model.Bounds != _transformationBounds)
                UpdateTransformationMatrix();
            LocalDraw(gameTime);

            // TODO: children.Draw(gameTime)
        }

        protected virtual void LocalDraw(Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        public virtual void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // TODO: children.Update(gameTime)
        }
        #endregion

        #region 3D Transformations
        void UpdateTransformationMatrix()
        {
            TransformationMatrix = GetWorldTransformation(Model) * GetProjectionMatrix();
            //RenderArea = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)ArrangedArea.Width, (int)ArrangedArea.Height);
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
    }
}
