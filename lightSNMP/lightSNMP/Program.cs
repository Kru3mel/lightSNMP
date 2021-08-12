using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Net;
using System.Net.Sockets;
using SnmpSharpNet;

namespace lightSNMP
{
    class Program
    {        
        static void Main(string[] args)
        {
            UDPListener();

            while (true)
            {
                Console.WriteLine(DateTime.Now);
                Thread.Sleep(10000);
            }
            
        }

        static void UDPListener()
        {            
            Task.Run(() =>
            {
                using(UdpClient client = new UdpClient(162))
                {
                    Console.WriteLine("Listening");
                    while (true)
                    {
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 162);
                        byte[] receivedBytes = client.Receive(ref endPoint);

                        Console.WriteLine(endPoint.Address);
                        SnmpV2Packet packet = new SnmpV2Packet();
                        packet.decode(receivedBytes, receivedBytes.Length);
                        Console.WriteLine(packet.ToString());
                    }
                }
            });
        }
    }
}
