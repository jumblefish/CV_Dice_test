using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CV_Dice_test
{
    internal class LineObject
    {
        private double cannyThreshold; //default 180
        private double cannyThresholdLinking; //default 120
        private UMat cannyEdges = new UMat();
        LineSegment2D[] lines;

        public LineObject(Mat grayScaleImage, double cannyThresh = 280.0, double cannyThreshLink = 120.0) 
        {
            this.cannyThreshold = cannyThresh;
            this.cannyThresholdLinking = cannyThreshLink;
            CvInvoke.Canny(grayScaleImage, cannyEdges, cannyThreshold, cannyThresholdLinking);
        }

        public void GenerateLines()
        {
            lines = CvInvoke.HoughLinesP(
                    cannyEdges,
                    1, //Distance resolution in pixel-related units, default 1
                    Math.PI / 45.0, //Angle resolution measured in radians. default pi/45
                    20, //threshold default 20
                    30, //min Line width default 30
                    10); //gap between lines default 10
        }
        public LineSegment2D[] GetLines() => lines;
        public UMat GetCannyEdges () => cannyEdges;
        
    }
}
