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
using DColour = System.Drawing.Color;

namespace HumanGraphicsPipelineXna
{
    abstract class TriangleRasterisingScene : TriangleScene
    {

        protected List<List<bool>> listPixelCheck = new List<List<bool>>();
        protected List<List<Square>> listSquares = new List<List<Square>>();
        protected List<List<float[]>> listResults = new List<List<float[]>>();

        protected Vector2 minimum; // Minimum triangle X and Y.
        protected Vector2 maximum; // Maximum triangle X and Y.
        protected Vector2 check; // Point in px being checked.
        protected Vector2 previousPixelInBox;
        protected Vector2 pixelInBox; // Pixel within the bounding box at the current checking location.
        protected Square boundingBox;

        protected abstract bool PerformFillingFunction(Vector2 pointToCheck, int i, int j);

        protected override void DerivedInit()
        {
            base.DerivedInit();

            listPixelCheck = new List<List<bool>>();
            listSquares = new List<List<Square>>();
            listResults = new List<List<float[]>>();
            minimum = Vector2.Zero; // Minimum triangle X and Y.
            maximum = Vector2.Zero; // Maximum triangle X and Y.
            check = Vector2.Zero; // Point in px being checked.
            previousPixelInBox = Vector2.Zero;
            pixelInBox = Vector2.Zero; // Pixel within the bounding box at the current checking location.
        }

        protected override void LastPointPlaced(GameTime gameTime)
        {
            minimum = new Vector2(Math.Min((int)trianglePoints[0].X, Math.Min((int)trianglePoints[1].X, (int)trianglePoints[2].X)),
                        Math.Min((int)trianglePoints[0].Y, Math.Min((int)trianglePoints[1].Y, (int)trianglePoints[2].Y)));
            maximum = new Vector2(Math.Max((int)trianglePoints[0].X, Math.Max((int)trianglePoints[1].X, (int)trianglePoints[2].X)),
                Math.Max((int)trianglePoints[0].Y, Math.Max((int)trianglePoints[1].Y, (int)trianglePoints[2].Y)));

            minimum = new Vector2(minimum.X - (minimum.X % Globals.pixelSize), minimum.Y - (minimum.Y % Globals.pixelSize));
            maximum = new Vector2(maximum.X - (maximum.X % Globals.pixelSize) + Globals.pixelSize, maximum.Y - (maximum.Y % Globals.pixelSize) + Globals.pixelSize);
            boundingBox = new Square(new Vector2(minimum.X, minimum.Y), new Vector2(maximum.X - minimum.X, maximum.Y - minimum.Y), new Color(255, 0, 0, 120));

            List<DColour> dCol = new List<DColour>(){
                DColour.Red,
                DColour.Yellow,
                DColour.Green,
                DColour.Blue,
                DColour.White,
                DColour.Gray,
                DColour.CornflowerBlue,
                DColour.Plum,
                DColour.Olive,
                DColour.Red,
                DColour.Yellow,
                DColour.Green,
                DColour.Blue,
                DColour.White,
                DColour.Gray,
                DColour.CornflowerBlue,
                DColour.Plum,
                DColour.Olive,
                DColour.Red,
                DColour.Yellow,
                DColour.Green,
                DColour.Blue,
                DColour.White,
                DColour.Gray,
                DColour.CornflowerBlue,
                DColour.Plum,
                DColour.Olive,
            };

            List<List<Vector2>> vecArr = new List<List<Vector2>>();

            vecArr.Add(new List<Vector2>());

            vecArr[0].Add(new Vector2(3f));
            vecArr[0].Add(new Vector2(3f));
            vecArr[0].Add(new Vector2(3f));
            vecArr[0].Add(new Vector2(3f));


            vecArr[0] = vecArr[0].Distinct().ToList();
        }

        protected override void DrawOnAnimate(SpriteBatch spriteBatch)
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

                        Square sq = new Square(new Vector2(minimum.X + (pixelInBox.X * Globals.pixelSize) + (Globals.pixelSize / 2) - 2, minimum.Y + (Globals.pixelSize * pixelInBox.Y) + (Globals.pixelSize / 2) - 2), new Vector2(4, 4), Color.Green);
                        sq.Draw(spriteBatch);
                        breakNow = true;
                        check = new Vector2(minimum.X + (i * Globals.pixelSize) + (Globals.pixelSize / 2), minimum.Y + (j * Globals.pixelSize) + (Globals.pixelSize / 2));
                        pixelInBox.X = i;
                        pixelInBox.Y = j;
                        break;
                    }

                    count++;
                }
                if (breakNow)
                    break;
            }

            int yPos = Globals.viewportHeight;
            if (listPixelCheck[(int)pixelInBox.X][(int)pixelInBox.Y])
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Pixel is within triangle.", new Vector2(10, yPos -= 20), Color.White, Color.Black);
            else
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Pixel not within triangle.", new Vector2(10, yPos -= 20), Color.White, Color.Black);
        }

        protected override void ActionOnTrianglePlaced(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            boundingBox.Draw(spriteBatch);

            // Once all points are placed, check if they are within triangle.
            if (listPixelCheck.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("New Triangle");
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
                animationCounterLimit = (listPixelCheck.Count * listPixelCheck[0].Count - 2);
            }
        }
    }
}
