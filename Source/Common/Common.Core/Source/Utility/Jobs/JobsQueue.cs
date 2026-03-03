// using System.Collections.Concurrent;

namespace VoxelEngine.Core;

public class JobsQueue : Singleton<JobsQueue>
{
    // Track running jobs if we need to wait for them
    // private ConcurrentBag<Task> _activeJobs = new();

    private readonly List<Action> _tasks = new();

    /// <summary>
    /// Fire and Forget: Run this logic on a background thread.
    /// Good for: Pathfinding, Chunk Generation, Save to Disk.
    /// </summary>
    public void ScheduleJob(Action job)
    {
        // Task.Run uses the .NET ThreadPool automatically
        Task.Run(() =>
        {
#if DEBUG // Log errors in debug mode
            try
            {
                job();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Job Error: {ex}");
            }
#else
            job();
#endif
        });
    }

    /// <summary>
    /// Parallel Processor: Run logic on a list of items using all cores.
    /// Good for: Updating 500 Physics Worlds at once.
    /// </summary>
    public void ProcessParallel<T>(IEnumerable<T> items, Action<T> processor)
    {
        // This blocks the calling thread until ALL items are processed.
        // It creates extremely low overhead tasks.
        Parallel.ForEach(items, processor);
    }

    public void Process<T>(IEnumerable<T> items, Action<T> processor)
    {
        foreach (var item in items)
        {
            processor(item);
        }
    }

    /// <summary>
    /// Async Data Fetch: Get data without blocking.
    /// Good for: Loading Assets, Web Requests.
    /// </summary>
    public async Task<T> ScheduleDataJob<T>(Func<T> heavyCalculation)
    {
        return await Task.Run(heavyCalculation);
    }
}