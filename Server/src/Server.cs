using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Server
{
    class Program
    {


        static void Main(string[] args)
        {
            Server s = new Server();
            s.ServerStart();
        }

        class  Server {
            bool isServerRunning;
            // Здесь будет список наших клиентов
            private Hashtable clients;
            // Это сокет нашего сервера
            Socket listener;
            // Порт, на котором будем прослушивать входящие соединения
            int port = 1055;
            // Точка для прослушки входящих соединений (состоит из адреса и порта)
            IPEndPoint Point;
            // Список потоков
            private List<Thread> threads = new List<Thread>();
            public void ServerStart()
            {
                clients = new Hashtable(30);
                isServerRunning = true;
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Определяем конечную точку, IPAddress.Any означает что наш сервер будет принимать входящие соединения с любых адресов
                Point = new IPEndPoint(IPAddress.Any, port);
                // Связываем сокет с конечной точкой
                listener.Bind(Point);
                // Начинаем слушать входящие соединения
                listener.Listen(10);

                SocketAccepter();
            }

            private void SocketAccepter()
            {
                    while (isServerRunning)
                    {
                        Socket client = listener.Accept();
                        Console.WriteLine("Client Connected");
                        clients.Add(client, "");
                        MessageReceiver(client);
                        byte[] sBytes = BitConverter.GetBytes((int) 123);
                        MessageSender(client,sBytes);
                    }


            }

            private void MessageReceiver(Socket r_client)
            {
                // Для каждого нового подключения, будет создан свой поток для приема пакетов
                Thread th = new Thread(delegate()
                {
                    while (isServerRunning)
                    {
                        try
                        {
                            MemoryStream stream = new MemoryStream();

                            // Сюда будем записывать принятые байты
                            byte[] bytes = new byte[4];
                            // Принимаем

                            r_client.Receive(bytes,4,SocketFlags.None);
                            Int32 a = BitConverter.ToInt32(bytes, 0);
                            Console.WriteLine(Convert.ToSingle(a));
                            continue;
                           /* if (bytes.Length != 0)
                            {
                                // Отсылаем принятый пакет от клиента всем клиентам
                                foreach (Socket s_client in clients.Keys)
                                {
                                    MessageSender(s_client, bytes);
                                }
                            }
                            * */
                        }
                        catch { }
                    }
                });
                th.Start();
                threads.Add(th);
            }

            private void MessageSender(Socket c_client, byte[] bytes)
            {
                try
                {
                    // Отправляем пакет
                    c_client.Send(bytes);

                }
                catch { }
            }
        }
    }
}
