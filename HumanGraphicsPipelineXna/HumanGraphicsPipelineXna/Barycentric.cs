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
    class Barycentric : Scene
    {

        List<List<bool>> listPixelCheck = new List<List<bool>>();
        List<List<Square>> listSquares = new List<List<Square>>();
        List<List<float[]>> listResults = new List<List<float[]>>();
        Vector2 minimum; // Minimum triangle X and Y.
        Vector2 maximum; // Maximum triangle X and Y.
        Vector2 check; // Point in px being checked.
        Vector2 previousPixelInBox;
        Vector2 pixelInBox; // Pixel within the bounding box at the current checking location.
        Square boundingBox;


        public Barycentric()
            : base()
        { 
        
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

                        if (PerformBarycentric(check))
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

                animationCounterLimit = (listPixelCheck.Count * listPixelCheck[0].Count);
                bool b = PerformBarycentric(check);
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
                        if (count > animationCounter)
                        {
                            if (new Vector2(i, j) != pixelInBox)
                                previousPixelInBox = new Vector2(pixelInBox.X, pixelInBox.Y);
                            breakNow = true;
                            check = new Vector2(minimum.X + (i * Globals.pixelSize) + (Globals.pixelSize / 2), minimum.Y + (j * Globals.pixelSize) + (Globals.pixelSize / 2));
                            pixelInBox.X = i;
                            pixelInBox.Y = j;
                            break;
                        }

                        listSquares[i][j].Draw(spriteBatch);

                        int y = 0;

                        count++;
                    }
                    if (breakNow)
                        break;
                }
            }

        }

        protected override void DrawText(SpriteBatch spriteBatch)
        {
            int y = 0;
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point A: " + normalisedTrianglePoints[0], new Vector2(10, y += 20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point B: " + normalisedTrianglePoints[1], new Vector2(10, y += 20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point C: " + normalisedTrianglePoints[2], new Vector2(10, y += 20), Color.White, Color.Black);

            y += 30;

            if (listResults.Count > 0)
            {
                /*
                Vector2 previous = GetPreviousValue(pixelInBox, listPixelCheck);
                Console.WriteLine("Pixel: " + pixelInBox);
                Console.WriteLine("PPP: " + previousPixelInBox);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient AB: " + listResults[(int)previous.X][(int)previous.Y][0], new Vector2(10, y += 20), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient BC: " + listResults[(int)previous.X][(int)previous.Y][1], new Vector2(10, y += 20), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient CA: " + listResults[(int)previous.X][(int)previous.Y][2], new Vector2(10, y += 20), Color.White, Color.Black);
                */
            }
        }

        protected Vector2 GetPreviousValue<T>(Vector2 vIn, List<List<T>> l)
        {
            // 1D representation of array
            int t = (int)(vIn.Y * l[0].Count + pixelInBox.X);

            if (t > 0)
                t--;

            int xx = 0;
            int yy = 0;

            // Get 2D representations.
            yy = (int)(t / l[0].Count);
            xx = (int)(t % l[0].Count);

            // Correct error if out of bounds.
            if (xx > listPixelCheck.Count)
            {
                xx = 0;
                yy++;
            }

            return new Vector2(xx, yy);
        }

        protected bool PerformBarycentric(Vector2 p)
        {
            Vector2 vs1 = new Vector2(trianglePoints[1].X - trianglePoints[0].X, trianglePoints[1].Y - trianglePoints[0].Y);
            Vector2 vs2 = new Vector2(trianglePoints[2].X - trianglePoints[0].X, trianglePoints[2].Y - trianglePoints[0].Y);

            Vector2 q = new Vector2(p.X - trianglePoints[0].X, p.Y - trianglePoints[0].Y);

            float s = (CrossProduct(q,vs2)) / CrossProduct(vs1, vs2);
            float t = (CrossProduct(vs1, q)) / CrossProduct(vs1, vs2);

            if (s >= 0 && t >= 0 && s + t <= 1)
                return true;

            return false;
        }

        protected float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
    }
}
