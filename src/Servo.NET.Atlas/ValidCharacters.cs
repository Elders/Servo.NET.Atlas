using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Netflix.Servo;
using Netflix.Servo.Monitor;
using Netflix.Servo.Tag;

namespace Servo.NET.Atlas
{
    /// <summary>
    /// Utility class to deal with rewriting keys/values to the character set accepted by atlas.
    /// </summary>
    public class ValidCharacters
    {
        /// <summary>
        /// Only allow letters, numbers, underscores, dashes and dots in our identifiers.
        /// </summary>
        private static Regex INVALID_CHARS = new Regex("[^a-zA-Z0-9_\\-\\.]");

        private ValidCharacters()
        {
            // utility class
        }

        /// <summary>
        /// Convert a given string to one where all characters are valid.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String toValidCharset(String str)
        {
            return INVALID_CHARS.Replace(str, "_");
        }

        /// <summary>
        /// Return a new metric where the name and all tags are using the valid character set.
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        public static Metric toValidValue(Metric metric)
        {
            MonitorConfig cfg = metric.getConfig();
            MonitorConfig.Builder cfgBuilder = MonitorConfig.builder(toValidCharset(cfg.getName()));
            foreach (ITag orig in cfg.getTags())
            {
                cfgBuilder.withTag(toValidCharset(orig.Key), toValidCharset(orig.Value));
            }
            cfgBuilder.withPublishingPolicy(cfg.getPublishingPolicy());
            return new Metric(cfgBuilder.build(), metric.getTimestamp(), metric.getValue());
        }

        /// <summary>
        /// Create a new list of metrics where all metrics are using the valid character set.
        /// </summary>
        /// <param name="metrics"></param>
        /// <returns></returns>
        public static List<Metric> toValidValues(List<Metric> metrics)
        {
            List<Metric> fixedMetrics = new List<Metric>(metrics.Count);
            foreach (var m in metrics)
            {
                fixedMetrics.Add(toValidValue(m));
            }
            return fixedMetrics;
        }
    }
}
