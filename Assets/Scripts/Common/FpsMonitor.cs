using UnityEngine;

namespace Assets.Scripts.Common
{
    public class FpsMonitor : Script
    {
        public float UpdateInterval = 0.5f;
        public static float Fps;
        public UILabel Text;
        
        private float _accum;
        private int _frames;
        private float _timeleft;

        public void Start()
        {
            _timeleft = UpdateInterval;
        }

        public void Update()
        {
            _timeleft -= Time.deltaTime;
            _accum += Time.timeScale / Time.deltaTime;
            ++_frames;

            if (_timeleft <= 0.0)
            {
                var fps = _accum / _frames;
                var format = string.Format("{0:F2} FPS ", fps);

                Text.text = format;

                if (fps < 10)
                {
                    Text.color = Color.red;
                }
                else if (fps < 30)
                {
                    Text.color = Color.yellow;
                }
                else
                {
                    Text.color = Color.green;
                }

                _timeleft = UpdateInterval;
                _accum = 0.0F;
                _frames = 0;

                Fps = fps;
            }
        }
    }
}