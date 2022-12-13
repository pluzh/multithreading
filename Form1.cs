using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Лабораторная1_семестр2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static Semaphore smf1 = new Semaphore(0, 1); //создание семафоров; данный смафор для синхронизации вывода в основном потоке
        public static Semaphore smf2 = new Semaphore(0, 1);// данный смафор для синхронизации вывода в основном потоке
        public static Semaphore smf = new Semaphore(0, 2); //данный семафор для одновременного запуска двух сортировок

        static object krit = new object(); //создаем объект для критической секции
    
        static void Puzirok() //с критичской секцией
        {
            smf.Release(); //высвобождает семафор
            smf.WaitOne();  // ждет, когда в семаформе освободится место и начинает выполнять все дальнейшие действия
            
            lock (krit) //собственно критическая секция
            {
                for (int i = 0; i < A1.Length; i++)
                    for (int j = i + 1; j < A1.Length; j++)
                        if (A1[i] > A1[j])
                        {
                            int temp = A1[i];
                            A1[i] = A1[j];
                            Thread.Sleep(100);
                            A1[j] = temp;
                        }
            }
            
            smf1.Release(); //высвобождает (открывает) семафор для основного потока
        }
        static void Puzirok2() // без крит секции
        {
            smf.WaitOne();// ждет, когда в семаформе освободится место и начинает выполнять все дальнейшие действия
            smf.Release();//высвобождает семафор
            for (int i = 0; i < A1.Length; i++)
                for (int j = i + 1; j < A1.Length; j++)
                    if (A1[i] > A1[j])
                    {
                            int temp = A1[i];
                            A1[i] = A1[j];
                            Thread.Sleep(100);
                            A1[j] = temp;
                    }

            smf1.Release();
        }
        static void Maksimum()
            {
                smf.Release(); 
                smf.WaitOne();
                lock (krit)
                {
                    for (int i = A1.Length - 1; i >= 1; i--)
                    {
                        int max = i;
                        for (int j = i - 1; j >= 0; j--)
                        {

                            if (A1[max] < A1[j])
                            {
                                max = j;
                            }
                        }

                        int temp = A1[i];
                        Thread.Sleep(100);
                        A1[i] = A1[max];
                        A1[max] = temp;

                    }
                }
            
            smf2.Release(); 
        }
        static void Maksimum2()
        {
            smf.Release();
            smf.WaitOne();

            for (int i = A1.Length - 1; i >= 1; i--)
            {
                int max = i;
                for (int j = i - 1; j >= 0; j--)
                {

                    if (A1[max] < A1[j])
                    {
                        max = j;
                    }
                }
                    int temp = A1[i];
                    Thread.Sleep(100);
                    A1[i] = A1[max];
                    A1[max] = temp;
            }


            smf2.Release();
        }
        public static int[] A1 = new int[10];
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            Random rand = new Random();
            for (int i = 0; i < A1.Length; i++)
            {
                A1[i] = rand.Next(-100,100);
                textBox1.Text = textBox1.Text + A1[i] + "\r\n";
            }
                
        }
         private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            if (checkBox1.Checked == false)
            {
                Thread potok1 = new Thread(Puzirok2);
                Thread potok2 = new Thread(Maksimum2);
                potok1.Start();
                potok2.Start();
            }
            else
            {
                Thread potok1 = new Thread(Puzirok);
                Thread potok2 = new Thread(Maksimum);
                potok1.Start();
                potok2.Start();
            }
            
            smf1.WaitOne();
            smf2.WaitOne();

            for (int i = 0; i < A1.Length; i++)
            {
                textBox2.Text = textBox2.Text + A1[i] + "\r\n";
            }         
        }
    }
}
