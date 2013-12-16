using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jv.Games.Xna.Async
{
    sealed class AsyncGameComponent : DrawableGameComponent
    {
        public readonly DrawableGameComponent Component;
        public readonly AsyncContext DrawContext, UpdateContext;

        public AsyncGameComponent(Game game, DrawableGameComponent component)
            : base(game)
        {
            Component = component;
            DrawContext = new AsyncContext();
            UpdateContext = new AsyncContext();
        }

        public override void Draw(GameTime gameTime)
        {
            DrawContext.Post(Component.Draw);
            DrawContext.Update(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateContext.Post(Component.Update);
            UpdateContext.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
                Component.Dispose();
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            Component.Initialize();
            base.Initialize();
        }
    }
}
