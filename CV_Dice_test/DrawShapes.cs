using Emgu.CV.CvEnum;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.Structure;

namespace CV_Dice_test
{
    internal class DrawShapes 
    {
        Mat triangleRectangleImage;
        public Mat GetTRImage() => triangleRectangleImage;
        public DrawShapes (Size size)
        {
            triangleRectangleImage = new Mat(size, DepthType.Cv8U, 3);
            triangleRectangleImage.SetTo(new MCvScalar(0));
        }

        public void DrawTriangles(List<Triangle2DF> triangleList)
        {
            foreach (Triangle2DF triangle in triangleList)
            {
                CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(triangle.GetVertices(), System.Drawing.Point.Round),
                    true, new Bgr(Color.DarkBlue).MCvScalar, 2);
            }
        }

        public void DrawRectangles(List<RotatedRect> boxList)
        {
            foreach (RotatedRect box in boxList)
            {
                CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(box.GetVertices(), System.Drawing.Point.Round), true,
                    new Bgr(Color.DarkOrange).MCvScalar, 2);
            }
        }
    }
}
