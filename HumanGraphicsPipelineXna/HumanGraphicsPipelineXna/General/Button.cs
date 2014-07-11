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

        private float thresh = 500;
        private string text;

        private Vector2 position;
        private Vector2 dimensions;
        private Vector2 textSize;
        private Vector2 textCentre;
        private SpriteFont font;
        private Texture2D tex;
        private TimeSpan pressTimer = TimeSpan.Zero;

        public delegate void ThisOnClick(Button b);
        public event ThisOnClick OnClick;
        public delegate void ThisOnHold(Button b);
        public event ThisOnHold OnHold;

        public Button(string s, SpriteFont f, Vector2 dim, Vector2 pos, Color col)
        {
            text = s;
            dimensions = new Vector2(dim.X, dim.Y);
            position = pos;
            SetColour(col);
            textSize = f.MeasureString(s);
            textCentre = new Vector2(Globals.graphics.GraphicsDevice.Viewport.Width / 2, dim.Y);
            font = f;
        }

        public void EmulateClick()
        {
            if (OnClick != null)
                OnClick(this);
        }

        public void SetColour(Color col)
        {
            Color[] pixels = new Color[1];
            pixels[0] = new Color(col.R, col.G, col.B, col.A);
            tex = new Texture2D(Globals.graphics.GraphicsDevice, 1, 1);
            tex.SetData<Color>(pixels);
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
                OnClick(this);
            else if (OnHold != null && pressTimer.TotalMilliseconds > thresh)
                OnHold(this);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y), Color.White);
            spriteBatch.DrawString(font, text, new Vector2((int)position.X + (dimensions.X/2) - (textSize.X/2), (int)position.Y+(dimensions.Y/2) - (textSize.Y/2)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
        }
    }
}
