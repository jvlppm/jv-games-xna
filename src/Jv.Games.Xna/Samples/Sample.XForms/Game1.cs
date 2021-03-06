using Jv.Games.Xna.XForms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Sample.XForms
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Xamarin.Forms.VisualElement _ui;
        Xamarin.Forms.Image _img;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Forms.Init(this);

            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            Jv.Games.Xna.XForms.Renderers.LabelRenderer.DefaultFont = Content.Load<SpriteFont>("DefaultFont");
            _ui = new Xamarin.Forms.ContentPage
            {
                Content = new Xamarin.Forms.StackLayout
                {
                    Orientation = Xamarin.Forms.StackOrientation.Vertical,
                    VerticalOptions = Xamarin.Forms.LayoutOptions.FillAndExpand,
                    HorizontalOptions = Xamarin.Forms.LayoutOptions.FillAndExpand,

                    Children =
                    {
                        new Xamarin.Forms.Label
                        {
                            HorizontalOptions = Xamarin.Forms.LayoutOptions.Center,
                            VerticalOptions = Xamarin.Forms.LayoutOptions.Center,
                            YAlign = Xamarin.Forms.TextAlignment.Center,
                            Text = "Title",
                        },
                        (_img = new Xamarin.Forms.Image
                        {
                            HorizontalOptions = Xamarin.Forms.LayoutOptions.CenterAndExpand,
                            VerticalOptions = Xamarin.Forms.LayoutOptions.CenterAndExpand,
                            Source = "TestImage"
                        })
                    }
                }
            };
            Components.Add(_ui.AsGameComponent());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float diffSpeed = 1f;

            var rend = (Jv.Games.Xna.XForms.Renderers.VisualElementRenderer)Jv.Games.Xna.XForms.Renderers.VisualElementRenderer.GetRenderer(_img);
            var m = Mouse.GetState();
            rend.CheckClick(new Vector2(m.X, m.Y));

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                _ui.RotationY = (_ui.RotationY + diffSpeed) % 360;
            else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                _ui.RotationY = (_ui.RotationY - diffSpeed) % 360;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                _ui.RotationX = (_ui.RotationX + diffSpeed) % 360;
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                _ui.RotationX = (_ui.RotationX - diffSpeed) % 360;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
