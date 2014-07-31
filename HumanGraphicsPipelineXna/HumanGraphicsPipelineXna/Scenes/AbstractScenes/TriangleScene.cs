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
    abstract class TriangleScene : Scene
    {
        protected enum TriangleState
        {
            PickPoint1 = 0,
            PickPoint2 = 1,
            PickPoint3 = 2,
            Animate = 42,
        }

        protected int triangleCount = 3;
        protected Vector2[] trianglePoints;
        protected Vector2[] normalisedTrianglePoints;
        protected Square[] triangleSquares;
        protected Line[] triangleLines;

        protected override void DerivedInit()
        {
            state = 0;
            trianglePoints = new Vector2[triangleCount];
            normalisedTrianglePoints = new Vector2[triangleCount];
            triangleSquares = new Square[triangleCount];
            triangleLines = new Line[triangleCount];
        }

        /// <summary>
        /// When all points are placed.
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected abstract void ActionOnTrianglePlaced(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch);

        /// <summary>
        /// When a new point is placed.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void StateChanges(GameTime gameTime)
        {
            if (Inputs.MouseState.LeftButton == ButtonState.Released && Inputs.MouseStatePrevious.LeftButton == ButtonState.Pressed)
            {
                if (state < triangleCount)
                {
                    trianglePoints[(int)state] = new Vector2(Inputs.MouseState.X, Inputs.MouseState.Y);
                    triangleSquares[(int)state] = new Square(new Vector2(Inputs.MouseState.X - 5, Inputs.MouseState.Y - 5), new Vector2(10, 10), Color.Green);
                    normalisedTrianglePoints[(int)state] = NormalisePoints(trianglePoints[(int)state]);

                    state++;
                }
                if (state == triangleCount)
                {
                    state = 42;
                    LastPointPlaced(gameTime);

                    for (int i = 1; i < triangleCount; i++)
                    {
                        triangleLines[i - 1] = new Line(trianglePoints[i - 1], trianglePoints[i], Color.Black, 1f);
                    }
                    triangleLines[triangleCount - 1] = new Line(trianglePoints[triangleCount-1], trianglePoints[0], Color.Black, 1);
                }
            }
        }

        protected abstract void DrawOnAnimate(SpriteBatch spriteBatch);

        public override void Draw(SpriteBatch spriteBatch)
        {

            base.Draw(spriteBatch);
            Vector2 normalisedScreen = Vector2.Normalize(new Vector2(Globals.viewport.X, Globals.viewport.Y));

            for (int i = 0; i < triangleCount; i++)
            {
                if (trianglePoints[i] != Vector2.Zero)
                {
                    triangleSquares[i].Draw(spriteBatch);
                    
                    spriteBatch.DrawString(Fonts.arial14, Convert.ToChar(65 + i).ToString(), new Vector2(trianglePoints[i].X - 20, trianglePoints[i].Y - 20), Color.White);
                }
            }

            if (trianglePoints[triangleCount-1] != Vector2.Zero)
            {
                for (int i = 0; i < triangleCount; i++)
                    triangleLines[i].Draw(spriteBatch);
                ActionOnTrianglePlaced(spriteBatch);
            }

            if (state == (int)TriangleState.Animate)
            {
                DrawOnAnimate(spriteBatch);
            }

            int yPos = 0;
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point A: " + normalisedTrianglePoints[0], new Vector2(10, yPos += 20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point B: " + normalisedTrianglePoints[1], new Vector2(10, yPos += 20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point C: " + normalisedTrianglePoints[2], new Vector2(10, yPos += 20), Color.White, Color.Black);

            if (state == 42)
            {
                
            }

        } 
    }
}
