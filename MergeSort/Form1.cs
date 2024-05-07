﻿using MergeSortSteps;

using OpenTK.Graphics.ES20;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MergeSort
{
    public partial class Form1 : Form
    {
        SKMatrix matrix = SKMatrix.CreateIdentity();
        SKPoint drawOffset = new SKPoint();
        float scale = 1f;
        Timer tick;
        Drawer drawer;
        public Form1()
        {
            InitializeComponent();
            tick = new Timer();
            tick.Interval = 1;
            tick.Tick += Update;
            int[] items = new int[50];
            Random rnd = new Random(0);
            for (int i = 0;i<items.Length;i++)
            {
                items[i] = i%10;
            }
            for (int i = 0;i<items.Length;i++)
            {
                int s = items[i];
                int sI = rnd.Next(items.Length);
                items[i] = items[sI];
                items[sI] = s;
            }
            drawer = new Drawer(items, squareHalfWidth, splitWidth, skiaView.Invalidate);
            skiaView.MouseWheel += SkiaView_MouseWheel;
            tick.Start();
        }

        private void SkiaView_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldScale = scale;
            if (e.Delta > 1)
            {
                scale *= 1.025f;
            } else
            {
                scale /= 1.025f;
            }
            if (scale > 10f) scale = 10f;
            if (scale < 0.1f) scale = 0.1f;
            //drawOffset = new SKPoint(drawOffset.X + (e.Location.X * (oldScale - scale)), drawOffset.Y + (e.Location.Y * (oldScale - scale)));
            drawOffset = new SKPoint(e.Location.X + ((drawOffset.X - e.Location.X) * scale / oldScale), e.Location.Y + ((drawOffset.Y - e.Location.Y) * scale / oldScale));
            matrix = SKMatrix.CreateScaleTranslation(scale, scale, drawOffset.X, drawOffset.Y);
            skiaView.Invalidate();
        }

        private void Update(object sender, EventArgs e)
        {
            drawer?.Tick();
            if (autoStep != 0) drawer.AnimateTo(drawer.Current + autoStep);
        }

        const float squareHalfWidth = 10;

        const float splitWidth = 10;
        private void skiaView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.SetMatrix(matrix);
            // make sure the canvas is blank
            canvas.Clear(SKColors.White);

            drawer.Paint(canvas);
        }

        Point mouseLastPos;

        private void skiaView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
            {
                int diffX = e.X - mouseLastPos.X;
                int diffY = e.Y - mouseLastPos.Y;
                mouseLastPos = e.Location;
                drawOffset = new SKPoint(drawOffset.X + diffX, drawOffset.Y + diffY);
                matrix = SKMatrix.CreateScaleTranslation(scale, scale, drawOffset.X, drawOffset.Y);
                skiaView.Invalidate();
            }
        }

        private void skiaView_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLastPos = e.Location;
        }

        private void NextStep()
        {
            drawer.AnimateTo(drawer.Current + 1);
        }

        private void LastStep()
        {
            drawer.AnimateTo(drawer.Current - 1);
        }

        int autoStep = 0;

        private void skiaView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'a')
            {
                LastStep();
            }
            if (e.KeyChar == 'd')
            {
                NextStep();
            }
            if (e.KeyChar == 's')
            {
                autoStep = 0;
            }
            if (e.KeyChar == 'q')
            {
                autoStep = -1;
            }
            if (e.KeyChar == 'e')
            {
                autoStep = 1;
            }
            if (e.KeyChar == 'r')
            {
                drawer.AnimateTo(0);
            }
            if (e.KeyChar == 'b')
            {
                matrix = SKMatrix.CreateIdentity();
                scale = 1f;
                drawOffset = SKPoint.Empty;
                skiaView.Invalidate();
            }
        }
    }
}