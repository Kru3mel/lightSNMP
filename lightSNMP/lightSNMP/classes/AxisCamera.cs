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
        string axisOID = "1.3.6.1.4.1.368";
        string axisAlarmOID = "1.3.6.1.4.1.368.4.2.0.3";

        public AxisCamera()
        {

        }

        /// <summary>
        /// Unique ID to specify alarm type
        /// </summary>
        public int AlarmID { get; private set; } 

        /// <summary>
        /// Name of Alarm of SNMP-Trap
        /// </summary>
        public string AlarmName { get; private set; }

        /// <summary>
        /// Text send with alarm SNMP- Trap
        /// </summary>
        public string AlarmText { get; private set; }

        /// <summary>
        /// Camara ID extracted from Alarm Text
        /// </summary>
        public int KameraID { get; private set; }

        /// <summary>
        /// Takes in a SNMP Trap and tries to extract the class parameters
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public bool parseTrap(SnmpV2Packet packet)
        {
            if (!packet.Pdu.TrapObjectID.ToString().Contains(axisOID))
            {
                Console.WriteLine("Packet does not originate from axis device");
                return false;                
            }
            else if (!(packet.Pdu.TrapObjectID.ToString() == axisAlarmOID))
            {
                Console.WriteLine("Packet does not qualify as alarm");
                return false;
            }
            else
            {
                AlarmID = int.Parse(packet.Pdu.VbList[0].Value.ToString());
                AlarmName = packet.Pdu.VbList[1].Value.ToString();
                AlarmText = packet.Pdu.VbList[2].Value.ToString();

                string cameraNumber = new string(AlarmName.Where(Char.IsDigit).ToArray());
                KameraID = int.Parse(cameraNumber);

                return true;
            }
        }
    }
}
