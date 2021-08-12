using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnmpSharpNet;

namespace lightSNMP
{
    class AxisCamera
    {
        public AxisCamera()
        {

        }

        public int AlarmID { get; private set; } 

        public string AlarmName { get; private set; }

        public string AlarmText { get; private set; }

        public int KameraID { get; private set; }

        public bool parseTrap(SnmpV2Packet packet)
        {
            if (!packet.Pdu.TrapObjectID.ToString().Contains("1.3.6.1.4.1.368"))
            {
                Console.WriteLine("Packet does not originate from axis device");
                return false;                
            }
            else if (!(packet.Pdu.TrapObjectID.ToString() == "1.3.6.1.4.1.368.4.2.0.3"))
            {
                Console.WriteLine("Packet does not qualify as alarm");
                return false;
            }
            else
            {
                AlarmID = int.Parse(packet.Pdu.VbList[0].Value.ToString());
                AlarmName = packet.Pdu.VbList[1].Value.ToString();
                AlarmText = packet.Pdu.VbList[2].Value.ToString();

                string cameraNumber = new string(AlarmText.Where(Char.IsDigit).ToArray());
                KameraID = int.Parse(cameraNumber);

                return true;
            }
        }
    }
}
