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
    class TriangleClippingScene : ClippingScene
    {
        protected Vector2[] trianglePoints;
        protected Vector2[] normalisedTrianglePoints;
        protected Square[] triangleSquares;
        protected Line[] triangleLines;


        protected override void DerivedInit()
        {
            trianglePoints = new Vector2[3];
            normalisedTrianglePoints = new Vector2[3];
            triangleSquares = new Square[3];
            triangleLines = new Line[3]; //AB, BC, CA
        }
        protected override void StateChanges(Microsoft.Xna.Framework.GameTime gameTime)
        {

            if (Inputs.MouseState.LeftButton == ButtonState.Released && Inputs.MouseStatePrevious.LeftButton == ButtonState.Pressed)
            {
                if (state < 3)
                {
                    trianglePoints[(int)state] = new Vector2(Inputs.MouseState.X, Inputs.MouseState.Y);
                    triangleSquares[(int)state] = new Square(new Vector2(Inputs.MouseState.X - 5, Inputs.MouseState.Y - 5), new Vector2(10, 10), Color.Green);
                    state++;
                }
                if (state == 3)
                {
                    state = 42;
                    LastPointPlaced(gameTime);
                    triangleLines[0] = new Line(trianglePoints[0], trianglePoints[1], Color.Black, 1);
                    triangleLines[1] = new Line(trianglePoints[1], trianglePoints[2], Color.Black, 1);
                    triangleLines[2] = new Line(trianglePoints[2], trianglePoints[0], Color.Black, 1);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Top horizontal
            spriteBatch.Draw(gridLine, new Rectangle(Globals.viewportWidth / 6, Globals.viewportHeight / 6, (Globals.viewportWidth / 2) + (Globals.viewportWidth / 6), 1), Color.White);
            // Bottom horizontal
            spriteBatch.Draw(gridLine, new Rectangle(Globals.viewportWidth / 6, Globals.viewportHeight - (Globals.viewportHeight / 6), (Globals.viewportWidth / 2) + (Globals.viewportWidth / 6), 1), Color.White);
            // Left vertical
            spriteBatch.Draw(gridLine, new Rectangle(Globals.viewportWidth / 6, Globals.viewportHeight / 6, 1, (Globals.viewportHeight / 2) + (Globals.viewportHeight / 6)), Color.White);
            // Right vertical
            spriteBatch.Draw(gridLine, new Rectangle(Globals.viewportWidth - (Globals.viewportWidth / 6), Globals.viewportHeight / 6, 1, (Globals.viewportHeight / 2) + (Globals.viewportHeight / 6)), Color.White);
        }
    }
}
