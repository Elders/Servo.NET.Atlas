using RestSharp;
using Servo.NET.Atlas.Logging;

namespace Servo.NET.Atlas
{
    public class AtlasMetricPublisher
    {
        static ILog log = LogProvider.GetLogger(typeof(AtlasMetricPublisher));

        readonly RestClient client;
        string resourceLocation;

        public AtlasMetricPublisher(AtlasConfig config)
        {
            client = new RestClient(config.Hostname);
            client.AddDefaultHeader("Content-Type", "application'/json");
            resourceLocation = config.Endpoint;
        }

        public void Publish(AtlasMetrics metrics)
        {
            var request = new RestRequest(resourceLocation, Method.POST);
            request.AddJsonBody(metrics);
            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                log.ErrorException("Failed to push metrics. Details: " + response.Content, response.ErrorException);
        }
    }
}
