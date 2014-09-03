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

    public class VisualElementRenderer<TModel> : ElementRenderer<TModel>
        where TModel : VisualElement
    {
        bool _validTransformationMatrix;
        Xamarin.Forms.Rectangle _transformationMatrixLastArea;
        protected Vector2 Translation;
        protected Matrix TransformationMatrix = Matrix.Identity;

        public VisualElementRenderer()
        {
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

        protected virtual bool HandleTransformationChange(BindableProperty prop)
        {
            _validTransformationMatrix = false;
            return true;
        }

        protected virtual bool HandleTranslationChange(BindableProperty prop)
        {
            Translation = new Vector2((float)Model.TranslationX, (float)Model.TranslationY);
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

        void UpdateTransformationMatrix()
        {
            TransformationMatrix = Matrix.Identity;
            // TODO: Change projection to perspective
            var absAnchorX = RenderArea.X + RenderArea.Width * Model.AnchorX;
            var absAnchorY = RenderArea.Y + RenderArea.Height * Model.AnchorY;
            TransformationMatrix *= Matrix.CreateTranslation(new Vector3((float)-absAnchorX, (float)-absAnchorY, 0));
            TransformationMatrix *= Matrix.CreateRotationZ(MathHelper.ToRadians((float)Model.Rotation));
            TransformationMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians((float)Model.RotationY));
            TransformationMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians((float)Model.RotationX));
            TransformationMatrix *= Matrix.CreateScale((float)Model.Scale, (float)Model.Scale, 0);
            TransformationMatrix *= Matrix.CreateTranslation(new Vector3((float)absAnchorX, (float)absAnchorY, 0));
            _transformationMatrixLastArea = RenderArea;
            _validTransformationMatrix = true;
        }

        protected override void BeginDraw(SpriteBatch spriteBatch)
        {
            if (!_validTransformationMatrix || _transformationMatrixLastArea != RenderArea)
                UpdateTransformationMatrix();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, TransformationMatrix);
        }
    }
}
