using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
namespace PdfiumViewer.Test
{
    [TestFixture]
    public class PdfTextFunctionTest
    {
        [Test]
        public void TestGetBoundedText()
        {
            var testfile =
                @"C:\TEST\TestFolder_CPF\YSOA PDF checking\PdfPerformance\YSOA_2016_0A664FF4-S133-D846-A061-F43A6BA5446D.pdf";
            using (var pdfDoc =PdfDocument.Load(testfile))
            {
                var text = pdfDoc.GetBoundedText(0, 10, 10, 500, 500);
                Debug.WriteLine(text);
            }


            Debug.WriteLine("suc");
        }


        [Test]
        public void TestGetMultiBoundedText()
        {
            var testfile =
                @"C:\TEST\TestFolder_CPF\YSOA PDF checking\PdfPerformance\YSOA_2016_0A664FF4-S133-D846-A061-F43A6BA5446D.pdf";
            using (var pdfDoc = PdfDocument.Load(testfile))
            {
                var texts = pdfDoc.GetMultiBoundedText(0, new Bound[]
                {
                    //index 1, 
                    new Bound(360, 841- 252, 552,841 - 313),
                    //index 2, 
                    new Bound(253, 841 - 342, 325, 841-356 ),
                    //index 3, 
                    new Bound(327, 841-341, 398,841- 356),
                    //index 5, 
                    new Bound(401,841- 341, 471,841- 357),
                    //index 6, 
                    new Bound(474,841- 341, 543,841- 357),

                    //new Bound(379, 542, 502, 540),
                });
                Debug.WriteLine(texts);
            }


            Debug.WriteLine("suc");
        }
    }
}