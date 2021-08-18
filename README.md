# lightSNMP
## Description
A lightweight SNMP Network Management Station able to receive traps, send GET and SET Requests.
In its current implementation it is able to use the traps sent from a Axis Communications Camera running the Axis Perimeter Defender to be used a virtual inputs for the intrusion alarm control MBSecure from Honeywell with a SNMP- License.
## Axis Camera Settings
In order to be able to use the traps sent from a Axis camera the alarm event configuration has to match the parameters from the follwing table.
The configuration can be done using the cameras webinterface. To add an Event Handeler navigate to → System → Events → Add Event (:heavy_plus_sign:)
|Event Name|Condition|Use Condition as Trigger|feature|enabled|Action|Alarm Name|Alarm Notification|
|---|---|---|---|---|---|---|---|
|Alarm On|AXISPerimterDefender|:white_check_mark:|ALL_SENARIOS|:white_check_mark:|Send SNMP-Trap|Intrusion Camera xx|true|
|Alarm Off|AXISPerimterDefender|:white_check_mark:|ALL_SENARIOS|:white_large_square:|Send SNMP-Trap|Intrusion Camera xx|false|
