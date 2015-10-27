using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1Serialization
{
    class Program
    {
        private readonly object _lock = new object();
        private readonly Queue<byte[]> _queue = new Queue<byte[]>();

        IPayloadSerializer _serializer;

        private void ProducerLoop()
        {
            while (true)
            {
                var dto = PayloadDto.Create();
                lock (_lock)
                {
                    var serialized = _serializer.Serialize(dto);
           
                    Console.WriteLine("Original data:");
                    Console.WriteLine("{0}", dto);

                    _queue.Enqueue(serialized);
                    Monitor.Pulse(_lock);
                }
                Thread.Sleep(5000);
            }
        }

        private void ConsumerLoop()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (_queue.Count == 0)
                        Monitor.Wait(_lock);

                    var serialized = _queue.Dequeue();
                    var dto = _serializer.Deserialize(serialized);
                
                    Console.WriteLine("Deserialized data:");
                    Console.WriteLine("{0}", dto);

                }
                Thread.Sleep(5000);
            }
        }

        public void Run()
        {
            _serializer = new BinaryFormatterPayloadSerializer();

            Thread consumerThread = new Thread(ConsumerLoop);
            Thread producerThread = new Thread(ProducerLoop);

            consumerThread.IsBackground = true;
            producerThread.IsBackground = true;

            consumerThread.Start();
            producerThread.Start();

            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}
