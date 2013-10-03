﻿using System.Drawing;
using System.IO;
using DnDCS.Libs.SocketObjects;

namespace DnDCS.WinFormsLibs
{
    public static class FormsUtils
    {
        public static byte[] ToBytes(this Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public static Image ToImage(this byte[] dataBytes)
        {
            using (var ms = new MemoryStream(dataBytes))
            {
                return Image.FromStream(ms);
            }
        }

        public static Point ToPoint(this DnDPoint point)
        {
            return new Point(point.X, point.Y);
        }

        public static DnDPoint ToDnDPoint(this Point point)
        {
            return new DnDPoint(point.X, point.Y);
        }

        public static Color ToColor(this SocketColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static SocketColor ToSocketColor(this Color color)
        {
            return new SocketColor(color.A, color.R, color.G, color.B);
        }
    }
}
