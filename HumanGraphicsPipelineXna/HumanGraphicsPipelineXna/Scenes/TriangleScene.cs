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

        protected Vector2 minimum; // Minimum triangle X and Y.
        protected Vector2 maximum; // Maximum triangle X and Y.
        protected Vector2 check; // Point in px being checked.
        protected Vector2 previousPixelInBox;
        protected Vector2 pixelInBox; // Pixel within the bounding box at the current checking location.
        protected Square boundingBox;

        protected List<List<bool>> listPixelCheck = new List<List<bool>>();
        protected List<List<Square>> listSquares = new List<List<Square>>();
        protected List<List<float[]>> listResults = new List<List<float[]>>();

        // Perform action 
        protected override void LastPointPlaced(GameTime gameTime)
        {
            // Find dimensions of bounding box and create it.
            minimum = new Vector2(Math.Min((int)trianglePoints[0].X, Math.Min((int)trianglePoints[1].X, (int)trianglePoints[2].X)),
                        Math.Min((int)trianglePoints[0].Y, Math.Min((int)trianglePoints[1].Y, (int)trianglePoints[2].Y)));
            maximum = new Vector2(Math.Max((int)trianglePoints[0].X, Math.Max((int)trianglePoints[1].X, (int)trianglePoints[2].X)),
                Math.Max((int)trianglePoints[0].Y, Math.Max((int)trianglePoints[1].Y, (int)trianglePoints[2].Y)));

            minimum = new Vector2(minimum.X - (minimum.X % Globals.pixelSize), minimum.Y - (minimum.Y % Globals.pixelSize));
            maximum = new Vector2(maximum.X - (maximum.X % Globals.pixelSize) + Globals.pixelSize, maximum.Y - (maximum.Y % Globals.pixelSize) + Globals.pixelSize);
            boundingBox = new Square(new Vector2(minimum.X, minimum.Y), new Vector2(maximum.X - minimum.X, maximum.Y - minimum.Y), new Color(255, 0, 0, 120));
        }

        protected override void DerivedInit()
        {
            state = 0;
            trianglePoints = new Vector2[3];
            normalisedTrianglePoints = new Vector2[3];
            triangleSquares = new Square[3];
            triangleLines = new Line[3]; //AB, BC, CA

            listPixelCheck = new List<List<bool>>();
            listSquares = new List<List<Square>>();
            listResults = new List<List<float[]>>();
            minimum = Vector2.Zero; // Minimum triangle X and Y.
            maximum = Vector2.Zero; // Maximum triangle X and Y.
            check = Vector2.Zero; // Point in px being checked.
            previousPixelInBox = Vector2.Zero;
            pixelInBox = Vector2.Zero; // Pixel within the bounding box at the current checking location.
        }

        protected override void StateChanges(GameTime gameTime)
        {
            if (Inputs.MouseState.LeftButton == ButtonState.Released && Inputs.MouseStatePrevious.LeftButton == ButtonState.Pressed)
            {
                if (state < 3)
                {
                    trianglePoints[(int)state] = new Vector2(Inputs.MouseState.X, Inputs.MouseState.Y);
                    triangleSquares[(int)state] = new Square(new Vector2(Inputs.MouseState.X - 5, Inputs.MouseState.Y - 5), new Vector2(10, 10), Color.Green);
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

        protected override void ActionOnTriangleDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            boundingBox.Draw(spriteBatch);
            
            // Once all points are placed, check if they are within triangle.
            if (listPixelCheck.Count == 0)
            {
                Console.Clear();
                int count = 0;
                for (int i = 0; i < (maximum.X - minimum.X) / Globals.pixelSize; i++)
                {
                    listPixelCheck.Add(new List<bool>());
                    listSquares.Add(new List<Square>());
                    for (int j = 0; j < (maximum.Y - minimum.Y) / Globals.pixelSize; j++)
                    {
                        listPixelCheck[i].Add(false);
                        check = (new Vector2(minimum.X + (i * Globals.pixelSize) + (Globals.pixelSize / 2), minimum.Y + (j * Globals.pixelSize) + (Globals.pixelSize / 2)));
                        Color col;

                        if (PerformFillingFunction(NormalisePoints(check), i, j))
                        {
                            listPixelCheck[i][j] = true;
                            col = new Color(0, 120, 120, 180);
                        }
                        else
                        {
                            listPixelCheck[i][j] = false;
                            col = new Color(255, 0, 0, 100);
                        }

                        listSquares[i].Add(new Square(new Vector2(minimum.X + (i * Globals.pixelSize), minimum.Y + (j * Globals.pixelSize)), new Vector2(Globals.pixelSize, Globals.pixelSize), col));
                        
                        Vector2 pOut = NormalisePoints(listSquares[i][j].position);
                        //Console.WriteLine("X: " + pOut.X + "\tY: " + pOut.Y + "\t" + listPixelCheck[i][j]);

                        string xSpace = (pOut.X < 0) ? "" : " ";
                        string ySpace = (pOut.Y < 0) ? "" : " ";
                        Console.WriteLine("X:" + xSpace + "{0}\t\tY:" + ySpace + "{1}\t\t{2}", pOut.X.ToString("F"), pOut.Y.ToString("F"), listPixelCheck[i][j]);
                        count++;
                    }
                    count = 0;
                }
                animationCounterLimit = (listPixelCheck.Count * listPixelCheck[0].Count-2);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            base.Draw(spriteBatch);
            Vector2 normalisedScreen = Vector2.Normalize(new Vector2(Globals.viewport.X, Globals.viewport.Y));

            for (int i = 0; i < 3; i++)
            {
                if (trianglePoints[i] != Vector2.Zero)
                {
                    triangleSquares[i].Draw(spriteBatch);
                    normalisedTrianglePoints[i] = NormalisePoints(trianglePoints[i]);
                    spriteBatch.DrawString(Fonts.arial14, Convert.ToChar(65 + i).ToString(), new Vector2(trianglePoints[i].X - 20, trianglePoints[i].Y - 20), Color.White);
                }
            }

            if (trianglePoints[2] != Vector2.Zero)
            {
                for (int i = 0; i < 3; i++)
                    triangleLines[i].Draw(spriteBatch);
                ActionOnTriangleDraw(spriteBatch);
            }

            if (state == (int)TriangleState.Animate)
            {
                bool breakNow = false;
                int count = 0;
                for (int j = 0; j < listPixelCheck[0].Count; j++)
                {
                    for (int i = 0; i < listPixelCheck.Count; i++)
                    {

                        listSquares[i][j].Draw(spriteBatch);

                        if (count > animationCounter)
                        {
                            if (new Vector2(i, j) != pixelInBox)
                                previousPixelInBox = new Vector2(pixelInBox.X, pixelInBox.Y);

                            Square sq = new Square(new Vector2(minimum.X + (pixelInBox.X * Globals.pixelSize) + (Globals.pixelSize / 2)-2, minimum.Y + (Globals.pixelSize * pixelInBox.Y) + (Globals.pixelSize / 2)-2), new Vector2(4, 4), Color.Green);
                            sq.Draw(spriteBatch);
                            breakNow = true;
                            check = new Vector2(minimum.X + (i * Globals.pixelSize) + (Globals.pixelSize / 2), minimum.Y + (j * Globals.pixelSize) + (Globals.pixelSize / 2));
                            pixelInBox.X = i;
                            pixelInBox.Y = j;
                            break;
                        }

                        

                        int y = 0;

                        count++;
                    }
                    if (breakNow)
                        break;
                }
            }

            int yPos = 0;
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point A: " + normalisedTrianglePoints[0], new Vector2(10, yPos += 20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point B: " + normalisedTrianglePoints[1], new Vector2(10, yPos += 20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point C: " + normalisedTrianglePoints[2], new Vector2(10, yPos += 20), Color.White, Color.Black);

            if (state == 42)
            {
                yPos = Globals.viewportHeight;
                if (listPixelCheck[(int)pixelInBox.X][(int)pixelInBox.Y])
                    Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Pixel is within triangle.", new Vector2(10, yPos -= 20), Color.White, Color.Black);
                else
                    Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Pixel not within triangle.", new Vector2(10, yPos -= 20), Color.White, Color.Black);
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
        protected abstract bool PerformFillingFunction(Vector2 pointToCheck, int i, int j);
    }
}
