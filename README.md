Serial port capture to PCAP
===========

This tool can capture serial port traffic and store all data in PCAP format. It is later possible to open it by Wireshark and analyze it.

This tool was created to capture Modbus-RTU on RS-485 but can be used to any other similar traffic.

Tutorial on using this capture is on YouTube https://www.youtube.com/watch?v=YtudbhexPv8

Tool is only for command line,

usage: `mono SerialPcap.exe [options] <portName>`

Option|Description
------+-----------
`-b, --baud=VALUE`|Serial port speed (default 9600)
`-g, --gap=VALUE`|Inter frame gap in miliseconds (default 10)
`-d, --dlt=VALUE`|Data link type in pcap format (default 147)
`-o, --output=VALUE`|Output file prefix (defalut port name)
`-h, --help`|Show this message and exit

`portName` is `COM1`, `\\.\COM15` or `/dev/ttyUSB0` or similar definition.


It is possible to run this tool using Mono on Linux or using .Net framework on Windows.
