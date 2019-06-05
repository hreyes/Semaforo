using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Semaforo
{
    class Program
    {
        const int MAXHILOS = 5;
        const int N = MAXHILOS;
        enum Estado
        {
            THINKING = 0,
            HUNGRY = 1,
            EATING = 2
        }

        static Semaphore mutex;
        static Semaphore[] semaforos = new Semaphore[N];
        static Estado[] estados = new Estado[N];
        static Thread[] hilo = new Thread[MAXHILOS];


        static void Main(string[] args)
        {
            int i;
            mutex = new Semaphore(1, 2);
            for (i = 0; i < N; i++)
                semaforos[i] = new Semaphore(0, 1);

            for (i = 0; i < MAXHILOS; i++)
            {
                hilo[i] = new Thread(delegate () { philosopher(i); });
                hilo[i].Start();
            }

            for (i = 0; i < MAXHILOS; i++)
                hilo[i].Join();
        }

        private static void philosopher(int i)
        {
            while (true)
            {
                thinking(i);
                take_fork(i);
                eat(i);
                puts_forks(i);
            }
        }
        private static void eat(int v)
        {
            Console.WriteLine($"Filosofo {v} comiendo");
        }
        private static void thinking(int v)
        {
            Console.WriteLine($"Filosofo {v} pensando");
        }
        private static void take_fork(int v)
        {
            mutex.WaitOne();
            estados[v] = Estado.HUNGRY;
            test(v);
            Thread.Sleep(300);
            mutex.Release();
            semaforos[v].WaitOne();
        }
        private static void puts_forks(int v)
        {
            mutex.WaitOne();
            estados[v] = Estado.THINKING;
            thinking(v);
            Thread.Sleep(300);

            //if (new Random().Next(0, 2) == 1)
                test((v + N - 1) % N);
            //else
                test((v + 1) % N);

            mutex.Release();
        }
        private static void test(int v)
        {
            if((estados[v]== Estado.HUNGRY)&& (estados[(v+N-1)%N]!= Estado.EATING) && (estados[(v+1)%N] != Estado.EATING))
            {
                estados[v] = Estado.EATING;
                eat(v);
                semaforos[v].Release();
            }


        }


    }
}
