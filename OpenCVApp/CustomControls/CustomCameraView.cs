using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util.Jar;
using OpenCV.Android;
using static Android.Hardware.Camera;

namespace OpenCVApp.CustomControls
{
    public class CustomCameraView : JavaCameraView, IPictureCallback
    { 
        public event Action<byte[]> PictureTaken;

        public CustomCameraView(Context context, int attr) : base(context, attr)
        {

        }

        public CustomCameraView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public void TakePicture( )
        { 
            // Postview and jpeg are sent in the same buffers if the queue is not empty when performing a capture.
            // Clear up buffers to avoid MCamera.takePicture to be stuck because of a memory issue
            //MCamera.SetPreviewCallback(null);

            // PictureCallback is implemented by the current class
            MCamera.TakePicture(null, null, this);
        }

        public void OnPictureTaken(byte[] data, Camera camera)
        {
            PictureTaken.Invoke(data);
        }
    }
}