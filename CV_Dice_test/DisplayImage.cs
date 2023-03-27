using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Dice_test
{
    internal class DisplayImage
    {
        private string imageName;
        private Mat image;
        public DisplayImage(Mat image, string imageName = "Default")
        {
            this.imageName = imageName;
            this.image = image;
        }
        public Mat GetImage => image; 
        public void CreateWindow() 
        {
            CvInvoke.NamedWindow(imageName);
            CvInvoke.Imshow(imageName, image);
            CvInvoke.WaitKey(0);
            CvInvoke.DestroyWindow(imageName);
        }

    }
}
