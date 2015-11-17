using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace PdfiumViewer
{
    /// <summary>
    /// print scaling setting
    /// </summary>
    public enum PdfPrintPageScaling
    {
        /// <summary>
        /// print page fit paper size used
        /// </summary>
        FitSize,
        /// <summary>
        /// use the actual size of print pages, ignore the discrepancy
        /// </summary>
        ActualSize,
        /// <summary>
        /// only shrink the pages that bigger than paper size
        /// </summary>
        ShrinkOversized,
        /// <summary>
        /// cuastomer scaling set, by percents
        /// </summary>
        CustomScale
    }
    /// <summary>
    /// pdf print class
    /// </summary>
    public class PdfPrintDocument : PrintDocument
    {
        private static double GetInchFromPoint(double point)
        {
            return 0.013888888888889 * point;
        }

        public int PrintToPage 
        {
            get { return this.PrinterSettings.ToPage; }
            set { this.PrinterSettings.ToPage = value;}
        }
        public int PrintFromPage
        {
            get { return this.PrinterSettings.FromPage; }
            set { this.PrinterSettings.FromPage = value;}
        }
        public PdfPrintPageScaling Scaling;
        public int CustomScaling;
        public bool IsAutoOrientation;

        private readonly PdfDocument _document;
        private readonly PdfPrintMode _printMode;
        private int _currentPage;

        /// <summary>
        /// print pdf file transmitted by PdfDocument instance 
        /// </summary>
        /// <param name="document">pdf document loaded in a PdfDocument instance</param>
        /// <param name="printMode">margin type</param>
        public PdfPrintDocument(PdfDocument document, PdfPrintMode printMode)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            this._document = document;
            this._printMode = printMode;
            
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            this._currentPage = this.PrinterSettings.FromPage == 0 ? 0 : this.PrinterSettings.FromPage - 1;
        }

        protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            if (this._currentPage <= this.PrintToPage)
            {
                // Some printers misreport landscape. The below check verifies
                // whether the page rotation matches the landscape setting.
                if (this.IsAutoOrientation)
                {
                    bool inverseLandscape = e.PageSettings.Bounds.Width > e.PageSettings.Bounds.Height !=
                                            e.PageSettings.Landscape;

                    bool landscape = GetOrientation(this._document.PageSizes[this._currentPage]) ==
                                     Orientation.Landscape;

                    if (inverseLandscape)
                        landscape = !landscape;

                    e.PageSettings.Landscape = landscape;
                }
                //set size
                if(this.Scaling != PdfPrintPageScaling.ActualSize)
                    e.PageSettings.PaperSize = this.PrinterSettings.DefaultPageSettings.PaperSize;
            }
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            if (this._currentPage < this._document.PageCount)
            {
                //page
                var pageOrientation = GetOrientation(this._document.PageSizes[this._currentPage]);
                var currentPageHeight = GetInchFromPoint(this._document.PageSizes[this._currentPage].Height)*100;
                var currentPageWidth = GetInchFromPoint(this._document.PageSizes[this._currentPage].Width)*100;
                //print
                var printOrientation = GetOrientation(e.PageBounds.Size);
                //default setting page orientation from page setting
                e.PageSettings.Landscape = pageOrientation == Orientation.Landscape;
                
                double left;
                double top;
                double width;
                double height;
                //

                //margin setting
                if (this._printMode == PdfPrintMode.ShrinkToMargin)
                {
                    left = 0;
                    top = 0;
                    width = e.PageBounds.Width - e.PageSettings.HardMarginX * 2;
                    height = e.PageBounds.Height - e.PageSettings.HardMarginY * 2;
                }
                else
                {
                    left = -e.PageSettings.HardMarginX;
                    top = -e.PageSettings.HardMarginY;
                    width = e.PageBounds.Width;
                    height = e.PageBounds.Height;
                }
                //use orientation setting from perinter setting.
                if (pageOrientation != printOrientation)
                {
                    double tmp = width;
                    width = height;
                    height = tmp;

                    tmp = left;
                    left = top;
                    top = tmp;
                }
                //scaling setting
                double writableWidth;
                double writableHeight;
                switch (this.Scaling)
                {
                    case PdfPrintPageScaling.ActualSize:
                        left = 0;
                        top = 0;
                        width = currentPageWidth;
                        height = currentPageHeight;
                        break;
                    case PdfPrintPageScaling.CustomScale:
                        left = 0;
                        top = 0;
                        width = currentPageWidth *this.CustomScaling / 100;
                        height = currentPageHeight *this.CustomScaling / 100;;
                        break;
                    case PdfPrintPageScaling.FitSize://
                        //left = e.PageSettings.HardMarginX;
                        //top = e.PageSettings.HardMarginY;
                        writableWidth = Convert.ToDouble(e.PageBounds.Width - e.PageSettings.HardMarginX*2);
                        writableHeight = Convert.ToDouble(e.PageBounds.Height)- e.PageSettings.HardMarginY*2;
                        if (writableHeight/writableWidth >= currentPageHeight/currentPageWidth)//extra writable space in heigh
                        {
                            width = writableWidth;
                            height = writableWidth/currentPageWidth*currentPageHeight;
                            left = 0;//e.PageSettings.HardMarginX;
                            top = (e.PageBounds.Height - height) / 2 - e.PageSettings.HardMarginY;
                        }
                        else
                        {
                            height = writableHeight;
                            width = writableHeight / currentPageHeight * currentPageWidth;
                            top = 0;//e.PageSettings.HardMarginY;
                            left = (e.PageBounds.Width - width) / 2 - e.PageSettings.HardMarginX;
                        }
                        break;
                    case PdfPrintPageScaling.ShrinkOversized:
                        writableWidth = Convert.ToDouble(e.PageBounds.Width - e.PageSettings.HardMarginX*2);
                        writableHeight = Convert.ToDouble(e.PageBounds.Height)- e.PageSettings.HardMarginY*2;
                        if (writableWidth >= currentPageWidth && writableHeight >= currentPageHeight)
                        {
                            left = 0;
                            top = 0;
                            width = currentPageWidth;
                            height = currentPageHeight;
                        }
                        else
                        {
                            if (writableHeight / writableWidth >= currentPageHeight / currentPageWidth)//extra writable space in heigh
                            {
                                width = writableWidth;
                                height = writableWidth / currentPageWidth * currentPageHeight;
                                left = 0;//e.PageSettings.HardMarginX;
                                top = (e.PageBounds.Height - height) / 2 - e.PageSettings.HardMarginY;
                            }
                            else
                            {
                                height = writableHeight;
                                width = writableHeight / currentPageHeight * currentPageWidth;
                                top = 0;//e.PageSettings.HardMarginY;
                                left = (e.PageBounds.Width - width) / 2 - e.PageSettings.HardMarginX;
                            }
                        }
                        break;
                }
                //render to graphics
                this._document.Render(this._currentPage++,
                    e.Graphics,
                    e.Graphics.DpiX,
                    e.Graphics.DpiY,
                    new Rectangle(
                        AdjustDpi(e.Graphics.DpiX, left),
                        AdjustDpi(e.Graphics.DpiY, top),
                        AdjustDpi(e.Graphics.DpiX, width),
                        AdjustDpi(e.Graphics.DpiY, height)
                    ),
                    PdfRenderFlags.ForPrinting | PdfRenderFlags.Annotations
                );
            }

            int pageCount = this.PrinterSettings.ToPage == 0
                ? this._document.PageCount
                : Math.Min(this.PrinterSettings.ToPage, this._document.PageCount);

            e.HasMorePages = this._currentPage < pageCount;
        }

        private static int AdjustDpi(double value, double dpi)
        {
            return (int)((value / 100.0) * dpi);
        }

        private Orientation GetOrientation(SizeF pageSize)
        {
            if (pageSize.Height > pageSize.Width)
                return Orientation.Portrait;
            return Orientation.Landscape;
        }

        private enum Orientation
        {
            Portrait,
            Landscape
        }

    }
}
