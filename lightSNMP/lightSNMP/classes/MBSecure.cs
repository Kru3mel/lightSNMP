﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;

using System.Net;
using SnmpSharpNet;
using System.Net.NetworkInformation;
using System.Threading;

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
            client.Client.ReceiveTimeout = 2000;
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
        /// Resets every input in range of inputCount
        /// </summary>
        /// <param name="inputCount"></param>
        public void ResetInputs(int inputCount)
        {
            Console.WriteLine("Waiting for Panel State");
            while (!CheckPanelState())
            {

            }
            Console.WriteLine("Reseting Inputs");
            for(int i = 0; i < inputCount; i++)
            {
                try
                {
                    SetInput(i + 1, 0);
                }
                catch
                {
                    Console.WriteLine("Could not reset input: " + (i+1));
                }
            }      
        }
        /// <summary>
        /// Used to assue that the MBSecure is online and able to process requests
        /// </summary>
        /// <returns></returns>
        public bool CheckPanelState()
        {
            try
            {
                //Create SNMP- GET Packet to Get the Panel State
                var getStatePacket = createSnmpGet(".1.3.6.1.4.1.48689.1.1.1.0").encode();

                //Send the created Packet
                getStatePacket = SendUDPPacket(getStatePacket);

                //Decode the received Paket
                SnmpV2Packet packet = new SnmpV2Packet();
                packet.decode(getStatePacket, getStatePacket.Length);

                //Extract the value of the Panel State
                int state = int.Parse(packet.Pdu.VbList[0].Value.ToString());
                Console.WriteLine("Panel State: " + state);

                //Only return true if Panel State is Ready(0)
                if (state == 0)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Panel not Ready");
                    return false;
                }
            }
            //Catch Socket and SNMP Exceptions
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
