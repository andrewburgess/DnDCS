﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DnDCS.Libs.SimpleObjects;

namespace DnDCS.WinFormsLibs
{
    public class DnDSparklesPictureBox : UserControl
    {
        private Bitmap map;
        private Bitmap fog;
        private Point startPoint;
        private Point origin = new Point(0, 0);
        private Graphics gfx;
        private Rectangle sourceRect;
        private Rectangle destRect;
        private double zoomFactor;
        private Size apparentSize;
        private int drawWidth;
        private int drawHeight;
        private Point centerPoint;

        public event Action<Keys> TryToggleFullScreen;

        public bool IsBlackoutOn { get; set; }

        public Size ApparentImageSize
        {
            get { return apparentSize; }
        }

        public Image Fog
        {
            get { return fog; }
            set
            {
                if (fog != null)
                {
                    fog.Dispose();
                    GC.Collect();
                }

                if (value == null)
                {
                    fog = null;
                    Invalidate();
                    return;
                }

                fog = (Bitmap)value;
            }
        }

        public Image Map
        {
            get { return map; }
            set
            {
                if (map != null)
                {
                    map.Dispose();
                    origin = new Point(0, 0);
                    apparentSize = new Size(0, 0);
                    ZoomFactor = 1;
                    GC.Collect();
                }

                if (value == null)
                {
                    map = null;
                    Invalidate();
                    return;
                }

                var r = new Rectangle(0, 0, value.Width, value.Height);
                map = new Bitmap(value);
                map = map.Clone(r, PixelFormat.Format32bppArgb);
                Invalidate();
            }
        }

        public Point Origin
        {
            get { return origin; }
            set
            {
                origin = value;
                Invalidate();
            }
        }

        public double ZoomFactor
        {
            get { return zoomFactor; }
            set
            {
                zoomFactor = value;
                if (zoomFactor > 15)
                {
                    zoomFactor = 15;
                }
                if (zoomFactor < 0.05)
                {
                    zoomFactor = 0.05;
                }

                if (map != null)
                {
                    apparentSize.Height = (int)(map.Height * zoomFactor);
                    apparentSize.Width = (int)(map.Width * zoomFactor);
                    ComputeDrawingArea();
                    CheckBounds();
                }

                Invalidate();
            }
        }

        public DnDSparklesPictureBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void Init()
        {
            this.Height = this.Parent.Height;
            this.Width = this.Parent.Width;
            this.BackColor = Color.Black;
            ZoomFactor = 1;
        }

        public void RefreshMapPictureBox(bool immediateRefresh = false)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => { RefreshMapPictureBox(immediateRefresh); }));
                return;
            }

            Invalidate();
        }

        public void CenterMap(SimplePoint centerMap)
        {

        }

        public void SetFogAsync(Image newFog)
        {

        }

        public void SetMapAsync(Image newMap)
        {

        }

        public void SetFogUpdateAsync(FogUpdate fogUpdate)
        {

        }

        public void SetGridSize(bool showGrid, int gridSize)
        {

        }

        public void SetGridColor(SimpleColor gridColor)
        {

        }

        public void ZoomIn()
        {
            ZoomImage(true);
        }

        public void ZoomOut()
        {
            ZoomImage(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            DrawImage(e.Graphics);
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            destRect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            ComputeDrawingArea();
            base.OnSizeChanged(e);
        }

        private void CheckBounds()
        {
            if (map == null)
            {
                return;
            }

            if (origin.X < 0)
            {
                origin.X = 0;
            }
            if (origin.Y < 0)
            {
                origin.Y = 0;
            }
            if (origin.X > map.Width - (ClientSize.Width / zoomFactor))
            {
                origin.X = (int)(map.Width - (ClientSize.Width / zoomFactor));
            }
            if (origin.Y > map.Height - (ClientSize.Height / zoomFactor))
            {
                origin.Y = (int)(map.Height - (ClientSize.Height / zoomFactor));
            }
        }

        private void ComputeDrawingArea()
        {
            drawHeight = (int)(Height / zoomFactor);
            drawWidth = (int)(Width / zoomFactor);
        }

        private void DrawImage(Graphics g)
        {
            if (map == null)
            {
                return;
            }

            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            sourceRect = new Rectangle(origin.X, origin.Y, drawWidth, drawHeight);
            g.DrawImage(map, destRect, sourceRect, GraphicsUnit.Pixel);
        }

        private void ZoomImage(bool zoomIn = true)
        {
            centerPoint.X = origin.X + sourceRect.Width / 2;
            centerPoint.Y = origin.Y + sourceRect.Height / 2;
            if (zoomIn)
            {
                zoomFactor = Math.Round(zoomFactor * 1.1, 2);
            }
            else
            {
                zoomFactor = Math.Round(zoomFactor * 0.9, 2);
            }

            origin.X = (int)(centerPoint.X - ClientSize.Width / zoomFactor / 2);
            origin.Y = (int)(centerPoint.Y - ClientSize.Height / zoomFactor / 2);

            CheckBounds();
        }
    }
}