﻿using System.Drawing;
using System.Linq;
using System.IO;
using DnDCS.Libs.SimpleObjects;
using DnDCS.Libs;
using DnDCS.Libs.PersistenceObjects;
using System.Collections.Generic;

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

        public static Point ToPoint(this SimplePoint point)
        {
            return new Point(point.X, point.Y);
        }

        public static Point Translate(this Point point, int x, int y, float factor = 1.0f)
        {
            return new Point((int)((point.X + x) * factor), (int)((point.Y + y)* factor));
        }

        public static Point Translate(this Point point, Point translate, float factor = 1.0f)
        {
            return point.Translate(translate.X, translate.Y, factor);
        }

        public static SimplePoint ToSimplePoint(this Point point)
        {
            return new SimplePoint(point.X, point.Y);
        }

        public static SimplePoint Translate(this SimplePoint point, int x, int y)
        {
            return new SimplePoint(point.X + x, point.Y + y);
        }

        public static Color ToColor(this SimpleColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static SimpleColor ToSocketColor(this Color color)
        {
            return new SimpleColor(color.A, color.R, color.G, color.B);
        }

        public static FogUpdate ToFogUpdate(this FogData fogData)
        {
            return new FogUpdate(fogData.Points, fogData.IsClearing);
        }

        public static FogUpdate[] ToFogUpdate(this FogData[] fogDatas)
        {
            return fogDatas.Select(ToFogUpdate).ToArray();
        }

        public static FogData ToFogData(this FogUpdate fogUpdate)
        {
            return new FogData()
            {
                Points = fogUpdate.Points.ToArray(),
                IsClearing = fogUpdate.IsClearing
            };
        }

        public static FogData[] ToFogData(this IEnumerable<FogUpdate> fogUpdates)
        {
            return fogUpdates.Select(ToFogData).ToArray();
        }

        public static void WriteMap(this ServerSocketConnection connection, Image map)
        {
            connection.WriteMap(map.Width, map.Height, map.ToBytes());
        }

        public static void WriteFog(this ServerSocketConnection connection, Image fog)
        {
            connection.WriteFog(fog.Width, fog.Height, fog.ToBytes());
        }

    }
}
