namespace PdfiumViewer
{
    public class Bound
    {
        public double Left;
        public double Top;
        public double Right;
        public double Bottom;

        public Bound()
        {
        }

        /// <summary>
        /// Rectangle used to capture pdf text. Messured by pixel
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public Bound(double left, double top, double right, double bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public override string ToString()
        {
            return string.Format("L:{0}, T:{1}, R:{2}, B:{3}", Left, Top, Right, Bottom);
        }
    }
}