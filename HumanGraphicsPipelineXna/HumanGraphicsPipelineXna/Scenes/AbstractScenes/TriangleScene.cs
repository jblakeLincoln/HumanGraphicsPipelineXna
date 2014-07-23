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

        protected Vector2[] trianglePoints;
        protected Vector2[] normalisedTrianglePoints;
        protected Square[] triangleSquares;
        protected Line[] triangleLines;

        protected override void DerivedInit()
        {
            state = 0;
            trianglePoints = new Vector2[3];
            normalisedTrianglePoints = new Vector2[3];
            triangleSquares = new Square[3];
            triangleLines = new Line[3]; //AB, BC, CA

            
        }

        protected abstract void ActionOnTrianglePlaced(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch);

        protected override void StateChanges(GameTime gameTime)
        {
            if (Inputs.MouseState.LeftButton == ButtonState.Released && Inputs.MouseStatePrevious.LeftButton == ButtonState.Pressed)
            {
                if (state < 3)
                {
                    trianglePoints[(int)state] = new Vector2(Inputs.MouseState.X, Inputs.MouseState.Y);
                    triangleSquares[(int)state] = new Square(new Vector2(Inputs.MouseState.X - 5, Inputs.MouseState.Y - 5), new Vector2(10, 10), Color.Green);
                    normalisedTrianglePoints[(int)state] = NormalisePoints(trianglePoints[(int)state]);

                    state++;
                }
                if (state ==3)
                {
                    state = 42;
                    LastPointPlaced(gameTime);
                    triangleLines[0] = new Line(trianglePoints[0], trianglePoints[1], Color.Black, 1);
                    triangleLines[1] = new Line(trianglePoints[1], trianglePoints[2], Color.Black, 1);
                    triangleLines[2] = new Line(trianglePoints[2], trianglePoints[0], Color.Black, 1);
                }
            }
        }

        protected abstract void DrawOnAnimate(SpriteBatch spriteBatch);

        public override void Draw(SpriteBatch spriteBatch)
        {

            base.Draw(spriteBatch);
            Vector2 normalisedScreen = Vector2.Normalize(new Vector2(Globals.viewport.X, Globals.viewport.Y));

            for (int i = 0; i < 3; i++)
            {
                if (trianglePoints[i] != Vector2.Zero)
                {
                    triangleSquares[i].Draw(spriteBatch);
                    
                    spriteBatch.DrawString(Fonts.arial14, Convert.ToChar(65 + i).ToString(), new Vector2(trianglePoints[i].X - 20, trianglePoints[i].Y - 20), Color.White);
                }
            }

            if (trianglePoints[2] != Vector2.Zero)
            {
                for (int i = 0; i < 3; i++)
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

        // Within a 2D array, find the value prior to the current one (x-1).
        protected Vector2 GetPreviousValue<T>(Vector2 vIn, List<List<T>> l)
        {
            // 1D representation of array
            int t = (int)(vIn.Y * l[0].Count + vIn.X);

            if (t > 0)
                t--;

            int xx = 0;
            int yy = 0;

            // Get 2D representations.
            yy = (int)(t / l[0].Count);
            xx = (int)(t % l[0].Count);

            // Correct error if out of bounds.
            if (xx > l.Count)
            {
                xx = 0;
                yy++;
            }

            return new Vector2(xx, yy);
        }

        // Implementation of algorithm by derived class.
        
    }
}
