using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AsyncUrlStatus
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await SomebL();
        }

        private static async Task SomebL()
        {
            Stopwatch st1 = Stopwatch.StartNew();
            var range = Enumerable.Range(0, 10000);
            List<string> Urls = range.Select(x => "/" + x.ToString()).ToList();
            //List<string> Urls = new List<string> { "/c" };
            List<Task> tasks = new List<Task>();

            HttpClient cl1 = new HttpClient { BaseAddress = new Uri("https://localhost:7227") };
            HttpClient cl2 = new HttpClient { BaseAddress = new Uri("https://localhost:7227") };
            HttpClient cl3 = new HttpClient { BaseAddress = new Uri("https://localhost:7227") };
            HttpClient cl4 = new HttpClient { BaseAddress = new Uri("https://localhost:7227") };
            int howManyTasks = 0;
            int i = 0;
            foreach (var url in Urls)
            {
                HttpClient cl = null;
                switch (i % 4)
                {
                    case 0:
                        cl = cl1;
                        howManyTasks++;
                        break;
                    case 1:
                        cl = cl2;
                        howManyTasks++;
                        break;
                    case 2:
                        cl = cl3;
                        howManyTasks++;
                        break;
                    case 3:
                        cl = cl4;
                        howManyTasks++;
                        break;
                }
                i++;
                tasks.Add(TaskUrlGet(cl, url, i));
            }



            try
            {
                await Task.WhenAll(tasks);
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    // Handle or log each exception
                    Console.WriteLine(innerException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main Exception: " + ex.ToString());
            }

            st1.Stop();
            Console.WriteLine(st1.ElapsedMilliseconds / 1000);
        }

        private static Task TaskUrlGet(HttpClient cl, string url, int i)
        {
            return Task.Run(async () =>
            {
                //var rsp = await cl.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                var rsp = await cl.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                //Console.WriteLine((int)rsp.StatusCode);

            });
        }

        private static async Task<HttpResponseMessage> xzAsync(HttpClient cl, string url)
        {
            var rsp = await cl.GetAsync(url);
            return rsp;
        }

    }
}
