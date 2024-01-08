using System;
using System.Collections.Concurrent;
using Demo.Service;

namespace Demo.Test
{
    public class TestMain
    {
        private const string BaseUrl = "http://localhost:8080";
        private HttpClient httpClient;

        public TestMain(HttpClient _HttpClient)
        {
            httpClient = _HttpClient;

        }

        public async Task Main()
        {
            // 启动服务
            var leaderboardService = new LeaderboardService();

            // 执行多线程测试
            await RunConcurrentTest();


        }

        public async Task RunConcurrentTest()
        {
            const int numberOfThreads = 100;
            const int requestsPerThread = 1000;
            var tasks = new ConcurrentBag<Task>();

            for (int i = 0; i < numberOfThreads; i++)
            {
                var threadNumber = i;
                var task = Task.Run(async () =>
                {
                    var random = new Random(threadNumber);
                

                    for (int j = 0; j < requestsPerThread; j++)
                    {
                        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                        var customerId = random.Next(1, 1000); // 随机生成用户ID
                        var scoreChange = random.Next(-10, 11); // 随机生成积分变化

                        var requestUri = $"{BaseUrl}/customer/{customerId}/score/{scoreChange}";
                        var response = await httpClient.PostAsync(requestUri, null);

                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error in thread {threadNumber}: {response.StatusCode}");
                        }
                        stopwatch.Stop();
                        Console.WriteLine($"Thread {threadNumber} completed in {stopwatch.ElapsedMilliseconds} ms");
                    }

               
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

    }
}

