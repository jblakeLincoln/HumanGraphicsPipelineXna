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
    abstract class Scene
    {
        protected Texture2D gridLine;
        protected Texture2D windowSpaceLine;

        protected bool animating = false;
        protected int animationCounter;
        protected int animationCounterLimit;

        protected Button buttonNext;
        protected Button buttonPrevious;
        protected Button buttonPlay;
        protected Button buttonReset;
        protected Button buttonBack;

        protected enum State { Animated = 42, };
        protected int state = 0;


        public delegate void ThisBackToMenu();
        public event ThisBackToMenu BackToMenu;

        protected abstract void DerivedInit();
        //protected abstract void ActionOnTriangleDraw(SpriteBatch spriteBatch);
        protected abstract void LastPointPlaced(GameTime gameTime);
        protected abstract void StateChanges(GameTime gameTime);

        public Scene()
        {
            Init();
            DerivedInit();
        }

        public void Init()
        {
            animationCounter = -1;
            animationCounterLimit = 0;

            // Set grid line colour.
            gridLine = new Texture2D(Globals.graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] pixels = new Color[1];
            for (int i = 0; i < 1; i++)
                pixels[i] = new Color(0, 0, 0, 100);

            gridLine.SetData<Color>(pixels);

            // Set window line colour (the thick horizontal and vertical lines).
            windowSpaceLine = new Texture2D(Globals.graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixels[0] = new Color(0, 0, 0, 255);
            windowSpaceLine.SetData<Color>(pixels);

            SetButtons();
        }

        private void SetButtons()
        {
            buttonNext = new Button(">", Fonts.font14, new Vector2(30, 30), new Vector2(Globals.viewport.X - 40, Globals.viewport.Y - 70), Color.DarkOliveGreen);
            buttonPrevious = new Button("<", Fonts.font14, new Vector2(30, 30), new Vector2(Globals.viewport.X - 100, Globals.viewport.Y - 70), Color.DarkOliveGreen);
            buttonPlay = new Button("||", Fonts.font14, new Vector2(30, 30), new Vector2(Globals.viewport.X - 70, Globals.viewport.Y - 70), Color.DarkSlateBlue);
            buttonReset = new Button("Reset", Fonts.font14, new Vector2(90, 30), new Vector2(Globals.viewport.X - 100, Globals.viewport.Y - 40), Color.DarkGreen);
            buttonBack = new Button("Back", Fonts.font14, new Vector2(90, 30), new Vector2(Globals.viewportWidth - 100, Globals.viewportHeight - 100), Color.DarkRed);

            buttonPlay.OnClick += (b) =>
            {
                animating = !animating;
                if (!animating)
                    buttonPlay.SetColour(Color.DarkSlateBlue);
                else
                    buttonPlay.SetColour(Color.DarkSlateGray);
            };

            // Next - incremenent the count if possible.
            buttonNext.OnClick += (b) => { if (!animating && animationCounter < animationCounterLimit) animationCounter++; };
            buttonNext.OnHold += (b) => { if (!animating && animationCounter < animationCounterLimit) animationCounter++; };

            // Previous - decrement the count if possible.
            buttonPrevious.OnClick += (b) => { if (!animating && animationCounter >= 0) animationCounter--; };
            buttonPrevious.OnHold += (b) => { if (!animating && animationCounter >= 0) animationCounter--; };

            // Reset - call all inits to restart.
            buttonReset.OnClick += (b) => { Init(); DerivedInit(); };

            // Back - call delegate BackToMenu function.
            buttonBack.OnClick += (b) => { BackToMenu(); };
        }

        public virtual void Update(GameTime gameTime)
        {
            StateChanges(gameTime);

            if (state == 42)
            {
                buttonPlay.Update(gameTime);
                buttonNext.Update(gameTime);
                buttonPrevious.Update(gameTime);
                buttonReset.Update(gameTime);
                buttonBack.Update(gameTime);
            }

            if (animating && animationCounter < animationCounterLimit)
                animationCounter++;
            else if (animating && animationCounter >= animationCounterLimit)
                buttonPlay.EmulateClick();
        }

        // Convert XNA window coordinates to OpenGL windowspace.
        protected Vector2 NormalisePoints(Vector2 vIn)
        {
            float normalisedX = (vIn.X - 0) / ((Globals.viewportWidth / 2) - 0) - 0.5f * 2;
            float normalisedY = -((vIn.Y - 0) / ((Globals.viewportHeight / 2) - 0) - 0.5f * 2);

            return new Vector2(normalisedX, normalisedY);
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            DrawGrid(spriteBatch);

            buttonPrevious.Draw(spriteBatch);
            buttonPlay.Draw(spriteBatch);
            buttonNext.Draw(spriteBatch);
            buttonReset.Draw(spriteBatch);
            buttonBack.Draw(spriteBatch);

            DrawText(spriteBatch);
        }

        protected virtual void DrawText(SpriteBatch spriteBatch) { }

        protected void DrawGrid(SpriteBatch spriteBatch)
        {

            //Pixel grid
            for (int i = 0; i <= (Globals.viewportHeight / Globals.pixelSize); i++)
                spriteBatch.Draw(gridLine, new Rectangle(0, i * (Globals.viewportHeight / (Globals.viewportHeight / Globals.pixelSize)), Globals.viewportWidth, 1), Color.White);

            for (int i = 0; i <= (Globals.viewportWidth / Globals.pixelSize); i++)
                spriteBatch.Draw(gridLine, new Rectangle(i * (Globals.viewportWidth / (Globals.viewportWidth / Globals.pixelSize)), 0, 1, Globals.viewportHeight), Color.White);
            
            spriteBatch.Draw(windowSpaceLine, new Rectangle(Globals.viewportWidth / 2 - 2, 0, 4, Globals.viewportHeight), Color.White);
            spriteBatch.Draw(windowSpaceLine, new Rectangle(0, Globals.viewportHeight / 2 - 2, (Globals.viewportWidth), 4), Color.White);

            /*//Screen space grid
            for (int i = 0; i <= 20; i++)
                spriteBatch.Draw(gridLine, new Rectangle(0, i * (Globals.viewportHeight/20), Globals.viewportWidth, 1), Color.White);

            for (int i = 0; i <= 20; i++)
                spriteBatch.Draw(gridLine, new Rectangle(i * (Globals.viewportWidth/20), 0, 1, Globals.viewportHeight), Color.White);
            */
        }
    }
}
