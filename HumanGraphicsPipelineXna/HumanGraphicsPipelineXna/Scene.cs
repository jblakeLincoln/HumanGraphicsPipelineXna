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
    class Scene
    {
        protected Vector2[] trianglePoints = new Vector2[3];
        protected Vector2[] normalisedTrianglePoints = new Vector2[3];

        protected Button buttonNext;

        protected enum State
        {
            PickPoint1,
            PickPoint2,
            PickPoint3,
            Animate,
        }

        protected State state = State.PickPoint1;

        public Scene()
        {
            buttonNext = new Button("Next", Fonts.font14, new Vector2(30, 30), new Vector2(Globals.viewport.X - 40, Globals.viewport.Y - 40), Color.DarkOliveGreen);
        }

        protected void DrawGrid(SpriteBatch spriteBatch)
        {
            Texture2D gridLine = new Texture2D(Globals.graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] pixels = new Color[1];
            for (int i = 0; i < 1; i++)
                pixels[i] = new Color(0, 0, 0, 100);

            gridLine.SetData<Color>(pixels);

            for (int i = 0; i <= Globals.viewportWidth/2; i++)
            {
                spriteBatch.Draw(gridLine, new Rectangle(0, i * ((int)Globals.viewportHeight / 24), (int)Globals.viewportWidth, 1), Color.White);
                //DrawLineBetweenTwoPoints(spriteBatch, new Vector2(0, i * (GraphicsDevice.Viewport.Height / 24)), new Vector2(GraphicsDevice.Viewport.Width, i * (GraphicsDevice.Viewport.Height / 24)), Vector2.Zero,Color.Black);
            }

            for (int i = 0; i <= Globals.viewportHeight/2; i++)
            {
                spriteBatch.Draw(gridLine, new Rectangle(i * ((int)Globals.viewportWidth / 40), 0, 1, (int)Globals.viewportHeight), Color.White);
                //DrawLineBetweenTwoPoints(spriteBatch, new Vector2(i * (GraphicsDevice.Viewport.Height / 24), 0), new Vector2(i * (GraphicsDevice.Viewport.Height / 24), GraphicsDevice.Viewport.Height), Vector2.Zero, Color.Black);
            }

            gridLine = new Texture2D(Globals.graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixels[0] = new Color(0, 0, 0, 255);

            spriteBatch.Draw(gridLine, new Rectangle(Globals.viewportWidth / 2 - 2, 0, 4, Globals.viewportHeight), Color.White);
            spriteBatch.Draw(gridLine, new Rectangle(0, Globals.viewportHeight / 2 - 2, (Globals.viewportWidth), 4), Color.White);

            gridLine.SetData<Color>(pixels);
        }

        public virtual void Update(GameTime gameTime)
        { 
        
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            if (state == State.Animate)
                buttonNext.Draw(spriteBatch);
        }

        protected void DrawSquare(SpriteBatch spriteBatch, Vector2 pos, Vector2 size, Color col)
        {
            Texture2D square = new Texture2D(Globals.graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] pixels = new Color[1];
            for (int i = 0; i < 1; i++)
                pixels[i] = col;
            square.SetData<Color>(pixels);

            spriteBatch.Draw(square, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), Color.White);

        }

        protected void DrawLineBetweenTwoPoints(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2, Vector2 offset, Color col, float thickness = 1f)
        {
            Texture2D pixel = new Texture2D(Globals.graphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            Vector2 direction = p2 - p1;
            float length = direction.Length();
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            spriteBatch.Draw(pixel, new Vector2(p1.X + offset.X, p1.Y + offset.Y), null, col, angle, new Vector2(0.0f, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 1.0f);
        }
    }

    
}
