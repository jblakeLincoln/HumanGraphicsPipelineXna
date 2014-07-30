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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        SpriteBatch spriteBatch;       

        Button buttonHalfSpace;
        Button buttonBarycentric;
        Button buttonTriangleFilling;

        Button buttonClipping;

        Scene scene;
        enum MenuState
        { 
            Main,
            TriangleFilling,
            Clipping,
            None,
        }
        MenuState menuState = MenuState.Main;

        public Game1()
        {
            Globals.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            Globals.Init();

            //CreateForm();
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Fonts.font14 = Content.Load<SpriteFont>("Font14");
            Fonts.smallFont = Content.Load<SpriteFont>("SmallFont");
            Fonts.arial14 = Content.Load<SpriteFont>("Arial14");

            SetButtons();
        }

        public void SetButtons()
        {
            buttonTriangleFilling = new Button("Triangle\n filling", Fonts.font14, new Vector2(150, 50), new Vector2(Globals.viewportWidth / 2 - 200, Globals.viewportHeight / 2 - 25), Color.Red);
            buttonClipping = new Button("Clipping", Fonts.font14, new Vector2(150, 50), new Vector2(Globals.viewportWidth / 2 + 50, Globals.viewportHeight / 2 - 25), Color.Red);

            buttonHalfSpace = new Button("Half-space", Fonts.font14, new Vector2(150, 50), new Vector2(Globals.viewportWidth/2-200, Globals.viewportHeight/2-25), Color.Red);
            buttonBarycentric = new Button("Barycentric", Fonts.font14, new Vector2(150, 50), new Vector2(Globals.viewportWidth/2+50, Globals.viewportHeight/2-25), Color.Red);

            

            buttonTriangleFilling.OnClick += (b) => menuState = MenuState.TriangleFilling;
            buttonHalfSpace.OnClick += (b) => { menuState = MenuState.None; scene = new HalfSpace(); scene.BackToMenu += BackToMainMenu; };
            buttonBarycentric.OnClick += (b) => { menuState = MenuState.None; scene = new Barycentric(); scene.BackToMenu += BackToMainMenu; };
        
        
            buttonClipping.OnClick += (b) => { menuState = MenuState.None; scene = new TriangleClippingSH(); scene.BackToMenu += BackToMainMenu; };
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            Inputs.Update(gameTime);

            if (scene != null)
                scene.Update(gameTime);

            switch (menuState)
            {
                case MenuState.Main:
                    buttonTriangleFilling.Update(gameTime);
                    buttonClipping.Update(gameTime);
                    break;
                case MenuState.TriangleFilling:
                    buttonHalfSpace.Update(gameTime);
                    buttonBarycentric.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SlateGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                switch(menuState)
                {
                    case MenuState.Main:
                        DrawMainMenu(spriteBatch);
                        break;
                    case MenuState.TriangleFilling:
                        DrawTriangleFillingMenu(spriteBatch);
                        break;
                }

                if (scene != null)
                    scene.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Menu drawing
        private void DrawMainMenu(SpriteBatch spriteBatch)
        {
            buttonTriangleFilling.Draw(spriteBatch);
            buttonClipping.Draw(spriteBatch);
        }

        private void DrawTriangleFillingMenu(SpriteBatch spriteBatch)
        {
            buttonHalfSpace.Draw(spriteBatch);
            buttonBarycentric.Draw(spriteBatch);
        }

        private void BackToMainMenu()
        {
            scene = null;
            menuState = MenuState.Main;
        }

        // Added to scene object when TriangleScene is created.
        private void BackToTriangleMenu()
        {
            scene = null;
            menuState = MenuState.TriangleFilling;
        }
        #endregion


        

        private void CreateForm()
        {
            /*
            WF.Control form = WF.Control.FromHandle(Window.Handle);
            

            WF.Button b = new WF.Button();
            b.Text = "Halfspace";
            b.FlatStyle = WF.FlatStyle.Standard;
            b.Click += (sender, args) => { menuState = MenuState.None; scene = new HalfSpace(); scene.BackToMenu += BackToTriangleMenu; };
            
            if (form != null)
                form.Controls.Add(Globals.panel);
            Globals.panel.Controls.Add(b);
             * */
        }
    }
}
