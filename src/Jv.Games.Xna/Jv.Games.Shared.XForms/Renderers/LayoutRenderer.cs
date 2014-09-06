[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Layout),
    typeof(Jv.Games.Xna.XForms.Renderers.LayoutRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.Linq;
    using Xamarin.Forms;

    public class LayoutRenderer : ViewRenderer
    {
        #region Properties
        public new Layout Model { get { return (Layout)base.Model; } }

        protected Dictionary<Element, IControlRenderer> ChildrenRenderers;

        public IEnumerable<IControlRenderer> Children
        {
            get
            {
                if (Model == null)
                    return Enumerable.Empty<IControlRenderer>();

                return from View c in Model.InternalChildren
                       select ChildrenRenderers[c];
            }
        }
        #endregion

        #region Constructors
        public LayoutRenderer()
        {
            ChildrenRenderers = new Dictionary<Element, IControlRenderer>();
        }
        #endregion

        #region Overrides
        public override void Initialize(Game game)
        {
            foreach (var c in ChildrenRenderers.Values)
                c.Initialize(game);
            base.Initialize(game);
        }

        protected override void LoadModel(Element model)
        {
            base.LoadModel(model);
            foreach (var c in Model.LogicalChildren)
                Model_ChildAdded(this, new ElementEventArgs(c));
            Model.ChildAdded += Model_ChildAdded;
            Model.ChildRemoved += Model_ChildRemoved;
        }

        protected override void UnloadModel(Element model)
        {
            ChildrenRenderers.Clear();
            Model.ChildAdded -= Model_ChildAdded;
            Model.ChildRemoved -= Model_ChildRemoved;
            base.UnloadModel(model);
        }

        protected override void BeginDraw(SpriteBatch spriteBatch)
        {
        }

        protected override void EndDraw(SpriteBatch spriteBatch)
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var it in ChildrenRenderers.Values)
                it.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var it in ChildrenRenderers.Values)
                it.Draw(spriteBatch, gameTime);
        }

        public override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
            foreach (var it in ChildrenRenderers.Values)
                it.InvalidateMeasure();
        }

        public override void InvalidateArrange()
        {
            base.InvalidateArrange();
            foreach (var it in ChildrenRenderers.Values)
                it.InvalidateArrange();
        }

        public override void InvalidateTransform()
        {
            base.InvalidateTransform();
            foreach (VisualElementRenderer it in ChildrenRenderers.Values)
                it.InvalidateTransform();
        }
        #endregion

        #region Private Methods
        void Model_ChildRemoved(object sender, ElementEventArgs e)
        {
            ChildrenRenderers.Remove((View)e.Element);
        }

        void Model_ChildAdded(object sender, ElementEventArgs e)
        {
            var renderer = (ViewRenderer)RendererFactory.Create(e.Element);
            renderer.Parent = this;
            ChildrenRenderers[(View)e.Element] = renderer;
            if (Game != null)
                renderer.Initialize(Game);
        }
        #endregion
    }
}