Serial port capture to PCAP
===========

This tool can capture serial port traffic and store all data in PCAP format. It is later possible to open it by Wireshark and analyze it. It is also possible to use realtime mode with named pipe instead of file.

This tool was created to capture Modbus-RTU on RS-485 but can be used to any other similar traffic.

Tutorial on using this capture is on YouTube https://www.youtube.com/watch?v=YtudbhexPv8

Tool is only for command line,

usage: `mono SerialPcap.exe [options] <portName>`

Option | Description
------ | -----------
`-b, --baud=VALUE` | Serial port speed (default 9600)
`-y, --parity=VALUE` | o (=odd), e (=even), n (=none) (defaul none)
`-p, --stopbits=VALUE` | 1, 2 (defaul 1)
`-g, --gap=VALUE` | Inter frame gap in miliseconds (default 10)
`-d, --dlt=VALUE` | Data link type in pcap format (default 147)
`-o, --output=VALUE` | Output file prefix (defalut port name)
`--pipe` | Use named pipe instead of file
`-h, --help` | Show this message and exit
`-s, --stdout` | Use stdout instead of file

`portName` is `COM1`, `\\.\COM15` or `/dev/ttyUSB0` or similar definition.


It is possible to run this tool using Mono on Linux or using .Net framework on Windows.


Pipe (realtime) mode on linux
-----------
It is possible to run the application in pipe mode, so you can see realtime traffic in Wireshark. On linux, you should perform these commands

    mkfifo /tmp/wspipe
    wireshark -k -i /tmp/wspipe &
    mono SerialPcap -o /tmp/wspipe --pipe [options] <portName>

More info on Wireshark capture pipes can be seen on https://wiki.wireshark.org/CaptureSetup/Pipes


Using stdout (realtime) mode in both Windows and Linux
-----------
Another useful option to see realtime traffic in Wireshark is to use stdout piping:

    SerialPcap -s | wireshark -k -i -
