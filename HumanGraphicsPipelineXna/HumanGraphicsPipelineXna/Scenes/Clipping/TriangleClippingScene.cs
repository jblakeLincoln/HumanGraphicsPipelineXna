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
using System.Drawing;

using XColour = Microsoft.Xna.Framework.Color;
using DColour = System.Drawing.Color;
using DPoint = System.Drawing.Point;
using DRectangle = System.Drawing.Rectangle;
using System.IO;

namespace HumanGraphicsPipelineXna
{
    class TriangleClippingScene : TriangleScene
    {
        static Vector2 pointTopLeft = new Vector2(Globals.viewportWidth / 6, Globals.viewportHeight / 6);
        static Vector2 pointTopRight = new Vector2(Globals.viewportWidth - (Globals.viewportWidth / 6), Globals.viewportHeight / 6);
        static Vector2 pointBottomLeft = new Vector2(Globals.viewportWidth / 6, Globals.viewportHeight - (Globals.viewportHeight / 6));
        static Vector2 pointBottomRight = new Vector2(Globals.viewportWidth - (Globals.viewportWidth / 6), Globals.viewportHeight - (Globals.viewportHeight / 6));

        Vector2 intersectionPointAB1;
        Vector2 intersectionPointBC1;
        Vector2 intersectionPointAC1;
        Vector2 intersectionPointAB2;
        Vector2 intersectionPointBC2;
        Vector2 intersectionPointAC2;

        Line lineTop = new Line(pointTopLeft, pointTopRight, XColour.Black, 1f);
        Line lineLeft = new Line(pointTopLeft, pointBottomLeft, XColour.Black, 1f);
        Line lineBottom = new Line(pointBottomLeft, pointBottomRight, XColour.Black, 1f);
        Line lineRight = new Line(pointTopRight, pointBottomRight, XColour.Black, 1f);

        List<Vector2> l = new List<Vector2>() {
            pointTopLeft, pointTopRight, // Top
            pointTopLeft, pointBottomLeft, // Left
            pointBottomLeft, pointBottomRight, // Bottom
            pointTopRight, pointBottomRight}; // Right

        protected override void DerivedInit()
        {
            base.DerivedInit();

            trianglePoints = new Vector2[3];
            normalisedTrianglePoints = new Vector2[3];
            triangleSquares = new Square[3];
            triangleLines = new Line[3]; //AB, BC, CA

            intersectionAB1 = false;
            intersectionBC1 = false;
            intersectionAC1 = false;
            intersectionAB2 = false;
            intersectionBC2 = false;
            intersectionAC2 = false;

            intersectionPointAB1 = new Vector2(float.NegativeInfinity);
            intersectionPointBC1 = new Vector2(float.NegativeInfinity);
            intersectionPointAC1 = new Vector2(float.NegativeInfinity);
            intersectionPointAB2 = new Vector2(float.NegativeInfinity);
            intersectionPointBC2 = new Vector2(float.NegativeInfinity);
            intersectionPointAC2 = new Vector2(float.NegativeInfinity);

        }

        protected override void StateChanges(GameTime gameTime)
        {
            base.StateChanges(gameTime);
            if (Inputs.MouseState.LeftButton == ButtonState.Released && Inputs.MouseStatePrevious.LeftButton == ButtonState.Pressed)
            {
                if (state <= 3)
                {
                    CorrectNormalisedTriangle(state);
                }
            }
        }

        private void CorrectNormalisedTriangle(int state)
        {
            float aX = normalisedTrianglePoints[(int)state - 1].X;
            float bX = normalisedTrianglePoints[(int)state - 1].X / 2;
            float pX = aX + bX;

            float aY = normalisedTrianglePoints[(int)state - 1].Y;
            float bY = normalisedTrianglePoints[(int)state - 1].Y / 2;
            float pY = aY + bY;
            normalisedTrianglePoints[(int)state - 1] = new Vector2(pX, pY);
        }
        

        private bool CheckLineIntersection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersectionPoint)
        {
            intersectionPoint = new Vector2(float.NegativeInfinity);
		    double xD1 = p2.X-p1.X;
		    double xD2 = q2.X-q1.X;
		    double yD1 = p2.Y-p1.Y;
		    double yD2 = q2.Y-q1.Y;
		    double xD3 = p1.X-q1.X;
		    double yD3 = p1.Y-q1.Y;

		    double len1 = Math.Sqrt(xD1*xD1+yD1*yD1);
		    double len2 = Math.Sqrt(xD2*xD2+yD2*yD2);
            
		    double dot = xD1*xD2+yD1*yD2;
		    double deg = dot/(len1*len2);

		    double div = yD2*xD1-xD2*yD1;
		    double ua = (xD2*yD3-yD2*xD3)/div;
		    double ub = (xD1*yD3-yD1*xD3)/div;

            Vector2 pt = new Vector2((float)(p1.X + ua * xD1), (float)(p1.Y + ua * yD1));

		    xD1=pt.X-p1.X;  
		    xD2=pt.X-p2.X;  
		    yD1=pt.Y-p1.Y;  
		    yD2=pt.Y-p2.Y;  

		    double segmentLength1 = Math.Sqrt(xD1*xD1+yD1*yD1)+Math.Sqrt(xD2*xD2+yD2*yD2);  
            
		    xD1=pt.X-q1.X;  
		    xD2=pt.X-q2.X;  
		    yD1=pt.Y-q1.Y;  
		    yD2=pt.Y-q2.Y; 

		    double segmentLength2 = Math.Sqrt(xD1*xD1+yD1*yD1)+Math.Sqrt(xD2*xD2+yD2*yD2); 

		    if(Math.Abs(len1-segmentLength1)>0.01 || Math.Abs(len2-segmentLength2)>0.01)
			    return false;

		    if ((Math.Round(pt.X) == Math.Round(p1.X) && Math.Round(pt.Y) == Math.Round(p1.Y)) || (Math.Round(pt.X) == Math.Round(p2.X) && Math.Round(pt.Y) == Math.Round(p2.Y)))
			    return false;

		    if (pt.X/2 != pt.X/2 || pt.Y/2 != pt.Y/2)
			    return false;

            intersectionPoint = pt;

		    return true;
        }

