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
        Vector2 _transformationMatrixLastSize;
        protected Vector2 Translation;
        protected Matrix TransformationMatrix = Matrix.Identity;

        public VisualElementRenderer()
        {
            HandleProperty(VisualElement.TranslationXProperty, HandleTranslation);
            HandleProperty(VisualElement.TranslationYProperty, HandleTranslation);
            HandleProperty(VisualElement.RotationYProperty, HandleTransformation);
            HandleProperty(VisualElement.RotationXProperty, HandleTransformation);
            HandleProperty(VisualElement.RotationProperty, HandleTransformation);
            HandleProperty(VisualElement.AnchorXProperty, HandleTransformation);
            HandleProperty(VisualElement.AnchorYProperty, HandleTransformation);
        }

        protected virtual bool HandleTransformation(BindableProperty prop)
        {
            _validTransformationMatrix = false;
            return true;
        }

        protected virtual bool HandleTranslation(BindableProperty prop)
        {
            Translation = new Vector2((float)Model.TranslationX, (float)Model.TranslationY);
            return true;
        }

        protected override void BeginDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Rectangle area)
        {
            if (!_validTransformationMatrix || _transformationMatrixLastSize != DesiredSize)
            {
                TransformationMatrix = Matrix.Identity;
                // TODO: Change projection to perspective

                var absAnchorX = Model.AnchorX * DesiredSize.X;
                var absAnchorY = Model.AnchorY * DesiredSize.Y;

                TransformationMatrix *= Matrix.CreateTranslation(new Vector3((float)-absAnchorX, (float)-absAnchorY, 0));
                TransformationMatrix *= Matrix.CreateRotationZ(MathHelper.ToRadians((float)Model.Rotation));
                TransformationMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians((float)Model.RotationY));
                TransformationMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians((float)Model.RotationX));
                TransformationMatrix *= Matrix.CreateTranslation(new Vector3((float)absAnchorX, (float)absAnchorY, 0));
                TransformationMatrix *= Matrix.CreateScale(1, 1, 0);

                _transformationMatrixLastSize = DesiredSize;
                _validTransformationMatrix = true;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, TransformationMatrix);
        }
    }
}
