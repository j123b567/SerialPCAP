Serial port capture to PCAP
===========

This tool can capture serial port traffic and store all data in PCAP format. It is later possible to open it by Wireshark and analyze it.

This tool was created to capture Modbus-RTU on RS-485 but can be used to any other similar traffic.

Tutorial on using this capture is on YouTube https://www.youtube.com/watch?v=YtudbhexPv8

Tool is only for command line,
usage: `serialpcap <portName> [<baudRate> [<frameGapMs]]`

- `portName` is name of the port, e.g. COM1, /dev/ttyUSB0, ...
- `baudrate` is speed of the serial port (default 9600)
- `frameGapMs` is gap between frames in ms (default 10)
