namespace btwm
{
    class PixelCoord
    {
        public int X;
        public int Y;

        public PixelCoord()
        {
            X = 0;
            Y = 0;
        }

        public PixelCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Return a boolean checking if this pixel is contained in a RECT
        /// </summary>
        /// <param name="rect">The RECT to check if the pixel is in</param>
        /// <returns></returns>
        public bool IsContainedInRect(RECT rect)
        {
            if (X < rect.Left || Y < rect.Top ||
                X > rect.Right || Y > rect.Bottom)
                return false;

            return true;
        }

		public override string ToString ()
		{
			return "{X=" + X.ToString() + ",Y=" + Y.ToString() + "}";
		}
    }
}
