using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Multithreading.Scheduler;
using Netflix.Servo;
using Netflix.Servo.Monitor;

namespace Servo.NET.Atlas
{
    public class MetricPublishTask : IWork
    {
        public DateTime ScheduledStart { get; set; }

        public void Start()
        {
            try
            {
                var publisher = new AtlasMetricPublisher(new Uri("http://192.168.69.91:7101"));
                var mon = DefaultMonitorRegistry.getInstance();
                var monitors = mon.getRegisteredMonitors();

                var monitorsFlatten = new List<IMonitor>();
                foreach (var monitor in monitors)
                {
                    var compositeMonitor = monitor as ICompositeMonitor;
                    if (ReferenceEquals(null, compositeMonitor) == false)
                        monitorsFlatten.AddRange(compositeMonitor.getMonitors());
                    else
                        monitorsFlatten.Add(monitor);
                }
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var metrics = monitorsFlatten.Select(x => new Metric(x.getConfig(), timestamp, x.GetValue()));

                AtlasMetrics aa = new AtlasMetrics(metrics);
                publisher.Publish(aa);
            }
            catch (Exception ex)
            {
                //log
            }
            finally
            {
                ScheduledStart = DateTime.UtcNow.AddSeconds(10);
            }
        }

        public void Stop()
        {
            //  Do cleanup here.
        }
    }
}