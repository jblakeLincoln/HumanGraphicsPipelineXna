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
    class Helper
    {
        /// <summary>
        /// Take in an array and remove non-unique elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        static public List<T> EliminateDuplicates<T>(List<T> v)
        {
            List<T> temp = new List<T>();

            for (int i = 0; i < v.Count; i++)
                if (!temp.Contains(v[i]))
                    temp.Add(v[i]);

            return temp;
        }

        static public float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        // Within a 2D array, find the value prior to the current one (x-1).
        static public Vector2 GetPreviousValue<T>(Vector2 vIn, List<List<T>> l)
        {
            // 1D representation of array
            int t = (int)(vIn.Y * l[0].Count + vIn.X);

            if (t > 0)
                t--;

            int xx = 0;
            int yy = 0;

            // Get 2D representations.
            yy = (int)(t / l[0].Count);
            xx = (int)(t % l[0].Count);

            // Correct error if out of bounds.
            if (xx > l.Count)
            {
                xx = 0;
                yy++;
            }

            return new Vector2(xx, yy);
        }
    }
}
