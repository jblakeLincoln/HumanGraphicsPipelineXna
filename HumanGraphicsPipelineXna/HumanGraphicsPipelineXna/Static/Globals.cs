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
    class Globals
    {
        public static void Init()
        {
            pixelSize = 20;
        }

        public static GraphicsDeviceManager graphics { get; set; }
        public static GraphicsDevice graphicsDevice { get { return graphics.GraphicsDevice; } }
        public static Vector2 viewport { get { return new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);} }
        public static int viewportWidth { get { return (int)viewport.X; } }
        public static int viewportHeight { get { return (int)viewport.Y; } }


        //Application specific
        public static int pixelSize { get; private set;}

        
    }
}
