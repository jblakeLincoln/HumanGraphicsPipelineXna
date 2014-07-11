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
    class Inputs
    {
        static public MouseState MouseState { get; private set; }
        static public MouseState MouseStatePrevious { get; private set; }

        public void Init() { }

        static public void Update(GameTime gameTime)
        {
            MouseState = MouseStatePrevious;
            MouseStatePrevious = Mouse.GetState();
        }


    }
}
