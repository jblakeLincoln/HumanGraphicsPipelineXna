using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HumanGraphicsPipelineXna
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        SpriteBatch spriteBatch;       

        Button buttonHalfSpace;
        Button buttonBarycentric;
        Button buttonTriangleFilling;

        Scene scene;

        enum MenuState
        { 
            Main,
            TriangleFilling,
            None,
        }

        MenuState menuState = MenuState.Main;

        public Game1()
        {
            Globals.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Globals.Init();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            IsMouseVisible = true;

           // Inputs.MouseState = Mouse.GetState();
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

            Fonts.font14 = Content.Load<SpriteFont>("Font14");
            Fonts.smallFont = Content.Load<SpriteFont>("SmallFont");
            
            buttonTriangleFilling = new Button("Triangle filling", Fonts.font14, new Vector2(150, 50), new Vector2(10, 10), Color.Red);
            buttonHalfSpace = new Button("Half-space", Fonts.font14, new Vector2(150, 50), new Vector2(180, 10), Color.Red);
            buttonBarycentric = new Button("Barycentric", Fonts.font14, new Vector2(150, 50), new Vector2(340, 10), Color.Red);

            // TODO: use this.Content to load your game content here
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
            Inputs.Update(gameTime);

            if (scene != null)
                scene.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (menuState == MenuState.Main)
            {
                if (buttonTriangleFilling.IsClicked())
                    menuState = MenuState.TriangleFilling;
            }
            else if (menuState == MenuState.TriangleFilling)
            {
                if (buttonHalfSpace.IsClicked())
                {
                    scene = new HalfSpace();
                    menuState = MenuState.None;
                }
                else if (buttonBarycentric.IsClicked())
                {
                    menuState = MenuState.None;
                }
 
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (menuState != MenuState.None)
            {
                if (menuState == MenuState.Main)
                    DrawMainMenu(spriteBatch);
                else if (menuState == MenuState.TriangleFilling)
                    DrawTriangleFillingMenu(spriteBatch);
            }

            if (scene != null)
                scene.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawMainMenu(SpriteBatch spriteBatch)
        {
            buttonTriangleFilling.Draw(spriteBatch);
        }

        private void DrawTriangleFillingMenu(SpriteBatch spriteBatch)
        {
            buttonHalfSpace.Draw(spriteBatch);
            buttonBarycentric.Draw(spriteBatch);
        }
    }
}
