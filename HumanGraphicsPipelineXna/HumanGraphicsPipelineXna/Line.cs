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
    class Line
    {
        Texture2D pixel;
        Vector2 p1;
        Vector2 p2;
        float length;
        float angle;
        Color color;
        float thickness;
        public Line(Vector2 point1, Vector2 point2, Color col, float thicknessIn)
        {
            p1 = point1;
            p2 = point2;
            color = col;
            thickness = thicknessIn;

            pixel = new Texture2D(Globals.graphicsDevice, 1, 1);
            pixel.SetData(new Color[] { color });

            Vector2 direction = p2 - p1;
            length = direction.Length();
            angle = (float)Math.Atan2(direction.Y, direction.X);
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pixel, new Vector2(p1.X, p1.Y), null, color, angle, new Vector2(0.0f, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 1.0f);
        }
    }
}
