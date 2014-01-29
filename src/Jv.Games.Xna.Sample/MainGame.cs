using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Sample
{
    class MainGame : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        SpriteBatch _spriteBatch;

        public MainGame()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600
            };
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize();

            this.Play(async activity =>
            {
                await activity.Run(new SampleActivity(this));
            });
        }
    }
}
