using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Sample
{
    class MainGame : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }

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
            base.Initialize();

            this.Play(async activity =>
            {
                await activity.Run(new SampleActivity(this));
            });
        }
    }
}
