using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Lounge.API
{
    public static class Kafka
    {
        public async static void Push(string message)
        {
            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = Helpers.GetConfig("Kafa:Connection")
                };

                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    var data = await producer.ProduceAsync(Helpers.GetConfig("Kafa:Topic"), new Message<Null, string> { Value = message });
                }

            }
            catch (Exception ex)
            {
                
            }

        }
    }
}
