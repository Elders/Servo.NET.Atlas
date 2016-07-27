using Elders.Multithreading.Scheduler;

namespace Elders.Servo.NET.Atlas
{
    public class CommandMetricObserver
    {
        readonly WorkPool pool;

        public CommandMetricObserver(AtlasConfig config)
        {
            string poolName = "atlas-" + config.Endpoint;
            int numberOfThreadsAvailableForThePool = 1;

            pool = new WorkPool(poolName, numberOfThreadsAvailableForThePool);
            for (int i = 0; i < numberOfThreadsAvailableForThePool; i++)
            {
                pool.AddWork(new MetricPublishTask(config));
            }
            pool.StartCrawlers();
        }

        public void Run()
        {
            pool.StartCrawlers();
        }

        public void Stop()
        {
            pool.Stop();
        }
    }
}
