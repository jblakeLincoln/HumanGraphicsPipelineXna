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
    class HalfSpace : TriangleScene
    {
        // Halfspace checks
        float v0;
        float v1;
        float v2;

        public HalfSpace()
            : base()
        { 
        
        }

        protected override void DerivedInit()
        {
            base.DerivedInit();
            v0 = 0;
            v1 = 1;
            v2 = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        
        protected override void DrawText(SpriteBatch spriteBatch)
        {


            int yPos = Globals.viewportHeight;

            Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Place points clockwise for backface culling.", new Vector2(Globals.viewportWidth-320, 20), Color.White, Color.Black);
            if (listResults.Count > 0)
            {

                Vector2 previous = new Vector2(pixelInBox.Y, pixelInBox.X);//GetPreviousValue(new Vector2(pixelInBox.Y, pixelInBox.X), listPixelCheck);


                if (listResults[(int)previous.X][(int)previous.Y][0] >= 0 &&
                    listResults[(int)previous.X][(int)previous.Y][1] >= 0 &&
                    listResults[(int)previous.X][(int)previous.Y][2] >= 0)
                {
                    Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Pixel is within triangle.", new Vector2(10, yPos -= 20), Color.White, Color.Black);
                }
                else
                    Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Pixel not within triangle.", new Vector2(10, yPos -= 20), Color.White, Color.Black);


                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient CA: " + listResults[(int)previous.X][(int)previous.Y][2], new Vector2(10, yPos -= 40), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient BC: " + listResults[(int)previous.X][(int)previous.Y][1], new Vector2(10, yPos -= 20), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient AB: " + listResults[(int)previous.X][(int)previous.Y][0], new Vector2(10, yPos -= 20), Color.White, Color.Black);

                
                
            }
             
        }

        // p = point to check.
        protected override bool PerformFillingFunction(Vector2 p, int i, int j)
        {
            if (j == listResults.Count)
                listResults.Add(new List<float[]>());

            v0 = orient2d(normalisedTrianglePoints[0], normalisedTrianglePoints[1], p);
            v1 = orient2d(normalisedTrianglePoints[1], normalisedTrianglePoints[2], p);
            v2 = orient2d(normalisedTrianglePoints[2], normalisedTrianglePoints[0], p);

            listResults[j].Add(new float[3]);
            listResults[j][i][0] = v0;
            listResults[j][i][1] = v1;
            listResults[j][i][2] = v2;

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
