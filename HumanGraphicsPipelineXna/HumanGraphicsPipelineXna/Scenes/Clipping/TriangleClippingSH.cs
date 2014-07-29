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
    class TriangleClippingSH : TriangleScene
    {
        class ClippingPoint
        {
            public Vector2 triPoint;
            public List<Vector2> intersectionPointsTo = new List<Vector2>();
            public List<Vector2> intersectionPointsFrom = new List<Vector2>();
            public bool isOutside = false;

            void setIsOutside(bool t) { isOutside = t; }

            public ClippingPoint(Vector2 triPointIn, Vector2 normalisedPointIn, List<Vector2> intersectionPointsToIn, List<Vector2> intersectionPointsFromIn)
            {
                intersectionPointsTo = intersectionPointsToIn;
                intersectionPointsFrom = intersectionPointsFromIn;
                triPoint = triPointIn;
            }


        }

        static Vector2 pointTopLeft = new Vector2(Globals.viewportWidth / 6, Globals.viewportHeight / 6);
        static Vector2 pointTopRight = new Vector2(Globals.viewportWidth - (Globals.viewportWidth / 6), Globals.viewportHeight / 6);
        static Vector2 pointBottomLeft = new Vector2(Globals.viewportWidth / 6, Globals.viewportHeight - (Globals.viewportHeight / 6));
        static Vector2 pointBottomRight = new Vector2(Globals.viewportWidth - (Globals.viewportWidth / 6), Globals.viewportHeight - (Globals.viewportHeight / 6));


        Line lineTop = new Line(pointTopLeft, pointTopRight, XColour.Black, 1f);
        Line lineLeft = new Line(pointTopLeft, pointBottomLeft, XColour.Black, 1f);
        Line lineBottom = new Line(pointBottomLeft, pointBottomRight, XColour.Black, 1f);
        Line lineRight = new Line(pointTopRight, pointBottomRight, XColour.Black, 1f);

        List<Vector2> l = new List<Vector2>() {
            pointTopLeft, pointTopRight, // Top
            pointBottomLeft, pointTopLeft, // Left
            pointBottomRight, pointBottomLeft, // Bottom
            pointTopRight, pointBottomRight}; // Right


        ClippingPoint clippingA;
        ClippingPoint clippingB;
        ClippingPoint clippingC;

        Polygon polygonOutput;

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

        protected override void DerivedInit()
        {
            base.DerivedInit();

            squareList = new List<Square>();
            polygonOutput = null;
        }

        private bool CheckLineIntersection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersectionPoint)
        {
            intersectionPoint = new Vector2(float.NegativeInfinity);
            double xD1 = p2.X - p1.X;
            double xD2 = q2.X - q1.X;
            double yD1 = p2.Y - p1.Y;
            double yD2 = q2.Y - q1.Y;
            double xD3 = p1.X - q1.X;
            double yD3 = p1.Y - q1.Y;

            double len1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1);
            double len2 = Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            double dot = xD1 * xD2 + yD1 * yD2;
            double deg = dot / (len1 * len2);

            double div = yD2 * xD1 - xD2 * yD1;
            double ua = (xD2 * yD3 - yD2 * xD3) / div;
            double ub = (xD1 * yD3 - yD1 * xD3) / div;

            Vector2 pt = new Vector2((float)(p1.X + ua * xD1), (float)(p1.Y + ua * yD1));

            xD1 = pt.X - p1.X;
            xD2 = pt.X - p2.X;
            yD1 = pt.Y - p1.Y;
            yD2 = pt.Y - p2.Y;

            double segmentLength1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            xD1 = pt.X - q1.X;
            xD2 = pt.X - q2.X;
            yD1 = pt.Y - q1.Y;
            yD2 = pt.Y - q2.Y;

            double segmentLength2 = Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            if (Math.Abs(len1 - segmentLength1) > 0.01 || Math.Abs(len2 - segmentLength2) > 0.01)
                return false;

            if ((Math.Round(pt.X) == Math.Round(p1.X) && Math.Round(pt.Y) == Math.Round(p1.Y)) || (Math.Round(pt.X) == Math.Round(p2.X) && Math.Round(pt.Y) == Math.Round(p2.Y)))
                return false;

            if (pt.X / 2 != pt.X / 2 || pt.Y / 2 != pt.Y / 2)
                return false;

            intersectionPoint = pt;

            return true;
        }


        bool isOutsideA = false;
        bool isOutsideB = false;
        bool isOutsideC = false;
        int outsideCount = 0;

        List<ClippingPoint> insideTriPoints;
        List<ClippingPoint> outsideTriPoints;
        List<Vector2> intersectionsC;
        List<Vector2> intersectionsA;
        List<Vector2> intersectionsB;


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


        protected override void LastPointPlaced(GameTime gameTime)
        {
            intersectionsC = new List<Vector2>();
            intersectionsA = new List<Vector2>();
            intersectionsB = new List<Vector2>();
            CorrectNormalisedTriangle(3);

            isOutsideA = (Math.Abs(normalisedTrianglePoints[0].X) >= 1 || Math.Abs(normalisedTrianglePoints[0].Y) >= 1) ? true : false;
            isOutsideB = (Math.Abs(normalisedTrianglePoints[1].X) >= 1 || Math.Abs(normalisedTrianglePoints[1].Y) >= 1) ? true : false;
            isOutsideC = (Math.Abs(normalisedTrianglePoints[2].X) >= 1 || Math.Abs(normalisedTrianglePoints[2].Y) >= 1) ? true : false;

            Console.WriteLine("A: " + isOutsideA);
            Console.WriteLine("B: " + isOutsideB);
            Console.WriteLine("C: " + isOutsideC + "\n");

            outsideCount = 0;

            for (int i = 0; i < 8; i += 2)
            {
                bool b;
                Vector2 v;

                b = CheckLineIntersection(trianglePoints[0], trianglePoints[1], l[i], l[i + 1], out v);
                intersectionsA.Add(v);

                b = CheckLineIntersection(trianglePoints[1], trianglePoints[2], l[i], l[i + 1], out v);
                intersectionsB.Add(v);

                b = CheckLineIntersection(trianglePoints[2], trianglePoints[0], l[i], l[i + 1], out v);
                intersectionsC.Add(v);
            }

            intersectionsA = intersectionsA.Distinct().ToList();
            intersectionsA.Remove(new Vector2(float.NegativeInfinity));

            intersectionsB = intersectionsB.Distinct().ToList();
            intersectionsB.Remove(new Vector2(float.NegativeInfinity));

            intersectionsC = intersectionsC.Distinct().ToList();
            intersectionsC.Remove(new Vector2(float.NegativeInfinity));



            


            /*
            if (intersectionsA.Count > 1)
            {
                float d1;
                float d2;
                Vector2 v1 = intersectionsA[0];
                Vector2 v2 = intersectionsA[1];
                Vector2 t1 = trianglePoints[0];
                Vector2.Distance(ref v1, ref v2, out d1);
                Vector2.Distance(ref v1, ref t1, out d2);
                if (d1 < d2)
                {
                    Vector2 temp = intersectionsA[0];
                    intersectionsA[0] = intersectionsA[1];
                    intersectionsA[1] = temp;
                }
            }


            if (intersectionsB.Count > 1)
            {
                float d1;
                float d2;
                Vector2 v1 = intersectionsB[0];
                Vector2 v2 = intersectionsB[1];
                Vector2 t1 = trianglePoints[0];
                Vector2.Distance(ref v1, ref v2, out d1);
                Vector2.Distance(ref v1, ref t1, out d2);
                if (d1 < d2)
                {
                    Vector2 temp = intersectionsB[0];
                    intersectionsB[0] = intersectionsB[1];
                    intersectionsB[1] = temp;
                }
            }


            if (intersectionsC.Count > 1)
            {
                float d1;
                float d2;
                Vector2 v1 = intersectionsC[0];
                Vector2 v2 = intersectionsC[1];
                Vector2 t1 = trianglePoints[0];
                Vector2.Distance(ref v1, ref v2, out d1);
                Vector2.Distance(ref v1, ref t1, out d2);
                if (d1 < d2)
                {
                    Vector2 temp = intersectionsC[0];
                    intersectionsC[0] = intersectionsC[1];
                    intersectionsC[1] = temp;
                }
            }*/

            insideTriPoints = new List<ClippingPoint>();
            outsideTriPoints = new List<ClippingPoint>();

            List<Vector2> tempIntersectionsTo = new List<Vector2>();
            List<Vector2> tempIntersectionsFrom = new List<Vector2>();
            tempIntersectionsFrom.AddRange(intersectionsA);
            tempIntersectionsTo.AddRange(intersectionsC);

            clippingA = new ClippingPoint(trianglePoints[0], normalisedTrianglePoints[0], tempIntersectionsTo, tempIntersectionsFrom);
            clippingA.isOutside = isOutsideA;

            if (isOutsideA)
                outsideTriPoints.Add(clippingA);
            else
                insideTriPoints.Add(clippingA);

            tempIntersectionsTo = new List<Vector2>();
            tempIntersectionsFrom = new List<Vector2>();
            tempIntersectionsFrom.AddRange(intersectionsB);
            tempIntersectionsTo.AddRange(intersectionsA);

            clippingB = new ClippingPoint(trianglePoints[1], normalisedTrianglePoints[1], tempIntersectionsTo, tempIntersectionsFrom);
            clippingB.isOutside = isOutsideB;

            if (isOutsideB)
                outsideTriPoints.Add(clippingB);
            else
                insideTriPoints.Add(clippingB);

            tempIntersectionsTo = new List<Vector2>();
            tempIntersectionsFrom = new List<Vector2>();
            tempIntersectionsTo.AddRange(intersectionsB);
            tempIntersectionsFrom.AddRange(intersectionsC);

            clippingC = new ClippingPoint(trianglePoints[2], normalisedTrianglePoints[2], tempIntersectionsTo, tempIntersectionsFrom);
            clippingC.isOutside = isOutsideC;

            if (isOutsideC)
                outsideTriPoints.Add(clippingC);
            else
                insideTriPoints.Add(clippingC);

            squareList = new List<Square>();

            for (int i = 0; i < clippingA.intersectionPointsFrom.Count; i++)
                squareList.Add(new Square(clippingA.intersectionPointsFrom[i], new Vector2(4, 4), XColour.Red));

            for (int i = 0; i < clippingB.intersectionPointsFrom.Count; i++)
                squareList.Add(new Square(clippingB.intersectionPointsFrom[i], new Vector2(4, 4), XColour.Red));

            for (int i = 0; i < clippingC.intersectionPointsFrom.Count; i++)
                squareList.Add(new Square(clippingC.intersectionPointsFrom[i], new Vector2(4, 4), XColour.Red));

            List<DPoint> pointList = new List<DPoint>();

            /*
            for (int i = 0; i < insideTriPoints.Count; i++)
            {
                pointList.Add(Vec2toPoint(insideTriPoints[i].triPoint));
                for (int j = 0; j < insideTriPoints[i].intersectionPointsFrom.Count; j++)
                {
                    DPoint d = Vec2toPoint(insideTriPoints[i].intersectionPointsFrom[j]);

                    if (!pointList.Contains(d))
                        pointList.Add(d);
                }

                for (int j = 0; j < insideTriPoints[i].intersectionPointsTo.Count; j++)
                {
                    DPoint d = Vec2toPoint(insideTriPoints[i].intersectionPointsTo[j]);

                    if (!pointList.Contains(d))
                        pointList.Add(d);
                }

            }

            for (int i = 0; i < outsideTriPoints.Count; i++)
            {
                //pointList.Add(Vec2toPoint(outsideTriPoints[i].triPoint));
                for (int j = 0; j < outsideTriPoints[i].intersectionPointsFrom.Count; j++)
                {
                    DPoint d = Vec2toPoint(outsideTriPoints[i].intersectionPointsFrom[j]);

                    if (!pointList.Contains(d))
                        pointList.Add(d);
                }

                for (int j = 0; j < outsideTriPoints[i].intersectionPointsTo.Count; j++)
                {
                    DPoint d = Vec2toPoint(outsideTriPoints[i].intersectionPointsTo[j]);

                    if (!pointList.Contains(d))
                        pointList.Add(d);
                }

            }*/

            if (intersectionsA.Count > 1)
            {
                Vector2 one = trianglePoints[0];
                for (int i = 1; i < intersectionsA.Count; i++)
                {
                    Vector2 dist1;
                    float result1 = 0;
                    float result2 = 0;
                    Vector2 check1 = intersectionsA[i-1];
                    Vector2 check2 = intersectionsA[i];
                    Vector2.Distance(ref check1, ref one, out result1);
                    Vector2.Distance(ref check2, ref one, out result2);

                    if (result1 > result2)
                    {
                        Vector2 temp = intersectionsA[i];
                        intersectionsA[i] = intersectionsA[i - 1];
                        intersectionsA[i - 1] = temp;
                        i = 0;
                    }

                }
            }

            if (intersectionsB.Count > 1)
            {
                Vector2 one = trianglePoints[1];
                for (int i = 1; i < intersectionsB.Count; i++)
                {
                    Vector2 dist1;
                    float result1 = 0;
                    float result2 = 0;
                    Vector2 check1 = intersectionsB[i - 1];
                    Vector2 check2 = intersectionsB[i];
                    Vector2.Distance(ref check1, ref one, out result1);
                    Vector2.Distance(ref check2, ref one, out result2);

                    if (result1 > result2)
                    {
                        Vector2 temp = intersectionsB[i];
                        intersectionsB[i] = intersectionsB[i - 1];
                        intersectionsB[i - 1] = temp;
                        i = 0;
                    }

                }
            }

            if (intersectionsC.Count > 1)
            {
                Vector2 one = trianglePoints[2];
                for (int i = 1; i < intersectionsC.Count; i++)
                {
                    Vector2 dist1;
                    float result1 = 0;
                    float result2 = 0;
                    Vector2 check1 = intersectionsC[i - 1];
                    Vector2 check2 = intersectionsC[i];
                    Vector2.Distance(ref check1, ref one, out result1);
                    Vector2.Distance(ref check2, ref one, out result2);

                    if (result1 > result2)
                    {
                        Vector2 temp = intersectionsC[i];
                        intersectionsC[i] = intersectionsC[i - 1];
                        intersectionsC[i - 1] = temp;
                        i = 0;
                    }

                }
            }

            if (!isOutsideA)
                pointList.Add(Vec2toPoint(trianglePoints[0]));
            for (int i = 0; i < intersectionsA.Count; i++)
                pointList.Add(Vec2toPoint(intersectionsA[i]));

            if (!isOutsideB)
                pointList.Add(Vec2toPoint(trianglePoints[1]));
            for (int i = 0; i < intersectionsB.Count; i++)
                pointList.Add(Vec2toPoint(intersectionsB[i]));

            if (!isOutsideC)
                pointList.Add(Vec2toPoint(trianglePoints[2]));
            for (int i = 0; i < intersectionsC.Count; i++)
                pointList.Add(Vec2toPoint(intersectionsC[i]));


            if (pointList.Count > 0)
                polygonOutput = new Polygon(pointList, DColour.Green);
        }

        

        List<Square> squareList;
        
        protected override void DrawOnAnimate(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            
        }

        private DPoint Vec2toPoint(Vector2 vecIn)
        {
            return new DPoint((int)vecIn.X, (int)vecIn.Y);
        }

        protected override void ActionOnTrianglePlaced(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            lineTop.Draw(spriteBatch);
            lineLeft.Draw(spriteBatch);
            lineBottom.Draw(spriteBatch);
            lineRight.Draw(spriteBatch);

           

            if (polygonOutput != null)
                polygonOutput.Draw(spriteBatch);

            for (int i = 0; i < squareList.Count; i++)
                squareList[i].Draw(spriteBatch);
        }
    }
}
