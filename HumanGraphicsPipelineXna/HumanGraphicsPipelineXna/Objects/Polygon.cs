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

using Bitmap = System.Drawing.Bitmap;
using SolidBrush = System.Drawing.SolidBrush;
using Graphics = System.Drawing.Graphics;
using DColour = System.Drawing.Color;
using DPoint = System.Drawing.Point;
using System.IO;


namespace HumanGraphicsPipelineXna
{
    class Polygon
    {
        List<DPoint> points = new List<DPoint>();
        Texture2D tex;
        int miniX;
        int miniY;

        public Polygon(DPoint[] pointsIn, DColour col)
        {
            for (int i = 0; i < pointsIn.Length; i++)
                points.Add(pointsIn[i]);

            int minX=int.MaxValue;
            int minY=int.MaxValue;

            int maxX=0;
            int maxY=0;

            foreach (DPoint val in points)
            {
                if (val.X < minX)
                    minX = val.X;
                else if (val.X > maxX)
                    maxX = val.X;

                if (val.Y < minY)
                    minY = val.Y;
                else if (val.Y > maxY)
                    maxY = val.Y;
            }

            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new DPoint(points[i].X - minX, points[i].Y - minY);
            }

            Bitmap b = new Bitmap(maxX-minX, maxY-minY);
            Graphics g = Graphics.FromImage(b);

            g.FillPolygon(new SolidBrush(col), points.ToArray());

            tex = null;
            using (MemoryStream s = new MemoryStream())
            {
                b.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                s.Seek(0, SeekOrigin.Begin); //must do this, or error is thrown in next line
                tex = Texture2D.FromStream(Globals.graphicsDevice, s);
            }

            miniX = minX;
            miniY = minY;


            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, new Vector2(miniX, miniY), Color.White);
        }
    }
}
