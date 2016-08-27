using System;
using System.Drawing;
using System.Windows.Forms;
using btwm.Config;

namespace btwm.Layouts.Forms
{
    class htabbedForm : Form
    {
        public string[] Titles
        {
            get { return titles; }
            set
            {
                titles = value;
                Invalidate();
            }
        }
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                Invalidate();
            }
        }
        public RECT Surface
        {
            get { return ClientRectangle; }
            set
            {
                SetClientSizeCore(value.Width, value.Height);
                Invalidate();
            }
        }

        private string[] titles;
        private int index;
        private SolidBrush back;
        private SolidBrush fore;
        private SolidBrush focusedBack;
        private SolidBrush focusedFore;
        private RECT surface;
        private Font font;
        private Htabbed parent;

        public htabbedForm(Configuration conf, RECT surface, Htabbed parent)
        {
            titles = new string[0];
            index = 0;
            DoubleBuffered = true;
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            this.surface = surface;

            back = new SolidBrush(conf.Colors.Background);
            fore = new SolidBrush(conf.Colors.Foreground);
            focusedBack = new SolidBrush(conf.Colors.FocusedBackground);
            focusedFore = new SolidBrush(conf.Colors.FocusedForeground);
            font = conf.Font;
            this.parent = parent;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && titles.Length >= 2)
            {
                int newIndex = e.X * titles.Length / surface.Width;
                if (newIndex != index)
                {
                    parent.focusIndex(newIndex);
                    index = newIndex;
                    Invalidate();
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            SetBoundsCore(surface.Left, surface.Top, surface.Width, surface.Height, BoundsSpecified.All);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(back, ClientRectangle);
            if (titles.Length != 0)
            {
                float blockWidth = ClientRectangle.Width / (float)titles.Length;
                if (index != -1)
                    g.FillRectangle(focusedBack, new RectangleF(blockWidth * index, 0, blockWidth, surface.Height));

                for (int i = 0; i < titles.Length; i++)
                    g.DrawString(titles[i], Font, i == index ? focusedFore : fore, new RectangleF(2 + blockWidth * i, 1, blockWidth - 4, surface.Height - 2));
            }
        }
    }
}
