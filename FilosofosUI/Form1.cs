using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilosofosUI
{
    public enum Estado
    {
        THINKING = 0,
        HUNGRY = 1,
        EATING = 2
    }
    public class Filosofo
    {
        public PictureBox Picture { get; set; }
        public Estado EstadoF { get; set; }
    }
    public partial class Form1 : Form
    {
        const int MAXHILOS = 5;
        const int N = MAXHILOS;
        static Semaphore mutex;
        static List<Semaphore> semaforos = new List<Semaphore>();
        static List<Thread> hilo = new List<Thread>();
        static List<Filosofo> filosofos = new List<Filosofo>();

        public Form1()
        {
            InitializeComponent();

            Filosofo f1 = new Filosofo();
            f1.Picture = F1;
            f1.EstadoF = Estado.THINKING;
            filosofos.Add(f1);

            Filosofo f2 = new Filosofo();
            f2.Picture = F2;
            f2.EstadoF = Estado.THINKING;
            filosofos.Add(f2);

            Filosofo f3 = new Filosofo();
            f3.Picture = F3;
            f3.EstadoF = Estado.THINKING;
            filosofos.Add(f3);

            Filosofo f4 = new Filosofo();
            f4.Picture = F4;
            f4.EstadoF = Estado.THINKING;
            filosofos.Add(f4);

            Filosofo f5 = new Filosofo();
            f5.Picture = F5;
            f5.EstadoF = Estado.THINKING;
            filosofos.Add(f5);



        }

        private void philosopher(int i)
        {
            while (true)
            {
                if (i <= 4)
                {
                    thinking(i);
                    take_fork(i);
                    eat(i);
                    puts_forks(i);
                }
            }
        }

        private void eat(int v)
        {
            Console.WriteLine($"Filosofo {v} comiendo");
        }
        private void thinking(int v)
        {
            Console.WriteLine($"Filosofo {v} pensando");
        }
        private void take_fork(int v)
        {
            mutex.WaitOne();
            filosofos[v].EstadoF = Estado.HUNGRY;
            filosofos[v].Picture.Image = Properties.Resources.fork_and_knife;
            test(v);
            Thread.Sleep(200);
            mutex.Release();
            semaforos[v].WaitOne();
        }
        private void puts_forks(int v)
        {
            mutex.WaitOne();
            filosofos[v].EstadoF = Estado.THINKING;
            filosofos[v].Picture.Image = Properties.Resources.thinking;
            thinking(v);
            Thread.Sleep(300);

            test((v + N - 1) % N);
            test((v + 1) % N);
            mutex.Release();
        }
        private void test(int v)
        {
            if ((filosofos[v].EstadoF == Estado.HUNGRY) && (filosofos[(v + N - 1) % N].EstadoF != Estado.EATING) && (filosofos[(v + 1) % N].EstadoF != Estado.EATING))
            {
                filosofos[v].EstadoF = Estado.EATING;
                filosofos[v].Picture.Image = Properties.Resources.eating;
                eat(v);
                semaforos[v].Release();
            }
        }
      

        private void Form1_Load(object sender, EventArgs e)
        {
            int i;
            mutex = new Semaphore(1, 2);
            for (i = 0; i < N; i++)
                semaforos.Add(new Semaphore(0, 1));

            for (i = 0; i < MAXHILOS; i++)
            {
                hilo.Add(new Thread(delegate () { philosopher(i); }));
                hilo[i].Start();
            }

            for (i = 0; i < MAXHILOS; i++)
                hilo[i].Join();
        }
    }
}
