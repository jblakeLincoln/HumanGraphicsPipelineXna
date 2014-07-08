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
    class HalfSpace : Scene
    {

        enum State
        { 
            PickPoint1,
            PickPoint2,
            PickPoint3,
            Animate,
        }

        State state = State.PickPoint1;

        public HalfSpace(GraphicsDevice g, SpriteFont f)
            : base(g, f)
        { 
        
        }

        public override void Update(GameTime gameTime)
        {
            if (Inputs.MouseState.LeftButton == ButtonState.Released && Inputs.MouseStatePrevious.LeftButton == ButtonState.Pressed)
            {
                if (state == State.PickPoint1)
                {
                    trianglePoints[0] = new Vector2(Inputs.MouseState.X, Inputs.MouseState.Y);
                    state = State.PickPoint2;
                }
                else if (state == State.PickPoint2)
                {
                    trianglePoints[1] = new Vector2(Inputs.MouseState.X, Inputs.MouseState.Y);
                    state = State.PickPoint3;
                }
                else if (state == State.PickPoint3)
                {
                    trianglePoints[2] = new Vector2(Inputs.MouseState.X, Inputs.MouseState.Y);
                    state = State.Animate;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawGrid(spriteBatch);

            Vector2 normalisedScreen = Vector2.Normalize(new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height));

            for (int i = 0; i < 3; i++)
            {
                if (trianglePoints[i] != Vector2.Zero)
                {
                    DrawSquare(spriteBatch, trianglePoints[i], new Vector2(10, 10), Color.Green);
                    float normalisedX = (trianglePoints[i].X - 0) / (400 - 0) - 0.5f * 2;
                    float normalisedY = (trianglePoints[i].Y - 0) / (240 - 0) - 0.5f * 2;

                    normalisedTrianglePoints[i] = new Vector2(normalisedX, normalisedY);
                    //spriteBatch.DrawString(smallFont, trianglePoint1.X.ToString() + ", " + trianglePoint1.Y.ToString(), new Vector2(trianglePoint1.X-10, trianglePoint1.Y - 15), Color.White);
                    spriteBatch.DrawString(smallFont, normalisedTrianglePoints[i].X.ToString() + ", " + normalisedTrianglePoints[i].Y.ToString(), new Vector2(trianglePoints[i].X - 10, trianglePoints[i].Y - 15), Color.White);
                }
            }
            if (trianglePoints[2] != Vector2.Zero)
            {
                DrawLineBetweenTwoPoints(spriteBatch, trianglePoints[0], trianglePoints[1], new Vector2(5, 5), Color.Black);
                DrawLineBetweenTwoPoints(spriteBatch, trianglePoints[1], trianglePoints[2], new Vector2(5, 5), Color.Black);
                DrawLineBetweenTwoPoints(spriteBatch, trianglePoints[0], trianglePoints[2], new Vector2(5, 5), Color.Black);

                int minX = Math.Min((int)trianglePoints[0].X, Math.Min((int)trianglePoints[1].X, (int)trianglePoints[2].X));
                int minY = Math.Min((int)trianglePoints[0].Y, Math.Min((int)trianglePoints[1].Y, (int)trianglePoints[2].Y));
                int maxX = Math.Max((int)trianglePoints[0].X, Math.Max((int)trianglePoints[1].X, (int)trianglePoints[2].X));
                int maxY = Math.Max((int)trianglePoints[0].Y, Math.Max((int)trianglePoints[1].Y, (int)trianglePoints[2].Y));

                DrawSquare(spriteBatch, new Vector2(minX + 5, minY + 5), new Vector2(maxX - minX, maxY - minY), new Color(255, 0, 0, 50));
            }
        }


    }
}
