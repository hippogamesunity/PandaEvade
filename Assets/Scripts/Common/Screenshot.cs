using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class Screenshot : Script
    {
        public int SuperSize = 1;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                var filename = Md5.Encode(Convert.ToString(DateTime.Now)) + ".png";

                Application.CaptureScreenshot(filename, SuperSize);
                Debug.Log(filename);
            }
        }
    }
}