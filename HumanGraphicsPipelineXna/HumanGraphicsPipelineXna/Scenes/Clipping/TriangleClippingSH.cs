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
            polyList = new List<Polygon>();
            squareList = new List<Square>();
            polygonOutput = null;
            isOutsideList = new List<bool>();
            intersectionsLists = new List<List<Vector2>>();
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

        List<ClippingPoint> insideTriPoints;
        List<ClippingPoint> outsideTriPoints;


        protected override void StateChanges(GameTime gameTime)
        {
            base.StateChanges(gameTime);
            if (Inputs.MouseState.LeftButton == ButtonState.Released && Inputs.MouseStatePrevious.LeftButton == ButtonState.Pressed)
            {
                if (state <= triangleCount)
                {
                    CorrectNormalisedTriangle(state);
                }
            }
        }

        List<bool> isOutsideList;

        List<List<Vector2>> intersectionsLists = new List<List<Vector2>>();
        protected override void LastPointPlaced(GameTime gameTime)
        {

            CorrectNormalisedTriangle(triangleCount);

            for (int i = 0; i < triangleCount; i++)
            {
                isOutsideList.Add((Math.Abs(normalisedTrianglePoints[i].X) >= 1 || Math.Abs(normalisedTrianglePoints[i].Y) >= 1) ? true : false);
                intersectionsLists.Add(new List<Vector2>());
            }
            for (int i = 0; i < 8; i += 2)
            {
                bool b;
                Vector2 v;

                for (int j = 0; j < triangleCount-1; j++)
                {
                    b = CheckLineIntersection(trianglePoints[j], trianglePoints[j + 1], l[i], l[i + 1], out v);
                    intersectionsLists[j].Add(v);
                }

                b = CheckLineIntersection(trianglePoints[triangleCount-1], trianglePoints[0], l[i], l[i + 1], out v);
                intersectionsLists.Last().Add(v);
            }

            for (int i = 0; i < triangleCount; i++)
            {
                intersectionsLists[i] = intersectionsLists[i].Distinct().ToList();
                intersectionsLists[i].Remove(new Vector2(float.NegativeInfinity));
            }

            insideTriPoints = new List<ClippingPoint>();
            outsideTriPoints = new List<ClippingPoint>();

            List<Vector2> tempIntersectionsTo = new List<Vector2>();
            List<Vector2> tempIntersectionsFrom = new List<Vector2>();

            /*
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
            */


            squareList = new List<Square>();

            for (int i = 0; i < triangleCount; i++)
            {
                for (int j = 0; j < intersectionsLists[i].Count; j++)
                {
                    squareList.Add(new Square(intersectionsLists[i][j], new Vector2(4, 4), XColour.Red));
                }
            }

            /*
            for (int i = 0; i < clippingA.intersectionPointsFrom.Count; i++)
                squareList.Add(new Square(clippingA.intersectionPointsFrom[i], new Vector2(4, 4), XColour.Red));

            for (int i = 0; i < clippingB.intersectionPointsFrom.Count; i++)
                squareList.Add(new Square(clippingB.intersectionPointsFrom[i], new Vector2(4, 4), XColour.Red));

            for (int i = 0; i < clippingC.intersectionPointsFrom.Count; i++)
                squareList.Add(new Square(clippingC.intersectionPointsFrom[i], new Vector2(4, 4), XColour.Red));
            */

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

            for (int i = 0; i < intersectionsLists.Count; i++)
            {
                if (intersectionsLists[i].Count > 1)
                {
                    Vector2 one = trianglePoints[i];
                    for (int j = 1; j < intersectionsLists[i].Count; j++)
                    {
                        float result1 = 0;
                        float result2 = 0;
                        Vector2 check1 = intersectionsLists[i][j - 1];
                        Vector2 check2 = intersectionsLists[i][j];
                        Vector2.Distance(ref check1, ref one, out result1);
                        Vector2.Distance(ref check2, ref one, out result2);

                        if (result1 > result2)
                        {
                            Vector2 temp = intersectionsLists[i][j];
                            intersectionsLists[i][j] = intersectionsLists[i][j - 1];
                            intersectionsLists[i][j - 1] = temp;
                            j = 0;
                        }
                    }
                }
            }
            Vector2 lastIntersectionPoint = Vector2.Zero;
            for (int i = 0; i < isOutsideList.Count; i++)
            { 
                if (!isOutsideList[i])
                    pointList.Add(Vec2toPoint(trianglePoints[i]));

                

                for (int j = 0; j < intersectionsLists[i].Count; j++)
                {
                    pointList.Add(Vec2toPoint(intersectionsLists[i][j]));
                    lastIntersectionPoint = intersectionsLists[i][j];
                }

                /*
                if (pointList.Count > 1)
                {
                    Vector2 triPoint1 = trianglePoints[i];
                    Vector2 triPoint2 = (i + 1 < isOutsideList.Count) ? trianglePoints[i + 1] : trianglePoints[0];
                    for (int j = 0; j < 8; j += 2)
                    {
                        bool b = false;

                        
                        
                        List<Vector2> vecList = new List<Vector2>(){
                            PointtoVec2(pointList[pointList.Count-1]),
                            triPoint1,
                            triPoint2
                        };
                        b = FindPointInPolygon(vecList, l[j]);

                        if (b)
                        {
                            DPoint temp = pointList.Last();
                            //pointList[pointList.Count-1] = Vec2toPoint(l[j]);
                            pointList.Add(Vec2toPoint(l[j]));
                        }
                    }
                }*/

                if (i >= 1 && lastIntersectionPoint != Vector2.Zero)
                {
                    Vector2 triPoint1 = trianglePoints[i];
                    Vector2 triPoint2 = trianglePoints[i-1];
                    for (int j = 0; j < 8; j += 2)
                    {
                        bool b = false;

                        List<Vector2> vecList = new List<Vector2>(){
                            lastIntersectionPoint,
                            triPoint1,
                            triPoint2
                        };
                        b = FindPointInPolygon(vecList, l[j]);

                        if (b)
                        {
                            DPoint temp = pointList.Last();
                            pointList[pointList.Count-1] = Vec2toPoint(l[j]);
                            pointList.Add(temp);
                            //pointList.Add(Vec2toPoint(l[j]));
                        }
                    }
                }

            }

            Vector2 tp1 = trianglePoints[triangleCount-1];
            Vector2 tp2 = trianglePoints[0];
            for (int j = 0; j < 8; j += 2)
            {
                bool b = false;

                List<Vector2> vecList = new List<Vector2>(){
                            lastIntersectionPoint,
                            tp1,
                            tp2
                        };
                b = FindPointInPolygon(vecList, l[j]);

                if (b)
                {
                    DPoint temp = pointList.Last();
                    pointList[pointList.Count - 1] = Vec2toPoint(l[j]);
                    pointList.Add(temp);
                   // pointList.Add(Vec2toPoint(l[j]));
                }
            }
            List<DColour> dCol = new List<DColour>(){
                DColour.Red,
                DColour.Yellow,
                DColour.Green,
                DColour.Blue,
                DColour.White,
                DColour.Gray,
                DColour.CornflowerBlue,
                DColour.Plum,
                DColour.Olive,
                DColour.Red,
                DColour.Yellow,
                DColour.Green,
                DColour.Blue,
                DColour.White,
                DColour.Gray,
                DColour.CornflowerBlue,
                DColour.Plum,
                DColour.Olive,
                DColour.Red,
                DColour.Yellow,
                DColour.Green,
                DColour.Blue,
                DColour.White,
                DColour.Gray,
                DColour.CornflowerBlue,
                DColour.Plum,
                DColour.Olive,
            };


            if (pointList.Count > 0)
            {
                polygonOutput = new Polygon(pointList, DColour.Green);

                for (int i = 0; i < pointList.Count - 2; i++)
                {
                    List<DPoint> d = new List<DPoint>(){
                        pointList[0],
                        pointList[i+1],
                        pointList[i+2],
                    };

                    

                    bool b = FindPointInPolygon(trianglePoints.ToList(), FindCentroidOfTriangle(d));

                    if (b)
                    {
                        Polygon p = new Polygon(d, dCol[i]);
                        polyList.Add(p);
                    }
                }
            }

        }

        public Vector2 FindCentroidOfTriangle(List<Vector2> pointsIn)
        {
            float centreX = (pointsIn[0].X + pointsIn[1].X + pointsIn[2].X) / 3;
            float centreY = (pointsIn[0].Y + pointsIn[1].Y + pointsIn[2].Y) / 3;

            return new Vector2(centreX, centreY);
        }

        public Vector2 FindCentroidOfTriangle(List<DPoint> pointsIn)
        {
            float centreX = (pointsIn[0].X + pointsIn[1].X + pointsIn[2].X) / 3;
            float centreY = (pointsIn[0].Y + pointsIn[1].Y + pointsIn[2].Y) / 3;

            return new Vector2(centreX, centreY);
        }

        public bool FindPointInPolygon(List<Vector2> points, Vector2 p)
        {
            List<float> v = new List<float>();
            for (int i = 1; i < points.Count; i++)
            {
                v.Add(orient2d(points[i - 1], points[i], p));
            }
            v.Add(orient2d(points[points.Count - 1], points[0], p));

            bool negative = false;

            if (v[0] < 0)
                negative = true;

            for (int i = 0; i < v.Count; i++)
            {
                if (negative && v[i] >= 0)
                    return false;
                else if (!negative && v[i] < 0)
                    return false;
            }


            return true;
        }

        private float orient2d(Vector2 a, Vector2 b, Vector2 p) // a = input 1, b = input 2, p = point to check
        {
            return (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
        }

        List<Polygon> polyList;

        List<Square> squareList;
        
        protected override void DrawOnAnimate(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            
        }

        private DPoint Vec2toPoint(Vector2 vecIn)
        {
            return new DPoint((int)vecIn.X, (int)vecIn.Y);
        }

        private Vector2 PointtoVec2(DPoint pointIn)
        {
            return new Vector2((int)pointIn.X, (int)pointIn.Y);
        }

        protected override void ActionOnTrianglePlaced(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
           // if (polygonOutput != null)
           //     polygonOutput.Draw(spriteBatch);

            if (polyList != null)
                for (int i = 0; i < polyList.Count; i++)
                    polyList[i].Draw(spriteBatch);

            base.Draw(spriteBatch);

            lineTop.Draw(spriteBatch);
            lineLeft.Draw(spriteBatch);
            lineBottom.Draw(spriteBatch);
            lineRight.Draw(spriteBatch);

           

            

            for (int i = 0; i < squareList.Count; i++)
                squareList[i].Draw(spriteBatch);
        }
    }
}
