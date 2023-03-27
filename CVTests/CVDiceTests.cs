using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CV_Dice_test;
using System.IO;
using System.Linq;

namespace CVTests
{
    [TestClass]
    public class CVDiceTests
    {
        public static bool AreFileContentsEqual(String path1, String path2) =>
              File.ReadAllBytes(path1).SequenceEqual(File.ReadAllBytes(path2));

        [TestMethod]
        public void TestImageBlur()
        {
            string imagePath = "C:\\source\\repos\\CV_Dice_test\\CV_Dice_test\\Assets\\Opencvpic3sample.png";
            string blurredImage = "C:\\source\\repos\\CV_Dice_test\\CV_Dice_test\\Assets\\Opencvpic3sampleblur.png";
            string goldenImage = "C:\\source\\repos\\CV_Dice_test\\CV_Dice_test\\Assets\\goldenBlur.png";
            PreProcess prep = new PreProcess(imagePath);
            prep.BlurImage();
            prep.SaveImage(blurredImage);
            Assert.IsTrue(AreFileContentsEqual(blurredImage, goldenImage));
        }
        [TestMethod]
        public void TestConvertToGrayScale()
        {
            string imagePath = "C:\\source\\repos\\CV_Dice_test\\CV_Dice_test\\Assets\\Opencvpic3sample.png";
            string grayScaleImage = "C:\\source\\repos\\CV_Dice_test\\CV_Dice_test\\Assets\\Opencvpic3sampleGrayScale.png";
            string goldenImage = "C:\\source\\repos\\CV_Dice_test\\CV_Dice_test\\Assets\\goldenGrayScale.png";
            PreProcess prep = new PreProcess(imagePath);
            prep.ConvertToGrayscale();
            prep.SaveImage(grayScaleImage);
            Assert.IsTrue(AreFileContentsEqual(grayScaleImage, goldenImage));
        }
    }
}
