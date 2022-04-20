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
                    LogManager.AddLog(client.RemoteEndPoint.ToString(), "connected", logType.ConnectionLog);
                    thread.Start();
                }

                catch (NonExistingElementException e)
                {
                    Console.WriteLine("An error occured! " + e.Message);
                }
            }


        }

        static void HandleClient(Socket client)
        {
            bool connected = true;
            // ip is saved in case a unexpected disconnect happens
            string clientIp = client.RemoteEndPoint.ToString();
            while (connected)
            {
                try
                {
                    string msg = SocketComm.RecvMsg(client);
                    switch (msg)
                    {
                        case "create account":
                            UserCreator.CreateUser(client);
                            break;

                        case "login":
                            LoginManager.Login(client);
                            break;

                        case "quit":
                            LogManager.AddLog(client, "Client disconnected", logType.ConnectionLog);
                            client.Close();
                            connected = false;
                            break;

                    }
                }
                catch (ClientDisconnectedException)
                {
                    LogManager.AddLog(clientIp, "client disconnected unexpectedly", logType.ConnectionLog);
                    client.Close();
                    connected = false;
                }
                catch (Exception e)
                {
                    LogManager.AddLog(clientIp, $"{e.Message} stacktrace: {e.StackTrace}", logType.ErrorLog);
                    client.Close();
                    connected = false;
                }
            }
        }

    }
}
