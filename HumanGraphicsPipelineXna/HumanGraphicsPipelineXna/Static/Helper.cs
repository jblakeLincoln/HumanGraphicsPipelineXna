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
    }
}
