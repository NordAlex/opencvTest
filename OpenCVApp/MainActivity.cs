using System.IO;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using OpenCV.Android;
using OpenCV.Core;
using OpenCV.ImgProc;
using OpenCV.ObjDetect;
using OpenCVApp.Callbacks;
using static OpenCV.Android.CameraBridgeViewBase;
using Size = OpenCV.Core.Size;


namespace OpenCVApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.SensorLandscape)]
    public class MainActivity : AppCompatActivity, ICvCameraViewListener
    {

        public static string basePath = JavaSystem.GetProperty("user.dir");
        public static string classifierPath1 = basePath + "\\src\\resources\\OpenCVApp\\haarcascade_frontalface_alt.xml";
        public static string inpImgFilename = basePath + "\\src\\resources\\OpenCVApp\\input.jpg";
        public static string opImgFilename = basePath + "\\src\\resources\\OpenCVApp\\output.jpg";

        private JavaCameraView _openCvCameraView;
        private ImageView _faceImageView;

        private Mat _lastFrame;
         

        private OpenCVLoaderCalback _loaderCallback;
        private Button _cameraButton; 

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Window.AddFlags(WindowManagerFlags.KeepScreenOn); 

            _openCvCameraView = FindViewById<JavaCameraView>(Resource.Id.cameraView); 
            _openCvCameraView.SetCvCameraViewListener(this);

            _loaderCallback = new OpenCVLoaderCalback(_openCvCameraView, this);

            _faceImageView = FindViewById<ImageView>(Resource.Id.faceImageView);
            _cameraButton = FindViewById<Button>(Resource.Id.cameraButton);
            _cameraButton.Click += TakePicture;
            FaceRecognizer.


        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cameraButton.Click -= TakePicture; 
        }
 
        private void TakePicture(object sender, System.EventArgs e)
        {
            var faceFrame = DetectAndDisplay(_lastFrame);
            Android.Graphics.Bitmap bitmap = Android.Graphics.Bitmap.CreateBitmap(faceFrame.Width(), faceFrame.Height(), Android.Graphics.Bitmap.Config.Argb8888);
            Utils.MatToBitmap(faceFrame, bitmap);
            _faceImageView.SetImageBitmap(bitmap);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!OpenCVLoader.InitDebug())
            {
                OpenCVLoader.InitAsync(OpenCVLoader.OpencvVersion300, this, _loaderCallback);
            }
            else
            {
                _loaderCallback.OnManagerConnected(LoaderCallbackInterface.Success);
            }
        }

        public Mat OnCameraFrame(Mat inputFrame)
        { 
            _lastFrame = inputFrame;
            return inputFrame;  
        }

        public void OnCameraViewStarted(int width, int height)
        {
        }

        public void OnCameraViewStopped()
        {

        }

        public Mat DetectAndDisplay(Mat frame)
        {
            MatOfRect faces = new MatOfRect();
            Mat grayFrame = new Mat();
            int absoluteFaceSize = 0;

            CascadeClassifier faceCascade = new CascadeClassifier();
            string path = CopyFaceDetector();  
            faceCascade.Load(path); 

            // convert the frame in gray scale
            Imgproc.CvtColor(frame, grayFrame, Imgproc.ColorRgb2gray);
            // equalize the frame histogram to improve the result
            Imgproc.EqualizeHist(grayFrame, grayFrame);

            // compute minimum face size (1% of the frame height, in our case)

            int height = grayFrame.Rows();
            if (Java.Lang.Math.Round(height * 0.2f) > 0)
            {
                absoluteFaceSize = Java.Lang.Math.Round(height * 0.01f);
            }

            // detect faces
            faceCascade.DetectMultiScale(grayFrame, faces, 1.1, 2, 0 | Objdetect.CascadeScaleImage,
                        new Size(absoluteFaceSize, absoluteFaceSize), new Size(height, height));

            // each rectangle in faces is a face: draw them!
            Rect[] facesArray = faces.ToArray();

            for (int i = 0; i < facesArray.Length; i++)
                Imgproc.Rectangle(frame, facesArray[i].Tl(), facesArray[i].Br(), new Scalar(0, 255, 0), 2);

            return frame; 
        }

        public string CopyFaceDetector()
        {
            var faceDetectorPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "haarcascade_frontalface_alt.xml");

            if (!File.Exists(faceDetectorPath))
            {
                    using (var dbAssetStream = Assets.Open("haarcascade_frontalface_alt.xml"))
                    using (var dbFileStream = new FileStream(faceDetectorPath, FileMode.OpenOrCreate))
                    {
                        var buffer = new byte[1024];

                        int b = buffer.Length;
                        int length;

                        while ((length = dbAssetStream.Read(buffer, 0, b)) > 0)
                        {
                            dbFileStream.Write(buffer, 0, length);
                        }

                        dbFileStream.Flush();
                        dbFileStream.Close();
                        dbAssetStream.Close();
                    }
                
            }
            return faceDetectorPath;
        }

    }


}

