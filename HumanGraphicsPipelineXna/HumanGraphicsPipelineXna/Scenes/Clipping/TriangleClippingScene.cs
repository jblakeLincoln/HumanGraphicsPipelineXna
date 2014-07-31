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

        // Viewport boundaries
        private Vector2 pointTopLeft;
        private Vector2 pointTopRight;
        private Vector2 pointBottomLeft;
        private Vector2 pointBottomRight;
        // Lines for detecting edges.
        private List<Vector2> lineBoundaries;

        // Viewport boundaries to draw
        private Line lineTop;
        private Line lineLeft;
        private Line lineBottom;
        private Line lineRight;

        // Polygons to draw
        private List<Polygon> polyList;

        // Intersections for each of the primitive
        private List<List<Vector2>> intersectionsLists;

        // Intersection points to draw
        private List<Square> squareList;

        // Lines of primitive to draw
        private List<Line> linesOutput;

        // If primitive vertice is outside of viewport.
        private List<bool> isOutsideList;

        protected override void DerivedInit()
        {
            base.DerivedInit();
            polyList = new List<Polygon>();
            squareList = new List<Square>();
            isOutsideList = new List<bool>();
            intersectionsLists = new List<List<Vector2>>();

            pointTopLeft = new Vector2(Globals.viewportWidth / 6, Globals.viewportHeight / 6);
            pointTopRight = new Vector2(Globals.viewportWidth - (Globals.viewportWidth / 6), Globals.viewportHeight / 6);
            pointBottomLeft = new Vector2(Globals.viewportWidth / 6, Globals.viewportHeight - (Globals.viewportHeight / 6));
            pointBottomRight = new Vector2(Globals.viewportWidth - (Globals.viewportWidth / 6), Globals.viewportHeight - (Globals.viewportHeight / 6));

            lineTop = new Line(pointTopLeft, pointTopRight, XColour.Black, 1f);
            lineLeft = new Line(pointTopLeft, pointBottomLeft, XColour.Black, 1f);
            lineBottom = new Line(pointBottomLeft, pointBottomRight, XColour.Black, 1f);
            lineRight = new Line(pointTopRight, pointBottomRight, XColour.Black, 1f);

             lineBoundaries = new List<Vector2>() {
                    new Vector2(int.MinValue, pointTopLeft.Y), new Vector2(int.MaxValue, pointTopRight.Y),
                    new Vector2(pointBottomLeft.X, int.MinValue), new Vector2(pointTopLeft.X, int.MaxValue),
                    new Vector2(int.MinValue, pointBottomLeft.Y), new Vector2(int.MaxValue, pointBottomRight.Y),
                    new Vector2(pointTopRight.X, int.MinValue), new Vector2(pointBottomRight.X, int.MaxValue)
            };


             linesOutput = new List<Line>();
        }

        /// <summary>
        /// Correct normalised coordinates to represent "viewport" instead of entire screen.
        /// </summary>
        /// <param name="state">Index of triangle point array to correct</param>
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

        /// <summary>
        /// Check if two line segments intersect, and output any intersection point
        /// </summary>
        /// <param name="p1">Line segment 1, point 1</param>
        /// <param name="p2">Line segment 1, point 2</param>
        /// <param name="q1">Line segment 2, point 1</param>
        /// <param name="q2">Line segment 2, point 2</param>
        /// <param name="intersectionPoint">Output any intersection points, or infinity if lines do not intersect.</param>
        /// <returns></returns>
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

        /// <summary>
        /// When a new point is placed.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void StateChanges(GameTime gameTime)
        {
            base.StateChanges(gameTime);
            if (Inputs.MouseState.LeftButton == ButtonState.Released && Inputs.MouseStatePrevious.LeftButton == ButtonState.Pressed)
            {
                if (state <= triangleCount)
                    CorrectNormalisedTriangle(state);
            }
        }

        /// <summary>
        /// Actions to be performed when the last point is placed.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void LastPointPlaced(GameTime gameTime)
        {
            // Correct normalised point of last point placed.
            CorrectNormalisedTriangle(triangleCount);

            // Determine whether points are inside or outside viewport.
            for (int i = 0; i < triangleCount; i++)
            {
                isOutsideList.Add((Math.Abs(normalisedTrianglePoints[i].X) >= 1 || Math.Abs(normalisedTrianglePoints[i].Y) >= 1) ? true : false);
                intersectionsLists.Add(new List<Vector2>());
            }

            // Check intersections of lines with the edges of viewport.
            for (int i = 0; i < 8; i += 2)
            {
                bool b = false;
                Vector2 v = Vector2.Zero;

                for (int j = 0; j < triangleCount-1; j++)
                {
                    b = CheckLineIntersection(trianglePoints[j], trianglePoints[j + 1], lineBoundaries[i], lineBoundaries[i + 1], out v);
                    intersectionsLists[j].Add(v);
                }

                b = CheckLineIntersection(trianglePoints[triangleCount-1], trianglePoints[0], lineBoundaries[i], lineBoundaries[i + 1], out v);
                intersectionsLists[intersectionsLists.Count-1].Add(v);
            }

            // For each intersection point along infinite viewport lines,
            // if it lies outside the viewport and is within the primitive, consider it part of the clipped primitive.
            for (int i = 0; i < triangleCount; i++)
            {
                for (int j = 0; j < intersectionsLists[i].Count; j++)
                {
                    bool changed = false;

                    Vector2 temp = intersectionsLists[i][j];
                    intersectionsLists[i][j] = Vector2.Clamp(intersectionsLists[i][j], pointTopLeft, pointBottomRight);

                    if (intersectionsLists[i][j] != temp)
                        changed = true;

                    if (changed && !FindPointInPolygon(trianglePoints.ToList(), intersectionsLists[i][j]))
                    {
                        intersectionsLists[i].RemoveAt(j);
                        j--;
                    }
                }

                intersectionsLists[i] = Helper.EliminateDuplicates(intersectionsLists[i]);
                intersectionsLists[i].Remove(new Vector2(float.NegativeInfinity));
            }


            List<Vector2> tempIntersectionsTo = new List<Vector2>();
            List<Vector2> tempIntersectionsFrom = new List<Vector2>();

            // Add intersection points to visualise on screen with red squares.
            squareList = new List<Square>();
            for (int i = 0; i < triangleCount; i++)
            {
                for (int j = 0; j < intersectionsLists[i].Count; j++)
                {
                    squareList.Add(new Square(intersectionsLists[i][j], new Vector2(4, 4), XColour.Red));
                }
            }


            List<DPoint> pointList = new List<DPoint>();

            // Sort ntersection points on line so the are ordered by their position along the line segment
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

            // Fill list of points that are part of the new primitive
            for (int i = 0; i < isOutsideList.Count; i++)
            { 
                if (!isOutsideList[i]) // If the original point is within the viewport, it is accepted.
                    pointList.Add(Vec2toPoint(trianglePoints[i]));

                for (int j = 0; j < intersectionsLists[i].Count; j++) // Intersection points are all added.
                    pointList.Add(Vec2toPoint(intersectionsLists[i][j]));
            }

            // Intersection points on the corners can sometimes produce duplicates, 
            // so make sure list only contains unique elements.
            pointList = Helper.EliminateDuplicates(pointList);

            // Set indexing for drawing primitive.
            // Triangle fan stemming from first point.
            List<XColour> dCol = new List<XColour>();
            if (pointList.Count > 0)
            {
                for (int i = 0; i < pointList.Count - 2; i++)
                {
                    List<DPoint> d = new List<DPoint>(){
                        pointList[0],
                        pointList[i+1],
                        pointList[i+2],
                    };

                    DColour c = DColour.FromArgb(15+Globals.rand.Next(127), 15+Globals.rand.Next(127), 15+Globals.rand.Next(127));
                    XColour xC = new XColour(c.R + 15, c.G + 15, c.B + 15);
                    linesOutput.Add(new Line(PointtoVec2(pointList[0]), PointtoVec2(pointList[i + 1]), xC, 2f));
                    linesOutput.Add(new Line(PointtoVec2(pointList[i+1]), PointtoVec2(pointList[i + 2]), xC, 2f));
                    linesOutput.Add(new Line(PointtoVec2(pointList[i+2]), PointtoVec2(pointList[0]), xC, 2f));

                    Polygon p = new Polygon(d, c);
                    polyList.Add(p); // List of polygons to be drawn on screen.
                }
            }

        }

        // Check if point resides in polygon using the halfspace method.
        public bool FindPointInPolygon(List<Vector2> points, Vector2 p)
        {
            List<float> v = new List<float>();
            for (int i = 1; i < points.Count; i++)
                v.Add(orient2d(points[i - 1], points[i], p));

            v.Add(orient2d(points[points.Count - 1], points[0], p));

            bool negative = false;

            if (v[0] < 0)
                negative = true;

            for (int i = 0; i < v.Count; i++)
                if (negative && v[i] >= 0)
                    return false;
                else if (!negative && v[i] < 0)
                    return false;

            return true;
        }

        // Used by FindPointInPolygon
        private float orient2d(Vector2 a, Vector2 b, Vector2 p) // a = input 1, b = input 2, p = point to check
        {
            return (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
        }

        private DPoint Vec2toPoint(Vector2 vecIn)
        {
            return new DPoint((int)vecIn.X, (int)vecIn.Y);
        }

        private Vector2 PointtoVec2(DPoint pointIn)
        {
            return new Vector2((int)pointIn.X, (int)pointIn.Y);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (polyList != null)
                for (int i = 0; i < polyList.Count; i++)
                    polyList[i].Draw(spriteBatch);

            base.Draw(spriteBatch);

            for (int i = linesOutput.Count-1; i >= 0; i--)
                linesOutput[i].Draw(spriteBatch);

            lineTop.Draw(spriteBatch);
            lineLeft.Draw(spriteBatch);
            lineBottom.Draw(spriteBatch);
            lineRight.Draw(spriteBatch);

            for (int i = 0; i < squareList.Count; i++)
                squareList[i].Draw(spriteBatch);
        }

        protected override void DrawOnAnimate(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch) { }
        protected override void ActionOnTrianglePlaced(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch) { }
    }
}
