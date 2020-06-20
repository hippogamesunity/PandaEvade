using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    [ExecuteInEditMode]
    public class Screenshot : Script
    {
        public int SuperSize = 1;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Take();
            }
        }

        public void Take()
        {
            var filename = Md5.Encode(Convert.ToString(DateTime.Now)) + ".png";

            ScreenCapture.CaptureScreenshot(filename, SuperSize);
            Debug.Log(filename);
        }
    }
}