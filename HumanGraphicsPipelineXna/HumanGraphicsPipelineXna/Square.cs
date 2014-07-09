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
    class Square
    {
        Texture2D tex;
        Color[] pixels = {Color.Black};

        public Vector2 position { get;private set;}
        Vector2 size;
        public Square(Vector2 pos, Vector2 sizeIn, Color col)
        {

            pixels[0] = col;

            position = pos;
            size = sizeIn;
            tex = new Texture2D(Globals.graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            tex.SetData<Color>(pixels);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), pixels[0]);
        }
    }
}
