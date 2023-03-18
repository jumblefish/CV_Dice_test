using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Text;
using System.IO;
using Emgu.CV.Flann;
using Emgu.CV.Features2D;
using Emgu.CV.Linemod;
using Emgu.CV.Face;
using Emgu.CV.Util;
using System.Runtime.InteropServices.WindowsRuntime;
using Dbscan;

namespace CV_Dice_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //If you are targeting .Net Framework, when using Emgu.CV.runtime.windows(.dldt / .cuda / .cuda.dldt) nuget packages for windows, please set the build architecture to either "x86" or "x64". Do not set the architecture to "Any CPU

            /*
            string inputFileName = "C:\\Users\\nz\\Pictures\\always1.png";//args[0];
            string outputFileName = $"{Path.GetFileNameWithoutExtension(inputFileName)}-gray.jpg";
            using (var image = new Mat(inputFileName))
                
            using (var gray = image.CvtColor(ColorConversionCodes.BGR2GRAY))
                gray.SaveImage(outputFileName);
            */

            Mat local_image = new Mat("C:\\Users\\nz\\Pictures\\dice2.jpg");
           // Mat local_image = new Mat("C:\\Users\\nz\\Pictures\\Opencvpic3sample.png");

            Mat highlightedImaged = ProcessImageML(local_image);

            String diceImage = "Image";
            CvInvoke.NamedWindow(diceImage);
            CvInvoke.Imshow(diceImage, highlightedImaged);
            CvInvoke.WaitKey(0);
            CvInvoke.DestroyWindow(diceImage);


            //this is limited to perfect square dice
            Mat ProcessImage(Mat img)
            {
                using (UMat gray = new UMat())
                using (UMat cannyEdges = new UMat())
                using (Mat triangleRectangleImage = new Mat(img.Size, DepthType.Cv8U, 3)) //image to draw triangles and rectangles on
                {
                    CvInvoke.CvtColor(img, gray, ColorConversion.Bgr2Gray);
                    CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 1);
                    double cannyThreshold = 280.0; //default 180
                    double cannyThresholdLinking = 120.0; //default 120
                    CvInvoke.Canny(gray, cannyEdges, cannyThreshold, cannyThresholdLinking);
                    LineSegment2D[] lines = CvInvoke.HoughLinesP(
                        cannyEdges,
                        1, //Distance resolution in pixel-related units, default 1
                        Math.PI / 45.0, //Angle resolution measured in radians. default 45
                        20, //threshold default 20
                        30, //min Line width default 30
                        10); //gap between lines default 10
                    //List<Triangle2DF> triangleList = new List<Triangle2DF>();
                    List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

                    //Find triangles and rectangles
                    using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                    {
                        CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List,
                            ChainApproxMethod.ChainApproxSimple);
                        int count = contours.Size;
                        for (int i = 0; i < count; i++) //for each contour,
                        {
                            using (VectorOfPoint contour = contours[i])
                            using (VectorOfPoint approxContour = new VectorOfPoint())
                            {
                                CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05,
                                    true);
                                if (CvInvoke.ContourArea(approxContour, false) > 250
                                ) //only consider contours with area greater than 250
                                {
                                    /*
                                    if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                                    {
                                        Point[] pts = approxContour.ToArray();
                                        triangleList.Add(new Triangle2DF(
                                            pts[0],
                                            pts[1],
                                            pts[2]
                                        ));
                                    }
                                    else 
                                    */
                                    if (approxContour.Size == 4) //The contour has 4 vertices.
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

                    #region draw triangles and rectangles
                    triangleRectangleImage.SetTo(new MCvScalar(0));
                    /*
                    foreach (Triangle2DF triangle in triangleList)
                    {
                        CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(triangle.GetVertices(), Point.Round),
                            true, new Bgr(Color.DarkBlue).MCvScalar, 2);
                    }*/

                    foreach (RotatedRect box in boxList)
                    {
                        //CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(box.GetVertices(), Point.Round), true,
                        CvInvoke.Polylines(img, Array.ConvertAll(box.GetVertices(), System.Drawing.Point.Round), true,
                            new Bgr(Color.DarkOrange).MCvScalar, 2);
                    }

                    //Drawing a light gray frame around the image
                    CvInvoke.Rectangle(img,
                        new Rectangle(System.Drawing.Point.Empty,
                            new Size(img.Width - 1, img.Height - 1)),
                        new MCvScalar(120, 120, 120));
                    //Draw the labels
                    CvInvoke.PutText(img, "Rectangles", new System.Drawing.Point(20, 20),
                        FontFace.HersheyDuplex, 0.5, new MCvScalar(120, 120, 120));
                    #endregion

                    Mat result = new Mat();
                    //CvInvoke.VConcat(new Mat[] { img, triangleRectangleImage, circleImage, lineImage }, result);
                    //CvInvoke.VConcat(new Mat[] { img, triangleRectangleImage }, result);
                    img.Save("C:\\Users\\nz\\Pictures\\Opencvpic3sample_save.png");

                    return img;
                }


            }

            Mat ProcessImageML(Mat imageToProcess)
            {
                using (Mat gray = new Mat())
                //using (UMat cannyEdges = new UMat())
                //using (Mat triangleRectangleImage = new Mat(imageToProcess.Size, DepthType.Cv8U, 3)) //image to draw triangles and rectangles on
                {
                    CvInvoke.CvtColor(imageToProcess, gray, ColorConversion.Bgr2Gray);
                    CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 1);

                    var grayArray = gray.GetData();
                    IEnumerable<IPointData> grayArrayIPointData = grayArray.Cast<IPointData>();

                    Console.WriteLine(grayArray.Length);
                    Console.WriteLine(grayArrayIPointData.Count());


                    //gray.CopyTo

                    ClusterSet<IPointData> clusters = Dbscan.Dbscan.CalculateClusters(
                        grayArrayIPointData,
                        epsilon: 40.0,
                        minimumPointsPerCluster: 0);

                    Console.WriteLine(clusters.Clusters.Count());


                    return null;
                }
            }

            Image<Bgr, Byte> GetFacePoints()
            {
                CascadeClassifier faceDetector = new CascadeClassifier(@"..\..\Resource\EMGUCV\haarcascade_frontalface_default.xml");
                FacemarkLBFParams fParams = new FacemarkLBFParams();
                fParams.ModelFile = @"..\..\Resource\EMGUCV\lbfmodel.yaml";
                fParams.NLandmarks = 68; // number of landmark points
                fParams.InitShapeN = 10; // number of multiplier for make data augmentation
                fParams.StagesN = 5; // amount of refinement stages
                fParams.TreeN = 6; // number of tree in the model for each landmark point
                fParams.TreeDepth = 5; //he depth of decision tree
                FacemarkLBF facemark = new FacemarkLBF(fParams);
                //facemark.SetFaceDetector(MyDetector);

                Image<Bgr, Byte> image = new Image<Bgr, byte>(@"C:\Users\nz\Pictures\dice1.jpg");
                Image<Gray, byte> grayImage = image.Convert<Gray, byte>();
                grayImage._EqualizeHist();

                VectorOfRect faces = new VectorOfRect(faceDetector.DetectMultiScale(grayImage));
                VectorOfVectorOfPointF landmarks = new VectorOfVectorOfPointF();
                facemark.LoadModel(fParams.ModelFile);

                bool success = facemark.Fit(grayImage, faces, landmarks);
                if (success)
                {
                    Rectangle[] facesRect = faces.ToArray();
                    for (int i = 0; i < facesRect.Length; i++)
                    {
                        image.Draw(facesRect[i], new Bgr(Color.Blue), 2);
                        FaceInvoke.DrawFacemarks(image, landmarks[i], new Bgr(Color.Blue).MCvScalar);
                    }
                    return image;
                }
                return null;
            }

            void GetDiceFromBlobs(MKeyPoint[] blobs)
            {
                List<System.Drawing.PointF> points = new List<PointF>();
                //PointF[] points =
                foreach (MKeyPoint b in blobs)
                {
                    var pos = b.Point;
                    points.Add(pos);
                }

                if(points.Count() > 0)
                {
                    FacemarkLBFParams fParams = new FacemarkLBFParams();
                    //fParams.
                    FacemarkLBF facemark = new FacemarkLBF(fParams);
                   // PointCollection.BoundingRectangle
                   // var clustering = FaceInvoke.Fit();
                }

            }

            MKeyPoint[] GetBlobs(Mat frame)
            {
                SimpleBlobDetectorParams param = new SimpleBlobDetectorParams();
                param.FilterByInertia = true;
                param.MinInertiaRatio = 0.6F;
                SimpleBlobDetector det = new SimpleBlobDetector(param);
                Mat blur = frame;//these might need to be newed
                Mat gray = frame;
                CvInvoke.MedianBlur(frame, blur, 7); //3,5,7?
                CvInvoke.CvtColor(blur, gray, ColorConversion.Bgr2Gray);
                var blobs = det.Detect(gray);
                return blobs;
            }

            void captureCamera()
            {
                VideoCapture capture = new VideoCapture();
                capture.Start(); // same as capture.open(0); ?

                String win1 = "Image";
                CvInvoke.NamedWindow(win1);
                while (true)
                {
                    Mat image = new Mat(); //= new Mat("C:\\Users\\nz\\Pictures\\always5.png");
                    capture.Read(image);
                    CvInvoke.Imshow("Image", image);
                    CvInvoke.WaitKey(0);
                    CvInvoke.DestroyWindow(win1);
                }
            }

            /*
            void helloworld()
            {

                String win2 = "MainWindow";
                CvInvoke.NamedWindow(win2);
                Mat img = new Mat(200, 400, DepthType.Cv8U, 3); //Create a 3 channel image of 400x200
                img.SetTo(new Bgr(255, 0, 0).MCvScalar); // set it to Blue color

                 //Draw "Hello, world." on the image using the specific font
                CvInvoke.PutText(
               img,
               "Hello, world",
               new System.Drawing.Point(10, 80),
               FontFace.HersheyComplex,
               1.0,
               new Bgr(0, 255, 0).MCvScalar);


                CvInvoke.Imshow(win2, img); //Show the image
                CvInvoke.WaitKey(0);  //Wait for the key pressing event
               CvInvoke.DestroyWindow(win2); //Destroy the window if key is pressed
            }
            */
            //CvInvoke.cvNamedWindow(win1);
            /*
            using (Image<Bgr, Byte> img = new Image<Bgr, byte>(400, 200, new Bgr(255, 0, 0)))

            {

                // assigning font

                MCvFont f = new MCvFont(CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX, 1.0, 1.0);

                // drawing on the image using the specified font

                img.Draw("Hello, world", ref f, new Point(10, 80), new Bgr(0, 255, 0));

                // displaying the image

                CvInvoke.cvShowImage(win1, img.Ptr);

            }
            */
        }
    }
}


