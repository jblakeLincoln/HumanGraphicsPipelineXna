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
        List<List<bool>> listPixelCheck = new List<List<bool>>();
        List<List<Square>> listSquares = new List<List<Square>>();
        int pixelCount = 0;

        public HalfSpace()
            : base()
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

        int myInt = 0;
        int numPixelsX = 0;
        int numPixelsY = 0;

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawGrid(spriteBatch);

            Vector2 normalisedScreen = Vector2.Normalize(new Vector2(Globals.viewport.X, Globals.viewport.Y));

            for (int i = 0; i < 3; i++)
            {
                if (trianglePoints[i] != Vector2.Zero)
                {
                    DrawSquare(spriteBatch, trianglePoints[i], new Vector2(10, 10), Color.Green);
                    float normalisedX = (trianglePoints[i].X - 0) / (400 - 0) - 0.5f * 2;
                    float normalisedY = (trianglePoints[i].Y - 0) / (240 - 0) - 0.5f * 2;

                    normalisedTrianglePoints[i] = new Vector2(normalisedX, normalisedY);
                    //spriteBatch.DrawString(smallFont, trianglePoint1.X.ToString() + ", " + trianglePoint1.Y.ToString(), new Vector2(trianglePoint1.X-10, trianglePoint1.Y - 15), Color.White);
                    spriteBatch.DrawString(Fonts.smallFont, normalisedTrianglePoints[i].X.ToString() + ", " + normalisedTrianglePoints[i].Y.ToString(), new Vector2(trianglePoints[i].X - 10, trianglePoints[i].Y - 15), Color.White);
                }
            }

            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;

            
            if (trianglePoints[2] != Vector2.Zero)
            {
                DrawLineBetweenTwoPoints(spriteBatch, trianglePoints[0], trianglePoints[1], new Vector2(5, 5), Color.Black);
                DrawLineBetweenTwoPoints(spriteBatch, trianglePoints[1], trianglePoints[2], new Vector2(5, 5), Color.Black);
                DrawLineBetweenTwoPoints(spriteBatch, trianglePoints[0], trianglePoints[2], new Vector2(5, 5), Color.Black);

                minX = Math.Min((int)trianglePoints[0].X, Math.Min((int)trianglePoints[1].X, (int)trianglePoints[2].X));
                minY = Math.Min((int)trianglePoints[0].Y, Math.Min((int)trianglePoints[1].Y, (int)trianglePoints[2].Y));
                maxX = Math.Max((int)trianglePoints[0].X, Math.Max((int)trianglePoints[1].X, (int)trianglePoints[2].X));
                maxY = Math.Max((int)trianglePoints[0].Y, Math.Max((int)trianglePoints[1].Y, (int)trianglePoints[2].Y));

                int modMinX = minX % 20;
                int modMinY = minY % 20;
                int modMaxX = maxX % 20;
                int modMaxY = maxY % 20;

                minX -= modMinX;
                minY -= modMinY;

                maxX -= modMaxX-20-1;
                maxY -= modMaxY-20-1;


                DrawSquare(spriteBatch, new Vector2(minX, minY), new Vector2(maxX - minX, maxY - minY), new Color(255, 0, 0, 20));

                if (listPixelCheck.Count == 0)
                {
                    for (int i = 0; i < (maxX - minX) / 20; i++)
                    {
                        listPixelCheck.Add(new List<bool>());
                        listSquares.Add(new List<Square>());
                        for (int j = 0; j < (maxY - minY) / 20; j++)
                        {
                            listPixelCheck[i].Add(false);
                            listSquares[i].Add(new Square(new Vector2(minX + (i * 20), minY + (j * 20)), new Vector2(20, 20), new Color(0, 255, 0, 20)));
                            numPixelsX++;
                        }

                        numPixelsY++;
                        numPixelsX=0;
                    }
                }
            }
            
            Vector2 checkPoint = Vector2.Zero;
            int tempi = 0;
            int tempj = 0;
            if (state == State.Animate)
            {
                bool breakNow = false;
                int count = 0;

                for (int j = 0; j < listPixelCheck[0].Count; j++)
                {
                    for (int i = 0; i < listPixelCheck.Count; i++)
                    {
                    

                        if (count >= pixelCount)
                        {
                            breakNow = true;
                            checkPoint = new Vector2(minX + (i*20)+10, minY+(j*20)+10);
                            tempi = i;
                            tempj = j;
                            break;
                        }

                        if (listPixelCheck[i][j])
                            listSquares[i][j].Draw(spriteBatch);

                        count++;
                        
                    }
                    if (breakNow)
                        break;
                }

                if (buttonNext.IsClicked())
                {
                    pixelCount++;

                    

                    float v0 = orient2d(trianglePoints[0], trianglePoints[1], checkPoint);
                    float v1 = orient2d(trianglePoints[1], trianglePoints[2], checkPoint);
                    float v2 = orient2d(trianglePoints[2], trianglePoints[0], checkPoint);

                    if (v0 >= 0 && v1 >= 0 && v2 >= 0)
                        listPixelCheck[tempi][tempj] = true;
                    else
                        listPixelCheck[tempi][tempj] = false;

                    float normalisedX = (checkPoint.X - 0) / (400 - 0) - 0.5f * 2;
                    float normalisedY = (checkPoint.Y - 0) / (240 - 0) - 0.5f * 2;
                    checkPoint = new Vector2(normalisedX, normalisedY);

                    v0 = orient2d(normalisedTrianglePoints[0], normalisedTrianglePoints[1], checkPoint);
                    v1 = orient2d(normalisedTrianglePoints[1], normalisedTrianglePoints[2], checkPoint);
                    v2 = orient2d(normalisedTrianglePoints[2], normalisedTrianglePoints[0], checkPoint);

                    if (v0 >= 0 && v1 >= 0 && v2 >= 0)
                        listPixelCheck[tempi][tempj] = true;
                    else
                        listPixelCheck[tempi][tempj] = false;
                }

                
                spriteBatch.DrawString(Fonts.font14, pixelCount.ToString(), new Vector2(50, 50), Color.White);
            }

            base.Draw(spriteBatch);
        }


        private float orient2d(Vector2 a, Vector2 b, Vector2 p) // a = input 1, b = input 2, p = point to check
        {
            return (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
        }

    }
}
