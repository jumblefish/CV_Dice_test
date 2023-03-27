using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Dice_test
{
    internal class TriangleRectangle
    {
        private VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        private UMat cannyEdges; 
        private List<Triangle2DF> triangleList = new List<Triangle2DF>();
        private List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

        public List<Triangle2DF> GetTriangleList() { return triangleList; }
        public List<RotatedRect> GetRectangleList() { return boxList; }
        public TriangleRectangle(UMat canEdges)
        {
            this.cannyEdges = canEdges;
            CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List,
                     ChainApproxMethod.ChainApproxSimple);
            this.ExtractTriangleRectangle();
        }
        private void ExtractTriangleRectangle()
        {
            for (int i = 0; i < contours.Size; i++) //for each contour,
            {
                using (VectorOfPoint contour = contours[i])
                using (VectorOfPoint approxContour = new VectorOfPoint())
                {
                    CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                    if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
                    {

                        if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                        {
                            System.Drawing.Point[] pts = approxContour.ToArray();
                            triangleList.Add(new Triangle2DF(
                                pts[0],
                                pts[1],
                                pts[2]
                            ));
                        }
                        else if (approxContour.Size == 4) //The contour has 4 vertices.
                        {
                            #region determine if all the angles in the contour are within [80, 100] degree
                            bool isRectangle = true;
                            System.Drawing.Point[] pts = approxContour.ToArray();
                            LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                            for (int j = 0; j < edges.Length; j++)
                            {
                                double angle = Math.Abs(
                                    edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                if (angle < 80 || angle > 100)
                                {
                                    isRectangle = false;
                                    break;
                                }
                            }

                            #endregion

                            if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                        }
                    }
                }
            }
        }
    }
}
