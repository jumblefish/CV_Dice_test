using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.CvEnum;
using System.Text.RegularExpressions;
using System.Drawing.Text;
using System.IO;
using Emgu.CV.Flann;
using Emgu.CV.Features2D;
using Emgu.CV.Linemod;
using Emgu.CV.Face;
using Emgu.CV.Util;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CV_Dice_test
{
    internal class CV_Dice_test
    {
        static void Main(string[] args)
        {
            //If you are targeting .Net Framework, when using Emgu.CV.runtime.windows(.dldt / .cuda / .cuda.dldt) nuget packages for windows, please set the build architecture to either "x86" or "x64". Do not set the architecture to "Any CPU

            string imagePath = "C:\\Pictures\\Opencvpic3sample.png";
            PreProcess prep = new PreProcess(imagePath);
            prep.ConvertToGrayscale();
            prep.BlurImage();
            Mat blurredGrayImage = prep.GetImage();
            LineObject lineObj = new LineObject(blurredGrayImage);
            lineObj.GenerateLines();
            TriangleRectangle triangleRectangle = new TriangleRectangle(lineObj.GetCannyEdges());
            DrawShapes shapes = new DrawShapes(prep.GetImageSize());
            shapes.DrawRectangles(triangleRectangle.GetRectangleList());
            shapes.DrawTriangles(triangleRectangle.GetTriangleList());
            DisplayImage display = new DisplayImage(shapes.GetTRImage());
            display.CreateWindow();


        }
    }
}


