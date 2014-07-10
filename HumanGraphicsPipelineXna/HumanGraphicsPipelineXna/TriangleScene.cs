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
        protected Vector2 minimum; // Minimum triangle X and Y.
        protected Vector2 maximum; // Maximum triangle X and Y.
        protected Vector2 check; // Point in px being checked.
        protected Vector2 previousPixelInBox;
        protected Vector2 pixelInBox; // Pixel within the bounding box at the current checking location.
        protected Square boundingBox;

        protected List<List<bool>> listPixelCheck = new List<List<bool>>();
        protected List<List<Square>> listSquares = new List<List<Square>>();
        protected List<List<float[]>> listResults = new List<List<float[]>>();

        protected override void LastTrianglePointPlaced(GameTime gameTime)
        {
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
            listPixelCheck = new List<List<bool>>();
            listSquares = new List<List<Square>>();
            listResults = new List<List<float[]>>();
            minimum = Vector2.Zero; // Minimum triangle X and Y.
            maximum = Vector2.Zero; // Maximum triangle X and Y.
            check = Vector2.Zero; // Point in px being checked.
            previousPixelInBox = Vector2.Zero;
            pixelInBox = Vector2.Zero; // Pixel within the bounding box at the current checking location.
        }

        protected override void ActionOnTriangleDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            boundingBox.Draw(spriteBatch);


            if (listPixelCheck.Count == 0)
            {
                int count = 0;

                for (int i = 0; i < (maximum.X - minimum.X) / Globals.pixelSize; i++)
                {
                    listPixelCheck.Add(new List<bool>());
                    listSquares.Add(new List<Square>());
                    listResults.Add(new List<float[]>());
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
                        listResults[i].Add(new float[3]);
                        //listResults[i][j][0] = v0;
                        //listResults[i][j][1] = v1;
                        //listResults[i][j][2] = v2;

                        count++;
                    }
                    count = 0;
                }

                animationCounterLimit = (listPixelCheck.Count * listPixelCheck[0].Count-2);
                //bool b = PerformFillingFunction(check);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            base.Draw(spriteBatch);


            Vector2 normalisedScreen = Vector2.Normalize(new Vector2(Globals.viewport.X, Globals.viewport.Y));

            if (state == State.Animate)
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

        }

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

        protected float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        protected abstract bool PerformFillingFunction(Vector2 pointToCheck, int i, int j);
    }
}
