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
    public class Button
    {
        Texture2D tex;
        Vector2 position;
        Vector2 dimensions;
        string text;
        SpriteFont font;

        Vector2 textSize;
        Vector2 textCentre;

        public delegate void ThisOnClick(Button b);
        public event ThisOnClick OnClick;

        public delegate void ThisOnPress(Button b);
        public event ThisOnPress OnPress;

        float thresh = 500;

        TimeSpan pressTimer = TimeSpan.Zero;

        public Button(string s, SpriteFont f, Vector2 dim, Vector2 pos, Color col)
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

            tex = new Texture2D(Globals.graphics.GraphicsDevice, (int)dim.X, (int)dim.Y, false, SurfaceFormat.Color);

            tex.SetData<Color>(pixels);

            textSize = f.MeasureString(s);
            textCentre = new Vector2(Globals.graphics.GraphicsDevice.Viewport.Width / 2, dim.Y);

            font = f;
        }

        /// <summary>
        /// Detects mouse click on release
        /// </summary>
        public bool IsClicked(GameTime gameTime)
        {
            if (new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y).Contains(Inputs.MouseState.X, Inputs.MouseState.Y) &&
                Inputs.MouseState.LeftButton == ButtonState.Pressed)
            {

                pressTimer += gameTime.ElapsedGameTime;
                Console.WriteLine(gameTime.ElapsedGameTime.TotalMilliseconds);
                if (pressTimer.TotalMilliseconds <= gameTime.ElapsedGameTime.TotalMilliseconds)
                    return true;
            }
            else if (pressTimer.TotalMilliseconds > 0 && Inputs.MouseState.LeftButton == ButtonState.Released)
                pressTimer = TimeSpan.Zero;

            return false;
        }

        /// <summary>
        /// Detects continued mouse click (held click)
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            if (OnClick != null && IsClicked(gameTime))
            {
                OnClick(this);
            }
            else if (OnPress != null && pressTimer.TotalMilliseconds > thresh)
            {
                OnPress(this);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y), Color.White);
            spriteBatch.DrawString(font, text, new Vector2((int)position.X + (dimensions.X/2) - (textSize.X/2), (int)position.Y+(dimensions.Y/2) - (textSize.Y/2)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
        }
    }
}
