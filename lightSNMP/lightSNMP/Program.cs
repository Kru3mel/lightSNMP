using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Net;
using System.Net.Sockets;
using SnmpSharpNet;

using System.Configuration;
using System.Collections.Specialized;

namespace lightSNMP
{
    public class Program
    {               
        static void Main(string[] args)
        {
            //Get the needed values from config file
            string ip = ConfigurationManager.AppSettings.Get("mbsecureIP");
            string readCommunity = ConfigurationManager.AppSettings.Get("mbsecureReadCommunity");
            string writeCommunity = ConfigurationManager.AppSettings.Get("mbsecureWriteCommunity");

            //initialise mbsecure object
            MBSecure mbSecure = new MBSecure(ip, readCommunity, writeCommunity);
            mbSecure.ResetInputs(2);

            //Start UDP-Listener in seperat Thread
            StartUDPListener(mbSecure);

            Console.Read();
        }

        static void StartUDPListener(MBSecure mbSecure)
        {            
            Task.Run(() =>
            {
                //Start UPD (SNMP) Listerner on port 162 in new Tread
                using(UdpClient client = new UdpClient(162))
                {
                    Console.WriteLine("Listening");
                    while (true)
                    {
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 162);
                        byte[] receivedBytes = client.Receive(ref endPoint);

                        //Write IP Address from packet sender to consol
                        Console.WriteLine(endPoint.Address);

                        try
                        {
                            //Decode received packet
                            SnmpV2Packet packet = new SnmpV2Packet();
                            packet.decode(receivedBytes, receivedBytes.Length);
                            //Write decodes packet to consol
                            Console.WriteLine(packet.ToString());

                            //start Trap Handler in seperat thread if received packet is a SNMPV2 Trap
                            if (packet.IsNotification && packet.Pdu.Type == PduType.V2Trap)
                            {
                                Task.Run(() => TrapHandler(packet, mbSecure));
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Could not decode packet");
                        }
                    }
                }
            });
        }
        
        static void TrapHandler(SnmpV2Packet packet, MBSecure mbSecure)
        {
            //initialise camera objcet
            AxisCamera camera = new AxisCamera();
            //set corresponding input in MBSecure if trap signals alarm
            if (camera.parseTrap(packet)){
                if(camera.AlarmText == "true")
                {
                    mbSecure.SetInput(camera.KameraID, 1);
                }
                else
                {
                    mbSecure.SetInput(camera.KameraID, 0);
                }
            }
        }
    }
}
