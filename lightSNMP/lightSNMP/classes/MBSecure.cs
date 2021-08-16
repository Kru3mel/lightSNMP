using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;

using System.Net;
using SnmpSharpNet;

namespace lightSNMP
{
    class MBSecure
    {
        Oid oidInput = new Oid(".1.3.6.1.4.1.48689.1.5.1");
        UdpClient client;

        public MBSecure(string ip, string readCommunity, string writeCommunity)
        {
            IPAdress = ip;
            ReadCommunity = readCommunity;
            WriteCommunity = writeCommunity;

            client = new UdpClient(10000);
            //clientSend = new UdpClient(9999);
        }

        /// <summary>
        /// IP- Address of Honeywell MBSecure
        /// </summary>
        public string IPAdress { get; }

        /// <summary>
        /// OID of Honeywell MBSecure
        /// </summary>
        public string OID { get; }

        /// <summary>
        /// Community Name to Read Data (get)
        /// </summary>
        public string ReadCommunity { get; }

        /// <summary>
        /// Community Name to Write Dara (set)
        /// </summary>
        public string WriteCommunity { get; }

        /// <summary>
        /// Creates a SNMP- GET Packet with specified OID
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        private SnmpV2Packet createSnmpGet(string oid)
        {
            SnmpV2Packet packet = new SnmpV2Packet();
            packet.Community.Set(ReadCommunity);
            packet.Pdu.Type = PduType.Get;
            packet.Pdu.RequestId = 100;
            packet.Pdu.VbList.Add(oid);
            return packet;
        }

        /// <summary>
        /// Creates a SNMP- SET Packet with specified OID and Value
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private SnmpV2Packet createSnmpSet(string oid, string value)
        {
            SnmpV2Packet packet = new SnmpV2Packet();
            packet.Community.Set(WriteCommunity);
            packet.Pdu.Type = PduType.Set;
            packet.Pdu.RequestId = 101;
            packet.Pdu.VbList.Add(new Oid(oid), new OctetString(value));
            return packet;
        }

        /// <summary>
        /// Creates a SNMP- SET Packet with specified OID and Value
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private SnmpV2Packet createSnmpSet(string oid, int value)
        {
            SnmpV2Packet packet = new SnmpV2Packet();
            packet.Community.Set(WriteCommunity);
            packet.Pdu.Type = PduType.Set;
            packet.Pdu.RequestId = 101;
            packet.Pdu.VbList.Add(new Oid(oid), new Integer32(value));
            return packet;
        }

        /// <summary>
        /// Send a UDP- Packet to the MBSecure and Returns the Response
        /// </summary>
        /// <param name="udpPacket"></param>
        private byte[] SendUDPPacket(byte[] udpPacket)
        {            
            try
            {
                client.Connect(IPAdress, 161);
                client.Send(udpPacket, udpPacket.Length);

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 161);
                return client.Receive(ref endPoint);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new byte[0];
            }
        }

        /// <summary>
        /// Creates and sends a SNMP- Packet to the MBSecure to set a certain Input
        /// </summary>
        /// <param name="number"></param>
        /// <param name="value"></param>
        public void SetInput(int number, int value)
        {
            Oid temp = new Oid(oidInput);
            //OID for inputRelease
            temp.Add(3);
            //OID for inpuReleaseIndex
            temp.Add(number);
            Debug.WriteLine(temp.ToString());
            //Create a SNMP- Byte Array containing OID and Value
            byte[] packet = createSnmpSet(temp.ToString(), value).encode();
            //Send the SNMP- Byte Array
            packet = SendUDPPacket(packet);

            SnmpV2Packet snmpV2Packet = new SnmpV2Packet();
            snmpV2Packet.decode(packet, packet.Length);
            Console.WriteLine(snmpV2Packet.ToString());
        }

        /// <summary>
        /// Creates and sends a SNMP- Packet to reset any set Inputs
        /// </summary>
        public void ResetInput()
        {
            Console.WriteLine("Reseting Inputs");
            for(int i=0; i<2; i++)
            {
                try
                {
                    Oid temp = new Oid(oidInput);
                    //OID for inputRelease
                    temp.Add(3);
                    //OID for inputReleaseIndex
                    temp.Add(i + 1);
                    Debug.WriteLine(temp.ToString(), "=0");
                    //Create a SNMP- Byte Array containing OID and Value
                    byte[] packet = createSnmpSet(temp.ToString(), 0).encode();
                    //Send the SNMP- Byte Array
                    SendUDPPacket(packet);
                }
                catch
                {
                    Console.WriteLine("Could not reset input: " + (i + 1));
                }
            }            
        }
    }
}
