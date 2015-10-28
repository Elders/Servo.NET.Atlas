using Elders.Multithreading.Scheduler;

namespace Servo.NET.Atlas
{
    public class CommandMetricObserver
    {
        readonly WorkPool pool;

        public CommandMetricObserver()
        {
            string poolName = "This name is assigned to the current Thread.Name";
            int numberOfThreadsAvailableForThePool = 1;

            pool = new WorkPool(poolName, numberOfThreadsAvailableForThePool);
            for (int i = 0; i < numberOfThreadsAvailableForThePool; i++)
            {
                pool.AddWork(new MetricPublishTask());
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
