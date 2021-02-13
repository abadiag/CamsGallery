using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TrafficCams.Services
{
    public class UpdateService
    {
        public delegate void OnRequestUpdate();
        public event OnRequestUpdate RequestCallback;

        private static Timer _timer;

        public UpdateService()
        {
            _timer = new Timer(10000);
            _timer.Elapsed += OnTick;
            _timer.Start();
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            RequestCallback?.Invoke();
        }

        public static void ChangeConfigAsync(double ms)
        {
            if (_timer is null) return;

            _timer.Stop();
            _timer.Interval = ms;
            _timer.Start();
        } 
    }
}
