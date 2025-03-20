using System.Diagnostics;
using System.Text;
using System.Collections.Concurrent;

class Program
{
    static void Main(string[] args)
    {
        string filePath = Path.Join(Directory.GetCurrentDirectory(), "names.csv");
        if (!File.Exists(filePath)) return;

        Process process = Process.GetCurrentProcess();
        StringBuilder sb = new();
        TimeSpan cpuTime;
        Stopwatch stopwatch = new();
        ConcurrentDictionary<string, int> collections = new();
        bool isRunning = false;
        int isDuplicate = 0;
        List<string> param = new() { "--r=", "--d=" };
        List<string> val = new() { "true", "1" };
        try
        {
            #region Setup params
            foreach (var arg in args)
            {
                if (arg.StartsWith(param[0]))
                {
                    string value = arg.Substring(param[0].Length).ToLower();
                    isRunning = val.Contains(value);
                }

                if (arg.StartsWith(param[1]))
                {
                    string value = arg.Substring(param[1].Length).ToLower(); ;
                    isDuplicate = val.Contains(value) ? 1 : 0;
                }
            }
            #endregion

            sb.AppendLine($"Start time: {DateTime.Now.ToFileTime()}");
            stopwatch = Stopwatch.StartNew();
            cpuTime = process.TotalProcessorTime;

            File.ReadAllLines(filePath, Encoding.UTF8).AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .ForAll(l =>
                {
                    if (string.IsNullOrEmpty(l) || l.Contains("State")) return;

                    string name = l.Split(",")[3];
                    collections.AddOrUpdate(name, 1, (key, counter) => counter + 1);
                });

            var victims = collections.Where(c => c.Value > isDuplicate).OrderByDescending(x => x.Value);
            var more = victims.Take(10).ToArray();
            var less = victims.TakeLast(10).ToArray();

            if (isRunning) {
                less = victims.GroupBy(v => v.Value)
                                        .OrderBy(g => g.Key)
                                        .SelectMany(g => g.Take(1))
                                        .Take(10)
                                        .ToArray();
            }

            sb.AppendLine($"Total unique name: {victims.Count()}");
            
            sb.AppendLine($"Most duplicated name: ");
            for (int i = 0; i < more.Length; i++) {
                sb.AppendLine($"{i+1}. {more[i].Key} = {more[i].Value}");
            }
            
            

            sb.AppendLine($"Least duplicated name:");
            for (int i = 0; i < less.Length; i++)
            {
                sb.AppendLine($"{i + 1}. {less[i].Key} = {less[i].Value}");
            }

            stopwatch.Stop();

            sb.AppendLine($"Finished time: {DateTime.Now.ToFileTime()}");
            sb.AppendLine($"Elapsed time: {stopwatch.ElapsedMilliseconds / 1000.00}s ({stopwatch.ElapsedMilliseconds}ms)");
            sb.AppendLine($"CPU time: {(process.TotalProcessorTime - cpuTime).TotalMilliseconds}");
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            Console.WriteLine(sb.ToString());
#if DEBUG
            Console.WriteLine("Running on debug mode !");
#else
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

#endif
        }
    }
}
