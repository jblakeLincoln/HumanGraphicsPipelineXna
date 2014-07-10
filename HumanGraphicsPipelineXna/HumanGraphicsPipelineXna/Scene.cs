﻿using System;
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
        protected Vector2[] trianglePoints;
        protected Vector2[] normalisedTrianglePoints;
        protected Square[] triangleSquares;
        protected Line[] triangleLines;

        Texture2D gridLine;
        Texture2D windowSpaceLine;
        protected Button buttonNext;
        protected Button buttonPrevious;
        protected Button buttonPlay;
        protected Button buttonReset;
        protected Button buttonBack;

        protected int animationCounter;
        protected int animationCounterLimit;

        bool animating = false;

        public delegate void ThisBackToMenu();
        public event ThisBackToMenu BackToMenu;

        protected enum State
        {
            PickPoint1=0,
            PickPoint2=1,
            PickPoint3=2,
            Animate=3,
        }

        protected State state = State.PickPoint1;

        protected abstract void DerivedInit();

        public Scene()
        {
            Init();
            DerivedInit();
        }

        public void Init()
        {
            state = State.PickPoint1;
            animationCounter = -1;
            animationCounterLimit = 0;

            trianglePoints = new Vector2[3];
            normalisedTrianglePoints = new Vector2[3];
            triangleSquares = new Square[3];
            triangleLines = new Line[3]; //AB, BC, CA


            gridLine = new Texture2D(Globals.graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] pixels = new Color[1];
            for (int i = 0; i < 1; i++)
                pixels[i] = new Color(0, 0, 0, 100);

            gridLine.SetData<Color>(pixels);

            windowSpaceLine = new Texture2D(Globals.graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixels[0] = new Color(0, 0, 0, 255);
            windowSpaceLine.SetData<Color>(pixels);

            buttonNext = new Button(">", Fonts.font14, new Vector2(30, 30), new Vector2(Globals.viewport.X - 40, Globals.viewport.Y - 70), Color.DarkOliveGreen);
            buttonPrevious = new Button("<", Fonts.font14, new Vector2(30, 30), new Vector2(Globals.viewport.X - 100, Globals.viewport.Y - 70), Color.DarkOliveGreen);
            buttonPlay = new Button("||", Fonts.font14, new Vector2(30, 30), new Vector2(Globals.viewport.X - 70, Globals.viewport.Y - 70), Color.DarkSlateBlue);
            buttonReset = new Button("Reset", Fonts.font14, new Vector2(90, 30), new Vector2(Globals.viewport.X - 100, Globals.viewport.Y - 40), Color.DarkGreen);
            buttonBack = new Button("Back", Fonts.font14, new Vector2(90, 30), new Vector2(Globals.viewportWidth-100, Globals.viewportHeight-100), Color.DarkRed);

            buttonPlay.OnClick += (b) =>
            {
                animating = !animating;
                if (!animating)
                    buttonPlay.SetColour(Color.DarkSlateBlue);
                else
                    buttonPlay.SetColour(Color.DarkSlateGray);

            };

            buttonNext.OnClick += (b) => { if (!animating && animationCounter < animationCounterLimit) animationCounter++; };
            buttonNext.OnPress += (b) => { if (!animating && animationCounter < animationCounterLimit) animationCounter++; };
            buttonPrevious.OnClick += (b) => { if (!animating && animationCounter >= 0) animationCounter--; };
            buttonPrevious.OnPress += (b) => { if (!animating && animationCounter >= 0) animationCounter--; };
            buttonReset.OnClick += (b) => { Init(); DerivedInit(); };
            buttonBack.OnClick += (b) => { BackToMenu(); };
        }

        public virtual void Update(GameTime gameTime)
        {
            
            StateChanges(gameTime);

            if (state == State.Animate)
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

        protected abstract void LastTrianglePointPlaced(GameTime gameTime);

        private void StateChanges(GameTime gameTime)
        {
            if (Inputs.MouseState.LeftButton == ButtonState.Released && Inputs.MouseStatePrevious.LeftButton == ButtonState.Pressed)
            {
                if ((int)state < 3)
                {
                    trianglePoints[(int)state] = new Vector2(Inputs.MouseState.X, Inputs.MouseState.Y);
                    triangleSquares[(int)state] = new Square(new Vector2(Inputs.MouseState.X - 5, Inputs.MouseState.Y - 5), new Vector2(10, 10), Color.Green);
                    state++;
                }
                if ((int)state ==3)
                {
                    LastTrianglePointPlaced(gameTime);

                    triangleLines[0] = new Line(trianglePoints[0], trianglePoints[1], Color.Black, 1);
                    triangleLines[1] = new Line(trianglePoints[1], trianglePoints[2], Color.Black, 1);
                    triangleLines[2] = new Line(trianglePoints[2], trianglePoints[0], Color.Black, 1);
                }
            }
        }

        protected void DrawGrid(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= (Globals.viewportHeight/Globals.pixelSize); i++)
                spriteBatch.Draw(gridLine, new Rectangle(0, i * (Globals.viewportHeight / (Globals.viewportHeight/Globals.pixelSize)), Globals.viewportWidth, 1), Color.White);

            for (int i = 0; i <= (Globals.viewportWidth / Globals.pixelSize); i++)
                spriteBatch.Draw(gridLine, new Rectangle(i * (Globals.viewportWidth / (Globals.viewportWidth / Globals.pixelSize)), 0, 1, Globals.viewportHeight), Color.White);        

            spriteBatch.Draw(windowSpaceLine, new Rectangle(Globals.viewportWidth / 2 - 2, 0, 4, Globals.viewportHeight), Color.White);
            spriteBatch.Draw(windowSpaceLine, new Rectangle(0, Globals.viewportHeight / 2 - 2, (Globals.viewportWidth), 4), Color.White);
        }


        public Vector2 NormalisePoints(Vector2 vIn)
        {
            float normalisedX = (vIn.X - 0) / ((Globals.viewportWidth / 2) - 0) - 0.5f * 2;
            float normalisedY = -((vIn.Y - 0) / ((Globals.viewportHeight / 2) - 0) - 0.5f * 2);

            return new Vector2(normalisedX, normalisedY);
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            DrawGrid(spriteBatch);

            for (int i = 0; i < 3; i++)
            {
                if (trianglePoints[i] != Vector2.Zero)
                {
                    triangleSquares[i].Draw(spriteBatch);

                    normalisedTrianglePoints[i] = NormalisePoints(trianglePoints[i]);

                    spriteBatch.DrawString(Fonts.arial14, Convert.ToChar(65+i).ToString(), new Vector2(trianglePoints[i].X - 20, trianglePoints[i].Y - 20), Color.White);
                }
            }

            if (trianglePoints[2] != Vector2.Zero)
            {
                for (int i = 0; i < 3; i++)
                    triangleLines[i].Draw(spriteBatch);
                ActionOnTriangleDraw(spriteBatch);
            }

            buttonPrevious.Draw(spriteBatch);
            buttonPlay.Draw(spriteBatch);
            buttonNext.Draw(spriteBatch);
            buttonReset.Draw(spriteBatch);
            buttonBack.Draw(spriteBatch);

            DrawText(spriteBatch);
        }

        protected abstract void ActionOnTriangleDraw(SpriteBatch spriteBatch);

        protected virtual void DrawText(SpriteBatch spriteBatch) { }
    }
}
