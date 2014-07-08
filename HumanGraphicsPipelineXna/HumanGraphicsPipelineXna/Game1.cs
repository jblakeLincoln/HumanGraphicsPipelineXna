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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        SpriteFont smallFont;
        MouseState mouseState;
        MouseState lastMouseState;

        Button buttonHalfSpace;
        Button buttonBarycentric;
        Button buttonTriangleFilling;

        enum MenuState
        { 
            Main,
            TriangleFilling,
            None,
        }

        enum ScreenState
        { 
            HalfSpace,
            Barycentric,
            None,
        }

        enum HalfSpaceState
        { 
            PickPoint1,
            PickPoint2,
            PickPoint3,
            Animate,
            None,
        }

        MenuState menuState = MenuState.Main;
        ScreenState screenState = ScreenState.None;
        HalfSpaceState halfSpaceState = HalfSpaceState.None;

        Vector2[] trianglePoints = new Vector2[3];
        Vector2[] normalisedTrianglePoints = new Vector2[3];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            mouseState = Mouse.GetState();
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

            font = Content.Load<SpriteFont>("SpriteFont1");
            smallFont = Content.Load<SpriteFont>("SmallFont");
            buttonTriangleFilling = new Button(graphics, "Triangle filling", font, new Vector2(150, 50), new Vector2(10, 10), Color.Red);
            buttonHalfSpace = new Button(graphics, "Half-space", font, new Vector2(150, 50), new Vector2(180, 10), Color.Red);
            buttonBarycentric = new Button(graphics, "Barycentric", font, new Vector2(150, 50), new Vector2(340, 10), Color.Red);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            lastMouseState = mouseState;
            mouseState = Mouse.GetState();


            if (menuState == MenuState.Main)
            {
                if (buttonTriangleFilling.IsClicked(mouseState, lastMouseState))
                    menuState = MenuState.TriangleFilling;
            }
            else if (menuState == MenuState.TriangleFilling)
            {
                if (buttonHalfSpace.IsClicked(mouseState, lastMouseState))
                {
                    menuState = MenuState.None;
                    screenState = ScreenState.HalfSpace;
                    halfSpaceState = HalfSpaceState.PickPoint1;
                    Console.WriteLine("Choose first triangle point.");
                }
                else if (buttonBarycentric.IsClicked(mouseState, lastMouseState))
                {
                    menuState = MenuState.None;
                    screenState = ScreenState.Barycentric;
                }
 
            }
            else if (screenState == ScreenState.HalfSpace)
            {
                if (halfSpaceState == HalfSpaceState.PickPoint1)
                {
                    if (mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
                    {
                        trianglePoints[0] = new Vector2(mouseState.X, mouseState.Y);
                        halfSpaceState = HalfSpaceState.PickPoint2;
                    }
                }
                else if (halfSpaceState == HalfSpaceState.PickPoint2)
                {
                    if (mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
                    {
                        trianglePoints[1] = new Vector2(mouseState.X, mouseState.Y);
                        halfSpaceState = HalfSpaceState.PickPoint3;
                    }
                }
                else if (halfSpaceState == HalfSpaceState.PickPoint3)
                {
                    if (mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
                    {
                        trianglePoints[2] = new Vector2(mouseState.X, mouseState.Y);
                        halfSpaceState = HalfSpaceState.Animate;
                    }
                }
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            if (menuState != MenuState.None)
            {
                if (menuState == MenuState.Main)
                    DrawMainMenu(spriteBatch);
                else if (menuState == MenuState.TriangleFilling)
                    DrawTriangleFillingMenu(spriteBatch);
            }
            else if (screenState != ScreenState.None)
            {
                if (screenState == ScreenState.HalfSpace)
                    DrawHalfSpace(spriteBatch);
            }
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

        private void DrawHalfSpace(SpriteBatch spriteBatch)
        {
            DrawGrid(spriteBatch);

            Vector2 normalisedScreen = Vector2.Normalize(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));

            for (int i = 0; i < 3; i++)
            {
                if (trianglePoints[i] != Vector2.Zero)
                {
                    DrawSquare(spriteBatch, trianglePoints[i], new Vector2(10, 10), Color.Green);
                    float normalisedX = (trianglePoints[i].X - 0) / (400 - 0) - 0.5f * 2;
                    float normalisedY = (trianglePoints[i].Y - 0) / (240 - 0) - 0.5f * 2;

                    normalisedTrianglePoints[i] = new Vector2(normalisedX, normalisedY);
                    //spriteBatch.DrawString(smallFont, trianglePoint1.X.ToString() + ", " + trianglePoint1.Y.ToString(), new Vector2(trianglePoint1.X-10, trianglePoint1.Y - 15), Color.White);
                    spriteBatch.DrawString(smallFont, normalisedTrianglePoints[i].X.ToString() + ", " + normalisedTrianglePoints[i].Y.ToString(), new Vector2(trianglePoints[i].X - 10, trianglePoints[i].Y - 15), Color.White);
                }
            }
            if (trianglePoints[2] != Vector2.Zero)
            {
                DrawLineBetweenTwoPoints(spriteBatch, trianglePoints[0], trianglePoints[1], new Vector2(5, 5), Color.Black);
                DrawLineBetweenTwoPoints(spriteBatch, trianglePoints[1], trianglePoints[2], new Vector2(5, 5), Color.Black);
                DrawLineBetweenTwoPoints(spriteBatch, trianglePoints[0], trianglePoints[2], new Vector2(5, 5), Color.Black);

                int minX = Math.Min((int)trianglePoints[0].X, Math.Min((int)trianglePoints[1].X, (int)trianglePoints[2].X));
                int minY = Math.Min((int)trianglePoints[0].Y, Math.Min((int)trianglePoints[1].Y, (int)trianglePoints[2].Y));
                int maxX = Math.Max((int)trianglePoints[0].X, Math.Max((int)trianglePoints[1].X, (int)trianglePoints[2].X));
                int maxY = Math.Max((int)trianglePoints[0].Y, Math.Max((int)trianglePoints[1].Y, (int)trianglePoints[2].Y));

                DrawSquare(spriteBatch, new Vector2(minX+5, minY+5), new Vector2(maxX-minX, maxY-minY), new Color(255, 0, 0, 50));
            }
        }
        
        private void DrawLineBetweenTwoPoints(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2, Vector2 offset, Color col,float thickness=1f)
        {
            Texture2D pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            Vector2 direction = p2 - p1;
            float length = direction.Length();
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            spriteBatch.Draw(pixel, new Vector2(p1.X+offset.X, p1.Y+offset.Y), null, col, angle, new Vector2(0.0f, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 1.0f);
        }

        private void DrawSquare(SpriteBatch spriteBatch, Vector2 pos, Vector2 size, Color col)
        {
            Texture2D square = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] pixels = new Color[1];
            for (int i = 0; i < 1; i++)
                pixels[i] = col;
            square.SetData<Color>(pixels);

            spriteBatch.Draw(square, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), Color.White);

        }

        private void DrawBarycentric(SpriteBatch spriteBatch)
        {
            
        }

        private void DrawGrid(SpriteBatch spriteBatch)
        { 
            Texture2D gridLine = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] pixels = new Color[1];
            for (int i = 0; i < 1; i++)
                pixels[i] = new Color(0, 0, 0, 100);

            gridLine.SetData<Color>(pixels);

            for (int i = 0; i <= 24; i++)
            {
                spriteBatch.Draw(gridLine, new Rectangle(0, i*(GraphicsDevice.Viewport.Height/24), GraphicsDevice.Viewport.Width, 1), Color.White);
                //DrawLineBetweenTwoPoints(spriteBatch, new Vector2(0, i * (GraphicsDevice.Viewport.Height / 24)), new Vector2(GraphicsDevice.Viewport.Width, i * (GraphicsDevice.Viewport.Height / 24)), Vector2.Zero,Color.Black);
            }

            for (int i = 0; i <= 40; i++)
            {
                spriteBatch.Draw(gridLine, new Rectangle(i*(GraphicsDevice.Viewport.Width/40), 0, 1, GraphicsDevice.Viewport.Height), Color.White);
                //DrawLineBetweenTwoPoints(spriteBatch, new Vector2(i * (GraphicsDevice.Viewport.Height / 24), 0), new Vector2(i * (GraphicsDevice.Viewport.Height / 24), GraphicsDevice.Viewport.Height), Vector2.Zero, Color.Black);
            }

            gridLine = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixels[0] = new Color(0, 0, 0, 255);

            spriteBatch.Draw(gridLine, new Rectangle(GraphicsDevice.Viewport.Width/2-2, 0, 4, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(gridLine, new Rectangle(0, GraphicsDevice.Viewport.Height / 2 - 2, GraphicsDevice.Viewport.Width, 4), Color.White);

            gridLine.SetData<Color>(pixels);
        }
    }
}
