﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DnDCS_Client.Shared
{
    public static class Debug
    {
        public static SpriteFont Font { get; set; }

        private static readonly IList<string> debugText = new List<string>();
        public static string FullDebugText { get { return string.Join("\n", debugText); } }

        public static void Clear()
        {
            debugText.Clear();
        }

        public static void Add(string msg)
        {
            lock (debugText)
            {
                debugText.Add(msg);
            }
        }
    }
}
