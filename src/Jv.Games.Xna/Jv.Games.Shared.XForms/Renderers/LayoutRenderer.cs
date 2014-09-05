[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Layout),
    typeof(Jv.Games.Xna.XForms.Renderers.LayoutRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.Linq;
    using Xamarin.Forms;

    public class LayoutRenderer : LayoutRenderer<Layout>
    {
    }

    public class LayoutRenderer<TModel> : ViewRenderer<TModel>
        where TModel : Layout
    {
        protected Dictionary<Element, IControlRenderer> ChildrenRenderers;

        public LayoutRenderer()
        {
            ChildrenRenderers = new Dictionary<Element, IControlRenderer>();
        }

        public override void Initialize(Game game)
        {
            foreach (var c in ChildrenRenderers.Values)
                c.Initialize(game);
            base.Initialize(game);
        }

        protected override void LoadModel(TModel model)
        {
            foreach (var c in Model.LogicalChildren)
                Model_ChildAdded(this, new ElementEventArgs(c));
            Model.ChildAdded += Model_ChildAdded;
            Model.ChildRemoved += Model_ChildRemoved;
            base.LoadModel(model);
        }

        protected override void UnloadModel(TModel model)
        {
            ChildrenRenderers.Clear();
            Model.ChildAdded -= Model_ChildAdded;
            Model.ChildRemoved -= Model_ChildRemoved;
            base.UnloadModel(model);
        }

        void Model_ChildRemoved(object sender, ElementEventArgs e)
        {
            ChildrenRenderers.Remove(e.Element);
        }

        void Model_ChildAdded(object sender, ElementEventArgs e)
        {
            ChildrenRenderers[e.Element] = RendererFactory.Create(e.Element);
            if (Game != null)
                ChildrenRenderers[e.Element].Initialize(Game);
        }

        protected override void BeginDraw(SpriteBatch spriteBatch)
        {
        }

        protected override void EndDraw(SpriteBatch spriteBatch)
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var it in ChildrenRenderers)
                it.Value.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var it in ChildrenRenderers)
                it.Value.Draw(spriteBatch, gameTime);
        }
    }
}