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
        readonly AtlasConfig config;

        public MetricPublishTask(AtlasConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            this.config = config;
        }

        public DateTime ScheduledStart { get; set; }

        public void Start()
        {
            try
            {
                var publisher = new AtlasMetricPublisher(config);
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
                var timestamp = DateTime.UtcNow.GetCurrentUnixTimestampMillis();
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

    public static class DateTimeExtensions
    {
        static readonly DateTime unixStartDate = new DateTime(1970, 1, 1, 0, 0, 0);

        /// <summary>
        /// Converts a <see cref="DateTime"/> object into a unix timestamp number.
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>An intger for the number of seconds since 1st January 1970, as per unix specification.</returns>
        public static int ToUnixTimestamp(this DateTime date)
        {
            TimeSpan ts = date - unixStartDate;
            return (int)ts.TotalSeconds;
        }

        /// <summary>
        /// Converts a string, representing a unix timestamp number into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="timestamp">The timestamp, as a string.</param>
        /// <returns>The <see cref="DateTime"/> object the time represents.</returns>
        public static DateTime UnixTimestampToDate(string timestamp)
        {
            if (timestamp == null || timestamp.Length == 0)
                return DateTime.MinValue;

            return UnixTimestampToDate(Int32.Parse(timestamp));
        }

        /// <summary>
        /// Converts a <see cref="long"/>, representing a unix timestamp number into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="timestamp">The unix timestamp.</param>
        /// <returns>The <see cref="DateTime"/> object the time represents.</returns>
        public static DateTime UnixTimestampToDate(int timestamp)
        {
            return unixStartDate.AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts a <see cref="double"/>, representing a unix timestamp + milliseconds number into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="timestamp">The unix timestamp.</param>
        /// <returns>The <see cref="DateTime"/> object the time represents.</returns>
        public static DateTime UnixTimestampToDate(double timestamp)
        {
            int unixSeconds = (int)Math.Floor(timestamp);
            var current = unixStartDate.AddSeconds(timestamp);
            return current.AddMilliseconds(timestamp - unixSeconds);
        }

        public static long GetCurrentUnixTimestampMillis(this DateTime date)
        {
            return (long)(date - unixStartDate).TotalMilliseconds;
        }
    }
}
