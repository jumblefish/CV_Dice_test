using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Dice_test
{
    public class PreProcess
    {
        private string imageToLoad;
        private Mat imageMat;

        public PreProcess(string image)//=> imageToLoad = image;
        {
            imageToLoad = image;
            imageMat = new Mat(image);
        }

        public Mat GetImage() => imageMat;
        public Size GetImageSize () => imageMat.Size;


        public void ConvertToGrayscale()
        {
            CvInvoke.CvtColor(imageMat, imageMat, ColorConversion.Bgr2Gray);
        }

        public void BlurImage()
        {
            CvInvoke.GaussianBlur(imageMat, imageMat, new Size(3, 3), 1);
        }
        public void SaveImage(string path)
        {
            imageMat.Save(path);
        }
    }
}
