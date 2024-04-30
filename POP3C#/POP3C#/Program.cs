using System;
using System.Collections.Generic;
using System.Threading;

namespace ProducerConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            int storageSize = 3;
            int itemNumbers = 10;
            int consumerCount = 2;
            int producerCount = 2;
            program.Starter(storageSize, itemNumbers, consumerCount, producerCount);
            Console.ReadKey();
        }

        private void Starter(int storageSize, int itemNumbers, int consumerCount, int producerCount)
        {
            Access = new Semaphore(1, 1);
            Full = new Semaphore(storageSize, storageSize);
            Empty = new Semaphore(0, storageSize);

            for (int i = 0; i < consumerCount; i++)
            {
                Thread threadConsumer = new Thread(() => Consumer(itemNumbers));
                threadConsumer.Start();
            }

            for (int i = 0; i < producerCount; i++)
            {
                Thread threadProducer = new Thread(() => Producer(itemNumbers));
                threadProducer.Start();
            }
        }

        private Semaphore Access;
        private Semaphore Full;
        private Semaphore Empty;

        private readonly List<string> storage = new List<string>();

        private void Producer(int itemNumbers)
        {
            for (int i = 0; i < itemNumbers; i++)
            {
                Full.WaitOne();
                Access.WaitOne();

                lock (storage)
                {
                    storage.Add("item " + i);
                    Console.WriteLine($"Producer added item {i}");
                }

                Access.Release();
                Empty.Release();
            }
        }

        private void Consumer(int itemNumbers)
        {
            for (int i = 0; i < itemNumbers; i++)
            {
                Empty.WaitOne();
                Thread.Sleep(1000);
                Access.WaitOne();

                string item;
                lock (storage)
                {
                    item = storage[0];
                    storage.RemoveAt(0);
                }

                Full.Release();

                Access.Release();

                Console.WriteLine($"Consumer took {item}");
            }
        }
    }
}
