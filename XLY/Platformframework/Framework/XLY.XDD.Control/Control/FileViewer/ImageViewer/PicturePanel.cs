using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace XLY.XDD.Control
{
    
    public partial class PicturePanel : Panel
    {
        internal bool newMethod = true;//for demo

        public PicturePanel():base()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.Opaque,true);
            
        }

        
        private EditableBitmap image;

        public EditableBitmap Image
        {
            get { return image; }
            set { image = value; Refresh();}
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
 
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            //these settings aren't even needed for good perf, but they are helpful
            pe.Graphics.InterpolationMode = InterpolationMode.Low;
            pe.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            pe.Graphics.SmoothingMode = SmoothingMode.HighSpeed;

            if (image != null)
            {
            	//draw empty area, if it exists, in an optimized way.
                if(AutoScrollPosition.X==0)
                {
                    int emptyRightAreaWidth=Width-image.Bitmap.Width;
                    if(emptyRightAreaWidth>0)
                    {
                        Rectangle fillRect=new Rectangle(image.Bitmap.Width,0,emptyRightAreaWidth,Height);
                        fillRect.Intersect(pe.ClipRectangle);
                        pe.Graphics.FillRectangle(SystemBrushes.Control,fillRect);
                    }
                }

                if(AutoScrollPosition.Y==0)
                {
                    int emptyRightAreaHeight=Height-image.Bitmap.Height;
                    if(emptyRightAreaHeight>0)
                    {
                        Rectangle fillRect=new Rectangle(0,image.Bitmap.Height,Width,emptyRightAreaHeight);
                        fillRect.Intersect(pe.ClipRectangle);
                        pe.Graphics.FillRectangle(SystemBrushes.Control,fillRect);
                    }
                }

                //calculate the visible area of the bitmap
                Rectangle bitmapRect = new Rectangle(AutoScrollPosition.X, AutoScrollPosition.Y, image.Bitmap.Width, image.Bitmap.Height);
                Rectangle visibleClientRect = bitmapRect;
                visibleClientRect.Intersect(pe.ClipRectangle);
                if (visibleClientRect.Width == 0 || visibleClientRect.Height == 0)
                    return;
                Rectangle visibleBitmapRect = visibleClientRect;
                visibleBitmapRect.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);

                //draw bitmap
                if (newMethod)
                {
                    using(EditableBitmap section = image.CreateView(visibleBitmapRect))
                        pe.Graphics.DrawImageUnscaled(section.Bitmap, visibleClientRect.Location);
                }
                else //normal method
                {
                    pe.Graphics.DrawImage(image.Bitmap, visibleClientRect, visibleBitmapRect, GraphicsUnit.Pixel);
                }
            }
            else //if no bitmap just fill with background color
                pe.Graphics.FillRectangle(SystemBrushes.Control, pe.ClipRectangle);
        }

        public void QuickUpdate(Rectangle rect)
        {
            OnPaint(new PaintEventArgs(CreateGraphics(),rect));
        }

        public void SetPixel(Brush brush, int bmpX, int bmpY)
        {
            using(Graphics g = CreateGraphics())
                g.FillRectangle(brush,new Rectangle(bmpX,bmpY,1,1));
        }
    }
}
