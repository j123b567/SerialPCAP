using System;
using System.IO;
using System.IO.Ports;

namespace SerialPCAP
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("usage: serialpcap <portName> [<baudRate> [<frameGapMs> [<pcapDLT>]]]");
				Console.WriteLine();
				Console.WriteLine("Available portName:");
				// Get a list of serial port names.
				string[] ports = SerialPort.GetPortNames();
				// Display each port name to the console.
				foreach (string port in ports)
				{
					Console.WriteLine("    " + port);
				}
				Console.WriteLine();
				Console.WriteLine("baudRate: serial port speed (default 9600)");
				Console.WriteLine();
				Console.WriteLine("frameGapMs: inter frame gap in miliseconds (defailt 10)");
				Console.WriteLine();
				Console.WriteLine("pcapDLT: data link type in pcap format (defailt 147)");
			}
			else {
				string portName = args[0];
				int baudRate = 9600;
				if (args.Length >= 2) baudRate = Int32.Parse(args[1]);
				int frameGapMs = 10;
				if (args.Length >= 3) frameGapMs = Int32.Parse(args[2]);
				uint dlt = 147;
				if (args.Length >= 3) dlt = UInt32.Parse(args[3]);

				string outputFile = "serial-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + ".pcap";
				var capture = new CaptureSerial(portName, baudRate, frameGapMs);

				Console.WriteLine("Serial port: " + portName);
				Console.WriteLine("Baud rate: " + baudRate + " Bd");
				Console.WriteLine("Frame gap: " + frameGapMs + " ms");
				Console.WriteLine("DLT: " + dlt);
				Console.WriteLine("Output file: " + outputFile);
				Console.WriteLine();
				Console.WriteLine("Starting capture (press Ctrl+c to stop)");

				using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
				{
					PcapHeader.Write(writer, dlt);

					while (capture.WritePacket(writer))
					{
						writer.Flush();
					}
				}

				capture.Close();
			}
		}
	}
}
