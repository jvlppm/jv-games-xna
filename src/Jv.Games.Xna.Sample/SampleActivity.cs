using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Jv.Games.Xna.Sample
{
    class SampleActivity : Activity
    {
        Reference<Color> _color = Color.CornflowerBlue;

        public SampleActivity(Game game)
            : base(game)
        {
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(_color);

            // TODO: Add your drawing code here
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
        }
    }
}
