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
    class HalfSpace : Scene
    {
        List<List<bool>> listPixelCheck = new List<List<bool>>();
        List<List<Square>> listSquares = new List<List<Square>>();

        Vector2 minimum = Vector2.Zero; // Minimum triangle X and Y.
        Vector2 maximum = Vector2.Zero; // Maximum triangle X and Y.
        Vector2 check = Vector2.Zero; // Point in px being checked.
        Vector2 pixelInBox = Vector2.Zero; // Pixel within the bounding box at the current checking location.
        Square boundingBox;

        // Halfspace checks
        float v0 = 0;
        float v1 = 0;
        float v2 = 0;

        public HalfSpace()
            : base()
        { 
        
        }

        protected override void LastTrianglePointPlaced(GameTime gameTime)
        {
            minimum = new Vector2(Math.Min((int)trianglePoints[0].X, Math.Min((int)trianglePoints[1].X, (int)trianglePoints[2].X)),
                        Math.Min((int)trianglePoints[0].Y, Math.Min((int)trianglePoints[1].Y, (int)trianglePoints[2].Y)));
            maximum = new Vector2(Math.Max((int)trianglePoints[0].X, Math.Max((int)trianglePoints[1].X, (int)trianglePoints[2].X)),
                Math.Max((int)trianglePoints[0].Y, Math.Max((int)trianglePoints[1].Y, (int)trianglePoints[2].Y)));

            minimum = new Vector2(minimum.X - (minimum.X % Globals.pixelSize), minimum.Y - (minimum.Y % Globals.pixelSize));
            maximum = new Vector2(maximum.X - (maximum.X % Globals.pixelSize) + Globals.pixelSize, maximum.Y - (maximum.Y % Globals.pixelSize) + Globals.pixelSize);

            boundingBox = new Square(new Vector2(minimum.X, minimum.Y), new Vector2(maximum.X - minimum.X, maximum.Y - minimum.Y), new Color(255, 0, 0, 20));
        }

        protected override void ActionOnTriangleDraw(SpriteBatch spriteBatch)
        {
            boundingBox.Draw(spriteBatch);

            
            if (listPixelCheck.Count == 0)
            {
                int count = 0;
                for (int i = 0; i < (maximum.X - minimum.X) / Globals.pixelSize; i++)
                {
                    listPixelCheck.Add(new List<bool>());
                    listSquares.Add(new List<Square>());

                    for (int j = 0; j < (maximum.Y - minimum.Y) / Globals.pixelSize; j++)
                    {
                        listPixelCheck[i].Add(false);
                        check = (new Vector2(minimum.X + (i * Globals.pixelSize) + (Globals.pixelSize / 2), minimum.Y + (j * Globals.pixelSize) + (Globals.pixelSize / 2)));
                        bool inTri = IsInTriangle(check);
                        Color col;

                        if (IsInTriangle(check))
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
                        count++;
                    }
                    count = 0;
                }

                animationCounterLimit = (listPixelCheck.Count * listPixelCheck[0].Count);
                bool b = IsInTriangle(check);
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
                        if (count >= animationCounter)
                        {
                            breakNow = true;
                            check = new Vector2(minimum.X + (i * Globals.pixelSize) + (Globals.pixelSize / 2), minimum.Y + (j * Globals.pixelSize) + (Globals.pixelSize / 2));
                            pixelInBox.X = i;
                            pixelInBox.Y = j;
                            break;
                        }

                        listSquares[i][j].Draw(spriteBatch);
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
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point A: " + normalisedTrianglePoints[0], new Vector2(10, y+=20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point B: " + normalisedTrianglePoints[1], new Vector2(10, y += 20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Point C: " + normalisedTrianglePoints[2], new Vector2(10, y += 20), Color.White, Color.Black);

            y += 30;

            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient AB: " + v0, new Vector2(10, y += 20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient BC: " + v1, new Vector2(10, y += 20), Color.White, Color.Black);
            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient CA: " + v2, new Vector2(10, y += 20), Color.White, Color.Black);
        }

        
        protected bool IsInTriangle(Vector2 pointToCheck)
        {
            v0 = orient2d(trianglePoints[0], trianglePoints[1], pointToCheck);
            v1 = orient2d(trianglePoints[1], trianglePoints[2], pointToCheck);
            v2 = orient2d(trianglePoints[2], trianglePoints[0], pointToCheck);

            if (v0 >= 0 && v1 >= 0 && v2 >= 0)
                return true;
            else
                return false;
        }


        private float orient2d(Vector2 a, Vector2 b, Vector2 p) // a = input 1, b = input 2, p = point to check
        {
            return (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
        }

    }
}
