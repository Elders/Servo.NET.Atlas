using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Servo.NET;
using Elders.Servo.NET.Attributes;
using Elders.Servo.NET.Tag;

namespace Elders.Servo.NET.Atlas
{
    public class AtlasMetrics
    {
        private static ITag ATLAS_COUNTER_TAG = new BasicTag("atlas.dstype", "counter");
        private static ITag ATLAS_GAUGE_TAG = new BasicTag("atlas.dstype", "gauge");

        public AtlasMetrics(IEnumerable<Metric> metrics)
        {
            this.tags = new Dictionary<string, string>();
            this.metrics = BuildAtlasMetric(metrics);
        }

        public AtlasMetrics(ITagList commonTags, IEnumerable<Metric> metrics)
        {
            this.tags = commonTags.AsDictionary();

            this.metrics = BuildAtlasMetric(metrics);
        }

        private List<AtlasMetric> BuildAtlasMetric(IEnumerable<Metric> theMetrics)
        {
            return theMetrics.Select(x => BuildAtlasMetric(x)).ToList();
        }

        private AtlasMetric BuildAtlasMetric(Metric metric)
        {
            var local = ValidCharacters.toValidValue(metric);

            if (isCounter(local))
                local = asCounter(local);
            else if (isGauge(local))
                local = asGauge(local);

            BasicTagList tags = local.getConfig().getTags() as BasicTagList;
            var asd = SmallTagMap.builder()
                .add(new NameTag(local.getConfig().getName()));

            var atlasTags = new BasicTagList(asd.result());
            var tagsWithName = tags.copy(atlasTags);
            return new AtlasMetric(local.getValue(), local.getTimestamp(), tagsWithName);
        }

        protected List<Metric> identifyDsTypes(List<Metric> metrics)
        {
            // since we never generate atlas.dstype = counter we can do the following:

            return metrics.Select(m => isRate(m) ? m : asGauge(m)).ToList();
        }

        protected static Metric asCounter(Metric m)
        {
            return new Metric(m.getConfig().withAdditionalTag(ATLAS_COUNTER_TAG), m.getTimestamp(), m.getValue());
        }

        protected static Metric asGauge(Metric m)
        {
            return new Metric(m.getConfig().withAdditionalTag(ATLAS_GAUGE_TAG), m.getTimestamp(), m.getValue());
        }

        protected static bool isCounter(Metric m)
        {
            ITagList tags = m.getConfig().getTags();
            string value = tags.getValue(DataSourceType.KEY);
            return value != null && value.Equals(DataSourceType.COUNTER.name);
        }

        protected bool isGauge(Metric m)
        {
            ITagList tags = m.getConfig().getTags();
            string value = tags.getValue(DataSourceType.KEY);
            return value != null && value.Equals(DataSourceType.GAUGE.name);
        }

        protected bool isRate(Metric m)
        {
            ITagList tags = m.getConfig().getTags();
            string value = tags.getValue(DataSourceType.KEY);
            return DataSourceType.RATE.name.Equals(value) || DataSourceType.NORMALIZED.name.Equals(value);
        }

        public IDictionary<string, string> tags { get; set; }

        public List<AtlasMetric> metrics { get; set; }
    }

    public class AtlasMetric
    {
        public AtlasMetric() { }

        public AtlasMetric(object value, long timestamp, IEnumerable<ITag> tags)
        {
            if (value is bool)
                this.value = Convert.ToInt32(value);
            else
                this.value = value;
            this.timestamp = timestamp;
            this.tags = tags.ToDictionary(key => key.Key, val => val.Value);
        }

        public long timestamp { get; set; }

        public object value { get; set; }

        public IDictionary<string, string> tags { get; set; }
    }
}
