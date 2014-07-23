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
    class Barycentric : TriangleRasterisingScene
    {
        public Barycentric()
            : base()
        {
            drawGrid = true;
        }

        protected override void DrawText(SpriteBatch spriteBatch)
        {
            int yPos = Globals.viewportHeight;
            yPos -= 20;

            if (listResults.Count > 0)
            {
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "S+T: " + listResults[(int)pixelInBox.X][(int)pixelInBox.Y][2], new Vector2(10, yPos -= 20), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "T: " + listResults[(int)pixelInBox.X][(int)pixelInBox.Y][1], new Vector2(10, yPos -= 20), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "S: " + listResults[(int)pixelInBox.X][(int)pixelInBox.Y][0], new Vector2(10, yPos -= 20), Color.White, Color.Black);
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
 	         base.Draw(spriteBatch);
             DrawText(spriteBatch);
        }

        protected override bool PerformFillingFunction(Vector2 p, int i, int j)
        {
            if (i == listResults.Count)
                listResults.Add(new List<float[]>());

            Vector2 vs1 = new Vector2(normalisedTrianglePoints[1].X - normalisedTrianglePoints[0].X, normalisedTrianglePoints[1].Y - normalisedTrianglePoints[0].Y);
            Vector2 vs2 = new Vector2(normalisedTrianglePoints[2].X - normalisedTrianglePoints[0].X, normalisedTrianglePoints[2].Y - normalisedTrianglePoints[0].Y);

            Vector2 q = new Vector2(p.X - normalisedTrianglePoints[0].X, p.Y - normalisedTrianglePoints[0].Y);

            float s = (Helper.CrossProduct(q, vs2)) / Helper.CrossProduct(vs1, vs2);
            float t = (Helper.CrossProduct(vs1, q)) / Helper.CrossProduct(vs1, vs2);

            listResults[i].Add(new float[3]);
            listResults[i][j][0] = s;
            listResults[i][j][1] = t;
            listResults[i][j][2] = s+t;

            if (s >= 0 && t >= 0 && s + t <= 1)
                return true;

            return false;
        }

        
    }
}
