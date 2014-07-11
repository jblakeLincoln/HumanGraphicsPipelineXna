using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HumanGraphicsPipelineXna
{
    class Square
    {
        public Vector2 position { get; private set; }
        private Texture2D tex;
        private Color[] pixels = {Color.Black};
        private Vector2 size;

        public Square(Vector2 pos, Vector2 sizeIn, Color col)
        {
            pixels[0] = col;
            position = pos;
            size = sizeIn;
            tex = new Texture2D(Globals.graphicsDevice, 1, 1, true, SurfaceFormat.Color);
            tex.SetData<Color>(pixels);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), pixels[0]);
        }
    }
}
