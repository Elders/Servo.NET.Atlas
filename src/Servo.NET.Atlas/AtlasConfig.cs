using System;

namespace Servo.NET.Atlas
{
    public class AtlasConfig
    {
        public AtlasConfig(string hostname, int port = 7101, string endpoint = "api/v1/publish")
        {
            Hostname = new Uri(hostname + ":" + port);
            Endpoint = endpoint;
        }

        public Uri Hostname { get; private set; }

        public string Endpoint { get; private set; }
    }
}
