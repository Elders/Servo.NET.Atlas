using System;
using RestSharp;

namespace Servo.NET.Atlas
{
    public class AtlasMetricPublisher
    {
        readonly RestClient client;
        string resourceLocation = "api/v1/publish";

        public AtlasMetricPublisher(Uri publishUri)
        {
            client = new RestClient(publishUri);
            client.AddDefaultHeader("Content-Type", "application'/json");
        }

        public void Publish(AtlasMetrics metrics)
        {
            var request = new RestRequest(resourceLocation, Method.POST);
            request.AddJsonBody(metrics);
            var respo = client.Execute(request);
            if (respo.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(respo.ErrorMessage, respo.ErrorException);
            }
        }
    }
}
