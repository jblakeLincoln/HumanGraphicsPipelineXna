using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HumanGraphicsPipelineXna
{
    class HalfSpace : TriangleScene
    {
        // Halfspace checks
        private float v0;
        private float v1;
        private float v2;

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
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient CA: " + listResults[(int)pixelInBox.X][(int)pixelInBox.Y][2], new Vector2(10, yPos -= 40), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient BC: " + listResults[(int)pixelInBox.X][(int)pixelInBox.Y][1], new Vector2(10, yPos -= 20), Color.White, Color.Black);
                Fonts.WriteStrokedLine(spriteBatch, Fonts.arial14, "Orient AB: " + listResults[(int)pixelInBox.X][(int)pixelInBox.Y][0], new Vector2(10, yPos -= 20), Color.White, Color.Black);
            }
        }

        // p = point to check.
        protected override bool PerformFillingFunction(Vector2 p, int i, int j)
        {
            if (i == listResults.Count)
                listResults.Add(new List<float[]>());

            v0 = orient2d(normalisedTrianglePoints[0], normalisedTrianglePoints[1], p);
            v1 = orient2d(normalisedTrianglePoints[1], normalisedTrianglePoints[2], p);
            v2 = orient2d(normalisedTrianglePoints[2], normalisedTrianglePoints[0], p);

            listResults[i].Add(new float[3]);
            listResults[i][j][0] = v0;
            listResults[i][j][1] = v1;
            listResults[i][j][2] = v2;

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
