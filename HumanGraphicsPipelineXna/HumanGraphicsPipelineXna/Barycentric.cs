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
    class Barycentric : TriangleScene
    {
        public Barycentric()
            : base()
        { 
        
        }


        

        protected override void DrawText(SpriteBatch spriteBatch)
        {
            int yPos = Globals.viewportHeight;
            yPos -= 30;

            if (listResults.Count > 0)
            {

                Vector2 previous = new Vector2(pixelInBox.Y, pixelInBox.X);// GetPreviousValue(pixelInBox, listPixelCheck);
                Console.WriteLine("Pixel: " + pixelInBox);
                Console.WriteLine("PPP: " + previousPixelInBox);

                if (listResults[(int)previous.X][(int)previous.Y][0] >= 0 && listResults[(int)previous.X][(int)previous.Y][1] >= 0 && listResults[(int)previous.X][(int)previous.Y][2] <= 1)
                {
                    Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Pixel is within triangle.", new Vector2(10, yPos -= 20), Color.White, Color.Black);
                }
                else
                    Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Pixel not within triangle.", new Vector2(10, yPos -= 20), Color.White, Color.Black);

                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "S+T: " + listResults[(int)previous.X][(int)previous.Y][2], new Vector2(10, yPos -= 20), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "T: " + listResults[(int)previous.X][(int)previous.Y][1], new Vector2(10, yPos -= 20), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "S: " + listResults[(int)previous.X][(int)previous.Y][0], new Vector2(10, yPos -= 40), Color.White, Color.Black);
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
 	         base.Draw(spriteBatch);
             DrawText(spriteBatch);
        }



        protected override bool PerformFillingFunction(Vector2 p, int i, int j)
        {
            if (j == listResults.Count)
                listResults.Add(new List<float[]>());

            Vector2 vs1 = new Vector2(normalisedTrianglePoints[1].X - normalisedTrianglePoints[0].X, normalisedTrianglePoints[1].Y - normalisedTrianglePoints[0].Y);
            Vector2 vs2 = new Vector2(normalisedTrianglePoints[2].X - normalisedTrianglePoints[0].X, normalisedTrianglePoints[2].Y - normalisedTrianglePoints[0].Y);

            Vector2 q = new Vector2(p.X - normalisedTrianglePoints[0].X, p.Y - normalisedTrianglePoints[0].Y);

            float s = (CrossProduct(q,vs2)) / CrossProduct(vs1, vs2);
            float t = (CrossProduct(vs1, q)) / CrossProduct(vs1, vs2);

            listResults[j].Add(new float[3]);
            listResults[j][i][0] = s;
            listResults[j][i][1] = t;
            listResults[j][i][2] = s+t;

            if (s >= 0 && t >= 0 && s + t <= 1)
                return true;

            return false;
        }

        
    }
}
