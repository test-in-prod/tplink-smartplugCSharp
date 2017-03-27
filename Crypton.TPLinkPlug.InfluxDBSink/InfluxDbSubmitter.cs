using Crypton.TPLinkPlug.InfluxDBSink.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.InfluxDBSink
{
    class InfluxDbSubmitter : IDisposable
    {

        private readonly InfluxDbElement config = null;
        private ConcurrentQueue<Metric> queue = new ConcurrentQueue<Metric>();
        private Timer submitTimer = null;

        public InfluxDbSubmitter(InfluxDbElement config)
        {
            this.config = config;
            this.submitTimer = new Timer(submitAsync, null, 0, 5000);
        }

        private void submitAsync(object state)
        {
            try
            {
                if (!queue.IsEmpty)
                {
                    using (var sw = new StringWriter())
                    {
                        sw.NewLine = "\n";
                        int count = 0;
                        do
                        {
                            Metric metric;
                            if (queue.TryDequeue(out metric))
                            {
                                sw.WriteLine(metric.ToString());
                                Console.WriteLine(metric.ToString());
                            }
                            else
                            {
                                break;
                            }
                        } while (++count < 2500);

                        using (var httpClient = new HttpClient())
                        {
                            httpClient.Timeout = TimeSpan.FromSeconds(30);
                            using (var message = new HttpRequestMessage())
                            {
                                message.RequestUri = new Uri(config.WriteUrl);
                                message.Method = HttpMethod.Post;
                                message.Content = new StringContent(sw.ToString());

                                var result = httpClient.SendAsync(message).Result;
                                Console.WriteLine($"[{DateTime.Now}] HTTP POST {config.WriteUrl} - {result.StatusCode} - {result.ReasonPhrase} - {result.Content.ReadAsStringAsync().Result}");
                            }
                        }

                    }
                }
            }
            catch { }
        }

        public void Add(Metric metric)
        {
            queue.Enqueue(metric);
        }

        public void Dispose()
        {
            queue = null;
            submitTimer.Dispose();
            submitTimer = null;
        }
    }
}
