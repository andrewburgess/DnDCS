﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace DnDCS.Libs.SocketObjects
{
    public class FogUpdateSocketObject : BaseSocketObject
    {
        public SocketPoint[] Points { get; set; }
        public bool IsClearing { get; set; }

        public FogUpdateSocketObject(SocketConstants.SocketAction action, SocketPoint[] points, bool isClearing)
            : base(action)
        {
            Points = (points ?? new SocketPoint[0]).ToArray();
            IsClearing = isClearing;
        }

        public FogUpdateSocketObject(SocketConstants.SocketAction action, Point[] points, bool isClearing)
            : base(action)
        {
            Points = (points ?? new Point[0]).Select(p => new SocketPoint(p.X, p.Y)).ToArray();
            IsClearing = isClearing;
        }

        public FogUpdateSocketObject(SocketConstants.SocketAction action, FogUpdate fogUpdate)
            : this(action, fogUpdate.Points, fogUpdate.IsClearing)
        {
        }

        public static FogUpdateSocketObject PointArrayObjectFromBytes(byte[] bytes)
        {
            var action = (SocketConstants.SocketAction)bytes[0];
            switch (action)
            {
                case SocketConstants.SocketAction.FogUpdate:
                    return new FogUpdateSocketObject(action, ConvertBytesToSocketPointArray(bytes.Skip(2).ToArray()), bytes[1] == (byte)1);

                default:
                    throw new NotSupportedException(string.Format("Action '{0}' is not supported.", action));
            }
        }

        private static byte[] ConvertSocketPointsToBytes(SocketPoint[] points)
        {
            var pointBytes = new List<byte>(points.Length * 4);
            foreach (var point in points)
            {
                pointBytes.AddRange(BitConverter.GetBytes(point.X));
                pointBytes.AddRange(BitConverter.GetBytes(point.Y));
            }
            return pointBytes.ToArray();
        }

        private static SocketPoint[] ConvertBytesToSocketPointArray(byte[] pointBytes)
        {
            var points = new List<SocketPoint>(pointBytes.Length / 4);
            for (int i = 0; i < pointBytes.Length; i += 8)
            {
                var x = BitConverter.ToInt32(pointBytes, i);
                var y = BitConverter.ToInt32(pointBytes, i + 4);
                points.Add(new SocketPoint(x, y));
            }
            return points.ToArray();
        }

        public override byte[] GetBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(ActionByte);
            bytes.Add(IsClearing ? (byte)1 : (byte)0);
            bytes.AddRange(ConvertSocketPointsToBytes(this.Points));
            return bytes.ToArray();
        }

        public override string ToString()
        {
            return string.Format("Socket Action: '{0}', Number of Points: {1}, IsClearing: {2}", Action, Points.Length, IsClearing);
        }
    }
}