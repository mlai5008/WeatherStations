using System;
using System.Net;
using System.Net.Sockets;
using WeatherStationsSimulator.Models.Models;

namespace WeatherStationsSimulator
{
    class Program
    {
        static void Main()
        {
            try
            {
                IPHostEntry ipHost = Dns.Resolve("127.0.0.1");
                var ipAddr = ipHost.AddressList[0];
                var ipEndPoint = new IPEndPoint(ipAddr, 11000);

                var sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(ipEndPoint);

                Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());
                Console.WriteLine("Введите Exit для выхода");
                Console.WriteLine("Нажмите s + Enter для - Отправки данных с Термодатчика");
                Console.WriteLine("Нажмите d + Enter для - Отправки данных с Датчика Влажности");
                Console.WriteLine("Нажмите f + Enter для - Отправки данных с Датчика Давления");
                Console.WriteLine("Нажмите g + Enter для - Отправки данных с Датчика Трёхосного акселерометра");
                Console.WriteLine("Нажмите h + Enter для - Отправки Не известных данных");
                while (true)
                {
                    var input = Console.ReadLine();
                    byte[] byteData = new byte[0] {};
                    if (input == "Exit")
                    {
                        break;
                    }
                    if (input == "s")
                    {
                        byteData  = new TemperatureModel().CreatePaket();
                    }
                    if (input == "d")
                    {
                        byteData = new HumidityModel().CreatePaket();
                    }
                    if (input == "f")
                    {
                        byteData = new PressureModel().CreatePaket();
                    }
                    if (input == "g")
                    {
                        byteData = new WindSpeedModel().CreatePaket();
                    }
                    if (input == "h")
                    {
                        byteData = new UnknowDataModel().CreatePaket();
                    }
                    var bytesSend = sender.Send(byteData);
                }
                
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
