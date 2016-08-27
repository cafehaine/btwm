using System;
using System.Drawing;
using System.Windows.Forms;
using btwm.Config;

namespace btwm.Layouts.Forms
{
    class vtabbedForm : Form
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
        private int lineHeight;
        private Font font;

        public vtabbedForm(Configuration conf, RECT surface)
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
            lineHeight = font.Height + 2;
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
                if (index != -1)
                    g.FillRectangle(focusedBack, new Rectangle(0, lineHeight * index, Width, lineHeight));

                for (int i = 0; i < titles.Length; i++)
                    g.DrawString(titles[i], Font, i == index ? focusedFore : fore, new PointF(2, lineHeight * i + 1));
            }
        }
    }
}
