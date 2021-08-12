using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnmpSharpNet;

namespace lightSNMP.classes
{
    class MBSecure
    {
        public MBSecure(string ip, string oid, string readCommunity, string writeCommunity)
        {
            IPAdress = ip;
            OID = oid;
            ReadCommunity = readCommunity;
            WriteCommunity = writeCommunity;
        }

        public string IPAdress { get; }

        public string OID { get; }

        public string ReadCommunity { get; }

        public string WriteCommunity { get; }

        private SnmpV2Packet createSnmpGet(string oid)
        {
            SnmpV2Packet packet = new SnmpV2Packet();
            packet.Community.Set(ReadCommunity);
            packet.Pdu.Type = PduType.Get;
            packet.Pdu.RequestId = 100;
            packet.Pdu.VbList.Add(oid);
            return packet;
        }

        private SnmpV2Packet createSnmpSet(string oid, string value)
        {
            SnmpV2Packet packet = new SnmpV2Packet();
            packet.Community.Set(WriteCommunity);
            packet.Pdu.Type = PduType.Set;
            packet.Pdu.RequestId = 101;
            packet.Pdu.VbList.Add(new Oid(oid), new OctetString(value));
            return packet;
        }
    }
}
