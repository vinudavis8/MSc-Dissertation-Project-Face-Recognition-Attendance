using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FaceClass
{

    public class LBPHFace
    {
        private CascadeClassifier haarCascade;
        private FaceRecognizer recognizer;
        private List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        private List<int> labels = new List<int>();
        string assemblyDirectory = "";
        string facePhotosPath = "";
        string faceListTextFile = "";
        string histogramPath = "";
        string modelPath = "trainedModel.xml";
        string trainingSet = "";

        public LBPHFace()
        {
            haarCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            recognizer = new LBPHFaceRecognizer();
            assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            facePhotosPath = Path.Combine(assemblyDirectory, CConfig.FacePhotosPath);
            faceListTextFile = Path.Combine(assemblyDirectory, CConfig.FaceListTextFile);
            histogramPath = Path.Combine(assemblyDirectory, CConfig.HistogramImagePath);
            trainingSet= Path.Combine(assemblyDirectory, CConfig.TrainingSetPath); 
            CreateDirectories();
            LoadExistingTrainingData();

            // Load existing model if available and valid
            if (File.Exists(modelPath))
            {
                try
                {
                    recognizer.Read(modelPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading model: " + ex.Message);
                    Console.WriteLine("Initiating training...");
                }
            }

        }

        public bool TrainModelFromSource()
        {

            
          
            return true;
        }


        public void CreateDirectories()
        {
            // Create empty directory / file for face data if it doesn't exist
            if (!Directory.Exists(facePhotosPath))
            {
                Directory.CreateDirectory(facePhotosPath);
            }
            if (!Directory.Exists(histogramPath))
            {
                Directory.CreateDirectory(histogramPath);
            }
            if (!Directory.Exists(trainingSet))
            {
                Directory.CreateDirectory(trainingSet);
            }
            if (!File.Exists(faceListTextFile))
            {
                string text = "Cannot find face data file:\n\n";
                text += CConfig.FaceListTextFile + "\n\n";
                text += "If this is your first time running the app, an empty file will be created for you.";
                String dirName = Path.GetDirectoryName(faceListTextFile);
                Directory.CreateDirectory(dirName);
                File.Create(faceListTextFile).Close();
            }
        }
        private void LoadExistingTrainingData()
        {
            if (File.Exists(faceListTextFile))
            {
                var lines = File.ReadAllLines(faceListTextFile);
                foreach (var line in lines)
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        var imagePath = Path.Combine(facePhotosPath, parts[0] + ".bmp"); // Update extension if needed
                        if (File.Exists(imagePath))
                        {
                            trainingImages.Add(new Image<Gray, byte>(imagePath));
                            labels.Add(int.Parse(parts[1]));
                        }
                    }
                }
            }
        }

        public void TrainModel(string studentId, Image<Bgr, byte> bgrFrame)
        {
            try
            {
                var grayFrame = bgrFrame.Convert<Gray, byte>();
                var faces = haarCascade.DetectMultiScale(grayFrame, 1.1, 5, new Size(30, 30));

                foreach (var face in faces)
                {
                    var detectedFace = grayFrame.Copy(face).Resize(200, 200, Inter.Cubic);
                    detectedFace._EqualizeHist(); // Normalize the image
                    detectedFace.Save(facePhotosPath + "face_" + studentId + "_"+(trainingImages.Count + 1)+ CConfig.ImageFileExtension);
                    using (StreamWriter writer = new StreamWriter(faceListTextFile, true))
                    {
                        writer.WriteLine($"face{trainingImages.Count + 1}:{studentId}");
                    }
                    trainingImages.Add(detectedFace);
                    labels.Add(int.Parse(studentId));             
                }
                if (trainingImages.Count > 0)
                {
                    using (var imagesVector = new VectorOfMat())
                    using (var labelsVector = new VectorOfInt(labels.ToArray()))
                    {
                        foreach (var img in trainingImages)
                        {
                            imagesVector.Push(new Mat[] { img.Mat });
                        }
                        recognizer.Train(imagesVector, labelsVector);
                        recognizer.Write(modelPath); // Use Write method to save the model
                        SaveHistogramImage(grayFrame.Mat, studentId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveHistogramImage(Mat grayImage,string studentId)
        {
            // Calculate the histogram
            VectorOfMat vm = new VectorOfMat();
            vm.Push(new Mat[] { grayImage });
            Mat hist = new Mat();
            int[] histSize = { 256 };
            float[] range = { 0, 256 };
            int[] channels = { 0 };
            CvInvoke.CalcHist(vm, channels, null, hist, histSize,  range , false);

            // Create the histogram image
            int hist_w = 512; int hist_h = 400;
            int bin_w = (int)((double)hist_w / histSize[0]);
            Mat histImage = new Mat(hist_h, hist_w, DepthType.Cv8U, 1);
            histImage.SetTo(new MCvScalar(255));

            // Normalize the result to [0, histImage.Rows]
            CvInvoke.Normalize(hist, hist, 0, histImage.Rows, NormType.MinMax, DepthType.Cv32F);

            // Convert histogram to a float array
            float[] histData = new float[histSize[0]];
            hist.CopyTo(histData);
            // Draw for each channel
            for (int i = 1; i < histSize[0]; i++)
            {
                CvInvoke.Line(histImage,
                    new Point(bin_w * (i - 1), hist_h - (int)Math.Round(histData[i - 1])),
                    new Point(bin_w * i, hist_h - (int)Math.Round(histData[i])),
                    new MCvScalar(0), 2, LineType.EightConnected, 0);
            }

            string histPath = histogramPath+$"histogram_{studentId}.bmp";
            // Save the histogram image
            // Save the histogram image
            if (!CvInvoke.Imwrite(histPath, histImage))
            {
                throw new Exception($"Failed to save histogram image to: {histPath}");
            }
            //histImage.Save(histogramPath);
        }

        public string RecognizeFace(Image<Bgr, byte> bgrFrame)
        {
            try
            {
                var grayFrame = bgrFrame.Convert<Gray, byte>();
                var faces = haarCascade.DetectMultiScale(grayFrame, 1.1, 5, new Size(30, 30));

                foreach (var face in faces)
                {
                    var detectedFace = grayFrame.Copy(face).Resize(200, 200, Inter.Cubic);
                    detectedFace._EqualizeHist(); // Normalize the image

                    var result = recognizer.Predict(detectedFace);

                    if (result.Label != -1 && result.Distance < 45)// Adjust distance threshold
                    {
                        return result.Label.ToString();
                    }
                    else
                    {
                        return "0";
                    }
                }

                return "-1";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
