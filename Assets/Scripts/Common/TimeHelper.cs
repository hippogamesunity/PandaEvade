using System;
using System.Net;
using System.Net.Sockets;

namespace Assets.Scripts.Common
{
    public static class TimeHelper
	{
        public static long TimeSecondsUtc
        {
            get { return ToSeconds(DateTime.UtcNow); }
        }

        public static DateTime NetworkTimeUtc
        {
            get
            {
                var servers = new[] { "pool.ntp.org", "time1.google.com" };
                
                foreach (var server in servers)
                {
                    try
                    {
                        var ntpData = new byte[48];
                        ntpData[0] = 0x1B; // LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

                        var addresses = Dns.GetHostEntry(server).AddressList;
                        var ipEndPoint = new IPEndPoint(addresses[0], 123);
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                        socket.SendTimeout = socket.ReceiveTimeout = 8000;
                        socket.Connect(ipEndPoint);
                        socket.Send(ntpData);
                        socket.Receive(ntpData);
                        socket.Close();

                        var intPart = (ulong) ntpData[40] << 24 | (ulong) ntpData[41] << 16 | (ulong) ntpData[42] << 8 | ntpData[43];
                        var fractPart = (ulong) ntpData[44] << 24 | (ulong) ntpData[45] << 16 | (ulong) ntpData[46] << 8 | ntpData[47];
                        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                        var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long) milliseconds);

                        return networkDateTime;
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log(e);
                    }
                }

                throw new Exception("Connection unavilable");
            }
        }

        public static DateTime FromSeconds(long seconds)
        {
            return new DateTime(seconds * TimeSpan.TicksPerSecond).ToUniversalTime();
        }

        public static long ToSeconds(DateTime time)
        {
            return time.Ticks / TimeSpan.TicksPerSecond;
        }
	}
}