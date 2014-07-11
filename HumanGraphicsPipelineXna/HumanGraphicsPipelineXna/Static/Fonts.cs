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
    class Fonts
    {
        public void Init(){ }
        
        public static SpriteFont smallFont { get; set; }
        public static SpriteFont font14 { get; set; }
        public static SpriteFont arial14 { get; set; }

        /// <summary>
        /// Writes text with a coloured outline around it.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="font">SpriteFont to be used.</param>
        /// <param name="text">Text to be drawn</param>
        /// <param name="position">Position on screen</param>
        /// <param name="textColour">Colour of text</param>
        /// <param name="strokeColour">Colour of outline</param>
        public static void WriteStrokedLine(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color textColour, Color strokeColour)
        {
            Color usingColour = strokeColour;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    spriteBatch.DrawString(font, text, new Vector2(position.X + i, position.Y + j), strokeColour);
                }
            }

            spriteBatch.DrawString(font, text, position, textColour);
        }
    }
}
