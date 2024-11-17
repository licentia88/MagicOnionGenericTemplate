using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace MagicT.Shared.MemoryProfiler;

public class MemoryMonitor
{
    private const long MemoryThreshold = 500 * 1024 * 1024; // 500 MB

    public static async Task MonitorMemoryUsageAsync()
    {
        while (true)
        {
            long totalMemory = GC.GetTotalMemory(false);
            
            // int gen0CountBefore = GC.CollectionCount(0);
            // int gen1CountBefore = GC.CollectionCount(1);
            // int gen2CountBefore = GC.CollectionCount(2);
            
            Console.WriteLine($"Total Memory: {totalMemory / (1024 * 1024)} MB");

            if (totalMemory > MemoryThreshold)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Console.WriteLine("Memory usage is high. Garbage Collector triggered.");
            }
            
            // Console.WriteLine("GC Generation 0 Count: " + (GC.CollectionCount(0) - gen0CountBefore));
            // Console.WriteLine("GC Generation 1 Count: " + (GC.CollectionCount(1) - gen1CountBefore));
            // Console.WriteLine("GC Generation 2 Count: " + (GC.CollectionCount(2) - gen2CountBefore));


            await Task.Delay(5000); // Check memory usage every 5 seconds
        }
    }

  
}