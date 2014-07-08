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
using Drawing = System.Drawing;

namespace HumanGraphicsPipelineXna
{
    class Button
    {
        Texture2D tex;
        Vector2 position;
        Vector2 dimensions;
        string text;
        SpriteFont font;

        Vector2 textSize;
        Vector2 textCentre;
        public Button(GraphicsDeviceManager g, string s, SpriteFont f, Vector2 dim, Vector2 pos, Color col)
        {
            text = s;
            dimensions = new Vector2(dim.X, dim.Y);
            Color[] pixels = new Color[(int)dim.X * (int)dim.Y];
            for (int i = 0; i < dim.X; i++)
            {
                for (int j = 0; j < dim.Y; j++)
                {
                    pixels[i * (int)dim.Y + j] = new Color(col.R, col.G, col.B, col.A);
                }
            }
            position = pos;

            tex = new Texture2D(g.GraphicsDevice, (int)dim.X, (int)dim.Y, false, SurfaceFormat.Color);

            tex.SetData<Color>(pixels);

            textSize = f.MeasureString(s);
            textCentre = new Vector2(g.GraphicsDevice.Viewport.Width / 2, dim.Y);

            font = f;
        }

        public bool IsClicked(MouseState mState, MouseState lState)
        {
            if (new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y).Contains(mState.X, mState.Y) &&
                mState.LeftButton == ButtonState.Released && lState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y), Color.White);
            spriteBatch.DrawString(font, text, new Vector2((int)position.X + (dimensions.X/2) - (textSize.X/2), (int)position.Y+(dimensions.Y/2) - (textSize.Y/2)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
        }
    }
}
