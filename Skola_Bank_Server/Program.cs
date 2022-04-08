using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Skola_Bank_Server
{
    internal class Program
    {
        static TcpListener tcplistener;
        static void Main(string[] args)
        {
            WaitForConnection();
        }
        static void WaitForConnection()
        {
            IPAddress myIP = IPAddress.Parse("127.0.0.1");
            tcplistener = new TcpListener(myIP, 8001);
            tcplistener.Start();
            while (true)
            {
                try
                {
                    Console.WriteLine("Waiting for connection...");
                    Socket client = tcplistener.AcceptSocket();
                    Thread thread = new Thread(() => HandleClient(client));
                    Console.WriteLine($"{client.RemoteEndPoint} connected");
                    thread.Start();
                }

                catch (Exception e)
                {
                    Console.WriteLine("An error occured! " + e.Message);
                }
            }


        }

        static void HandleClient(Socket client)
        {
            bool connected = true;
            while (connected)
            {
                try
                {
                    string msg = SocketComm.RecvMsg(client);
                    switch (msg)
                    {
                        case "create account":
                            CreateUser(client);
                            break;

                        case "login":
                            Login(client);
                            break;

                        case "quit":
                            Console.WriteLine($"{client.RemoteEndPoint} disconnected");
                            client.Close();
                            connected = false;
                            break;

                    }
                }
                catch (ClientDisconnectedException)
                {
                    Console.WriteLine("CLient connection closed unexpectedly");
                    client.Close();
                    connected = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    client.Close();
                    connected = false;
                }
            }
        }

    }
}
