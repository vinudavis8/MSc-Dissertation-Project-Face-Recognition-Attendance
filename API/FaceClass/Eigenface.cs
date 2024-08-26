using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using System.Reflection.Emit;


namespace FaceClass
{
  

    public class FaceData
    {
        public string PersonName { get; set; }
        public Image<Gray, byte> FaceImage { get; set; }
        public DateTime CreateDate { get; set; }

    }
    public static class CConfig
    {
        public static string FacePhotosPath = "Source\\Faces\\";
        public static string FaceListTextFile = "Source\\FaceList.txt";
        public static string HaarCascadePath = "Resources\\haarcascade_frontalface_default.xml";
        public static int TimerResponseValue = 500;
        public static string ImageFileExtension = ".bmp";
        public static int ActiveCameraIndex = 0;//0: Default active camera device
        public static string errorFilePath = "error.txt";
        public static string HistogramImagePath = "Source\\Histogram\\";
        public static string TrainingSetPath = "Source\\TrainingSet\\";


    }
    public class FaceClassR
        {

        private CascadeClassifier haarCascade;
        private Image<Bgr, Byte> bgrFrame = null;
        private Image<Gray, Byte> detectedFace = null;
        private List<FaceData> faceList = new List<FaceData>();
        private VectorOfMat imageList = new VectorOfMat();
        private List<string> nameList = new List<string>();
        private VectorOfInt labelList = new VectorOfInt();

        private EigenFaceRecognizer recognizer;
        public string FaceName;




        private void RegisterNewFace(string studentId)
        {
            if (detectedFace == null)
            {
                return;
            }
            //Save detected face
            detectedFace = detectedFace.Resize(200, 200, Inter.Cubic);
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string FacePhotosPath = Path.Combine(assemblyDirectory, CConfig.FacePhotosPath);
            string FaceListTextFile = Path.Combine(assemblyDirectory, CConfig.FaceListTextFile);

            detectedFace.Save(FacePhotosPath + "face" + (faceList.Count + 1) + CConfig.ImageFileExtension);
            StreamWriter writer = new StreamWriter(FaceListTextFile, true);
            string personName = studentId;
            FaceName = studentId;
            writer.WriteLine(String.Format("face{0}:{1}", (faceList.Count + 1), studentId));
            writer.Close();
            GetFacesList();
        }

        public void GetFacesList()
        {

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string HaarCascadePath = Path.Combine(assemblyDirectory, CConfig.HaarCascadePath);
            string FacePhotosPath = Path.Combine(assemblyDirectory, CConfig.FacePhotosPath);
            string FaceListTextFile = Path.Combine(assemblyDirectory, CConfig.FaceListTextFile);
            //haar cascade classifier
            if (!File.Exists(HaarCascadePath))
            {
                string text = "Cannot find Haar cascade data file:\n\n";
                text += CConfig.HaarCascadePath;
            }

            haarCascade = new CascadeClassifier(HaarCascadePath);
            faceList.Clear();
            string line;
            FaceData faceInstance = null;

            // Create empty directory / file for face data if it doesn't exist
            if (!Directory.Exists(FacePhotosPath))
            {
                Directory.CreateDirectory(FacePhotosPath);
            }

            if (!File.Exists(FaceListTextFile))
            {
                string text = "Cannot find face data file:\n\n";
                text += CConfig.FaceListTextFile + "\n\n";
                text += "If this is your first time running the app, an empty file will be created for you.";
                String dirName = Path.GetDirectoryName(FaceListTextFile);
                Directory.CreateDirectory(dirName);
                File.Create(FaceListTextFile).Close();
            }

            StreamReader reader = new StreamReader(FaceListTextFile);
            int i = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] lineParts = line.Split(':');
                faceInstance = new FaceData();
                faceInstance.FaceImage = new Image<Gray, byte>(FacePhotosPath + lineParts[0] + CConfig.ImageFileExtension);
                faceInstance.PersonName = lineParts[1];
                faceList.Add(faceInstance);
            }
            foreach (var face in faceList)
            {
                imageList.Push(face.FaceImage.Mat);
                nameList.Add(face.PersonName);
                labelList.Push(new[] { i++ });
            }
            reader.Close();

            // Train recogniser
            if (imageList.Size > 0)
            {
                recognizer = new EigenFaceRecognizer(imageList.Size);
                recognizer.Train(imageList, labelList);
            }

        }
       
        public string ProcessFrame(byte[] imageData,string StudentId=null)
        {
            GetFacesList();
            // Load the image data into an Emgu CV image (Bgr format)
            Image<Bgr, byte> bgrFrame;
            using (Mat imageMat = new Mat())
            {
                CvInvoke.Imdecode(imageData, ImreadModes.Color, imageMat);
                bgrFrame = imageMat.ToImage<Bgr, byte>();
            }
            // Image<Bgr, Byte> bgrFrame = BitmapToImage(bitmap);
            if (bgrFrame != null)
            {
                try
                {//for emgu cv bug
                    Image<Gray, byte> grayframe = bgrFrame.Convert<Gray, byte>();

                     Rectangle[] faces = haarCascade.DetectMultiScale(grayframe, 1.2, 10, new System.Drawing.Size(50, 50), new System.Drawing.Size(200, 200));
                     Rectangle[] faces1 = haarCascade.DetectMultiScale(grayframe, scaleFactor: 1.1, minNeighbors: 5, minSize: new System.Drawing.Size(30, 30), maxSize: new System.Drawing.Size(300, 300));
                    Rectangle[] facesc = haarCascade.DetectMultiScale(grayframe, 1.1, 3, System.Drawing.Size.Empty);
                    //Image<Gray, byte> grayFace = null;
                    //MCvAvgComp[][] facesDetectedNow = grayFace.DetectHaarCascade(faceDetected, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
                    //detect face
                    FaceName = "";
                    foreach (var face in facesc)
                    {
                        bgrFrame.Draw(face, new Bgr(255, 255, 0), 2);
                        detectedFace = bgrFrame.Copy(face).Convert<Gray, byte>();
                           FaceRecognition();
                        if (!string.IsNullOrEmpty(FaceName) && !string.IsNullOrEmpty(StudentId))
                            return " Face Already Registered : "+FaceName;
                        else if(facesc.Count()==0)
                            return "No Face Detected";
                        else if(!string.IsNullOrEmpty(StudentId))
                        { 
                            RegisterNewFace(StudentId);
                            return "Face Registered Successfully ";
                        }
                    }
                    return FaceName;
                }
                catch (Exception ex)
                {
                    return "";
                }

            }
            return "";
        }


        private void FaceRecognition()
        {
            if (imageList.Size != 0)
            {
                //Eigen Face Algorithm
                // Initialize the Eigenface recognizer
                EigenFaceRecognizer recognizer1 = new EigenFaceRecognizer();


                // Load the test image
                Image<Gray, byte> testImage = new Image<Gray, byte>("C:\\Users\\VINU\\Downloads\\vinu2020.jpg");

                // Recognize the face in the test image
                //var c = recognizer1.Predict(testImage);
               // FaceRecognizer.PredictionResult result = recognizer.Predict(testImage);

                FaceRecognizer.PredictionResult result = recognizer.Predict(detectedFace.Resize(200, 200, Inter.Cubic));
                FaceName = nameList[result.Label];
            }

        }
    }
}