        bool intersectionAB1;
        bool intersectionBC1;
        bool intersectionAC1;
        bool intersectionAB2;
        bool intersectionBC2;
        bool intersectionAC2;

        Square squareIntersectionAB1;
        Square squareIntersectionBC1;
        Square squareIntersectionAC1;
        Square squareIntersectionAB2;
        Square squareIntersectionBC2;
        Square squareIntersectionAC2;


        protected override void LastPointPlaced(GameTime gameTime)
        {
            CorrectNormalisedTriangle(3);
            for (int i = 0; i < 8; i+=2)
            {
                if (!intersectionAB1)
                    intersectionAB1 = CheckLineIntersection(trianglePoints[0], trianglePoints[1], l[i], l[i+1], out intersectionPointAB1);
                else if (!intersectionAB2)
                    intersectionAB2 = CheckLineIntersection(trianglePoints[0], trianglePoints[1], l[i], l[i + 1], out intersectionPointAB2);

                if (!intersectionBC1)
                    intersectionBC1 = CheckLineIntersection(trianglePoints[1], trianglePoints[2], l[i], l[i + 1], out intersectionPointBC1);
                else if (!intersectionBC2)
                    intersectionBC2 = CheckLineIntersection(trianglePoints[1], trianglePoints[2], l[i], l[i + 1], out intersectionPointBC2);

                if (!intersectionAC1)
                    intersectionAC1 = CheckLineIntersection(trianglePoints[2], trianglePoints[0], l[i], l[i + 1], out intersectionPointAC1);
                else if (!intersectionAC2)
                    intersectionAC2 = CheckLineIntersection(trianglePoints[2], trianglePoints[0], l[i], l[i + 1], out intersectionPointAC2);

            }

            squareIntersectionAB1 = new Square(new Vector2(intersectionPointAB1.X - 2, intersectionPointAB1.Y - 2), new Vector2(4, 4), XColour.Red);
            squareIntersectionBC1 = new Square(new Vector2(intersectionPointBC1.X - 2, intersectionPointBC1.Y - 2), new Vector2(4, 4), XColour.Red);
            squareIntersectionAC1 = new Square(new Vector2(intersectionPointAC1.X - 2, intersectionPointAC1.Y - 2), new Vector2(4, 4), XColour.Red);
            squareIntersectionAB2 = new Square(new Vector2(intersectionPointAB2.X - 2, intersectionPointAB2.Y - 2), new Vector2(4, 4), XColour.Blue);
            squareIntersectionBC2 = new Square(new Vector2(intersectionPointBC2.X - 2, intersectionPointBC2.Y - 2), new Vector2(4, 4), XColour.Blue);
            squareIntersectionAC2 = new Square(new Vector2(intersectionPointAC2.X - 2, intersectionPointAC2.Y - 2), new Vector2(4, 4), XColour.Blue);
        }

        protected override void DrawOnAnimate(SpriteBatch spriteBatch)
        {
           // throw new NotImplementedException();
        }
        
        protected override void ActionOnTrianglePlaced(SpriteBatch spriteBatch)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            lineTop.Draw(spriteBatch);
            lineLeft.Draw(spriteBatch);
            lineBottom.Draw(spriteBatch);
            lineRight.Draw(spriteBatch);


            if (squareIntersectionAB1 != null)
                squareIntersectionAB1.Draw(spriteBatch);
            if (squareIntersectionBC1 != null)
                squareIntersectionBC1.Draw(spriteBatch);
            if (squareIntersectionAC1 != null)
                squareIntersectionAC1.Draw(spriteBatch);
            if (squareIntersectionAB2 != null)
                squareIntersectionAB2.Draw(spriteBatch);
            if (squareIntersectionBC2 != null)
                squareIntersectionBC2.Draw(spriteBatch);
            if (squareIntersectionAC2 != null)
                squareIntersectionAC2.Draw(spriteBatch);


            if (intersectionAB1 && intersectionAB2 && intersectionBC1)
            {                
                /*
                System.Drawing.Point[] p = { 
                                         new System.Drawing.Point((int)intersectionPointAB1.X, (int)intersectionPointAB1.Y),

                                         new System.Drawing.Point((int)intersectionPointAB2.X, (int)intersectionPointAB2.Y),

                                         new System.Drawing.Point((int)intersectionPointBC1.X, (int)intersectionPointBC1.Y),
                                         };

                Polygon poly = new Polygon(p, DColour.Red);
                poly.Draw(spriteBatch);
                 * */
                
            }
        }
    }
}
