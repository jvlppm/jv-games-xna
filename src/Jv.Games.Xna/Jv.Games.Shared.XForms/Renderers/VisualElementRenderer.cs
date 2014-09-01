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
        protected Vector2 Translation;
        protected Matrix TransformationMatrix = Matrix.Identity;

        public VisualElementRenderer()
        {
            HandleProperty(VisualElement.TranslationXProperty, HandleTranslation);
            HandleProperty(VisualElement.TranslationYProperty, HandleTranslation);
            HandleProperty(VisualElement.RotationYProperty, HandleTransformation);
            HandleProperty(VisualElement.RotationXProperty, HandleTransformation);
            HandleProperty(VisualElement.RotationProperty, HandleTransformation);
        }

        protected virtual bool HandleTransformation(BindableProperty prop)
        {
            TransformationMatrix = Matrix.Identity;
            // TODO: Change projection to perspective
            // TODO: Translate Left, according to anchorY, then translate back
            //TransformationMatrix *= Matrix.CreateTranslation(new Vector3((float)Model.TranslationX, (float)Model.TranslationY, 0));
            TransformationMatrix *= Matrix.CreateRotationZ(MathHelper.ToRadians((float)Model.Rotation));
            TransformationMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians((float)Model.RotationY));
            TransformationMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians((float)Model.RotationX));
            TransformationMatrix *= Matrix.CreateScale(1, 1, 0);
            return true;
        }

        protected virtual bool HandleTranslation(BindableProperty prop)
        {
            Translation = new Vector2((float)Model.TranslationX, (float)Model.TranslationY);
            return true;
        }

        protected override void BeginDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, TransformationMatrix);
        }
    }
}
