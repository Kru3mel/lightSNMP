# lightSNMP
## Description
A lightweight SNMP Network Management Station able to receive traps, send GET and SET Requests.
In its current implementation it is able to use the traps sent from a Axis Communications Camera running the Axis Perimeter Defender to be used a virtual inputs for the intrusion alarm control MBSecure from Honeywell with a SNMP- License.
## Prerequisites
* Axis Camera
* Axis Perimeter Defender Licens for the camera
* Honeywell MBSecure intrusion alarm control
* SNMP extension license
* .NET Framework 4.8 or higher
## Axis Camera Settings
### SNMP Settings
To be able to use SNMP- Traps to signal alarm evnets the Axis Cameras SNMP- Settings need to be set correctly.
The configuration can be done using the cameras webinterface. To set up SNMP navigate to → System → SNMP and set SNMP from __off__ to __v1 and v2c__
|SNMP|Community Read|Community Write|Activate Traps|Trap-Adress|Trap-Community|Traps|
|---|---|---|---|---|---|---|
|v1 and v2c|*read*|*write*|✅|Device-IP|*trap*|all ⬜|

All terms written in italics can be chosen freely and won't affect this program.
### Event Settings
In order to be able to use the traps sent from a Axis camera the alarm event configuration has to match the parameters from the follwing table.
The configuration can be done using the cameras webinterface. To add the required Event Handler navigate to → System → Events → Add Event (:heavy_plus_sign:)
|Event Name|Condition|Use Condition as Trigger|feature|enabled|Action|Alarm Name|Alarm Notification|
|---|---|---|---|---|---|---|---|
|Alarm On|AXISPerimterDefender|✅|ALL_SENARIOS|✅|Send SNMP-Trap|Intrusion Camera xx|true|
|Alarm Off|AXISPerimterDefender|✅|ALL_SENARIOS|⬜|Send SNMP-Trap|Intrusion Camera xx|false|
