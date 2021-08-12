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
    public class Program
    {               
        static void Main(string[] args)
        {
            MBSecure mbSecure = new MBSecure("10.2.10.99", "read", "write");
            mbSecure.ResetInput();
            StartUDPListener(mbSecure);
            //mBSecure.SetInput(1, 0);
            Console.Read();
        }

        static void StartUDPListener(MBSecure mbSecure)
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

                        if (packet.IsNotification && packet.Pdu.Type == PduType.V2Trap)
                        {
                            Task.Run(() => TrapHandler(packet,mbSecure));
                        }
                    }
                }
            });
        }

        static void TrapHandler(SnmpV2Packet packet, MBSecure mbSecure)
        {
            AxisCamera camera = new AxisCamera();
            if (camera.parseTrap(packet)){
                mbSecure.SetInput(camera.KameraID, 1);
            }
        }
    }
}
