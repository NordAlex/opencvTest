using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenCV.Android; 
 
namespace OpenCVApp.Callbacks
{
    public class OpenCVLoaderCalback : BaseLoaderCallback
    {
        private CameraBridgeViewBase _mOpenCvCameraView;

        public OpenCVLoaderCalback(CameraBridgeViewBase mOpenCvCameraView, Context context) : base(context)
        {
            _mOpenCvCameraView = mOpenCvCameraView;
        }

        public override void OnManagerConnected(int status)
        {
            switch (status)
            {
                case LoaderCallbackInterface.Success:
                    {
                        _mOpenCvCameraView.EnableView();
                    }
                    break;
                default:
                    {
                        base.OnManagerConnected(status);
                    }
                    break;
            }
        }
    }
}