using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using Mono.Options;

namespace SerialPCAP
{
	class MainClass
	{
		static int baudRate = 9600;
		static int frameGapMs = 10;
		static uint dlt = 147;
		static string portName;
		static string outputFile;

		static void ShowHelp(OptionSet options)
		{
			Console.WriteLine("usage: serialpcap [options] <portName>");
			options.WriteOptionDescriptions(Console.Out);
			Console.WriteLine();
			Console.WriteLine("Available portName:");
			// Get a list of serial port names.
			string[] ports = SerialPort.GetPortNames();
			// Display each port name to the console.
			foreach (string port in ports)
			{
				Console.WriteLine("    " + port);
			}
		}

		static void RunCapture()
		{
			Console.WriteLine("Serial port: " + portName);
			Console.WriteLine("Baud rate: " + baudRate + " Bd");
			Console.WriteLine("Frame gap: " + frameGapMs + " ms");
			Console.WriteLine("DLT: " + dlt);
			Console.WriteLine("Output file: " + outputFile);
			Console.WriteLine();
			Console.WriteLine("Starting capture (press Ctrl+c to stop)");

			using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
			{
				Pcap.Header.Write(writer, dlt);
				writer.Flush();

				using (var capture = new CaptureSerial(portName, baudRate, frameGapMs))
				{
					while (true)
					{
						var packet = capture.CapturePacket();
						if (packet != null)
						{
							packet.Write(writer);
							writer.Flush();
						}
					}
				}
			}
		}

		public static void Main(string[] args)
		{
			string outputFilePrefix = "";
			bool shouldShowHelp = false;

			var options = new OptionSet {
				{"b|baud=", "Serial port speed (default 9600)", (int b) => baudRate = b},
				{"g|gap=", "Inter frame gap in miliseconds (default 10)", (int g) => frameGapMs = g},
				{"d|dlt=", "Data link type in pcap format (default 147)", (uint d) => dlt = d},
				{"o|output=", "Output file prefix (defalut port name)", o => outputFilePrefix = o},
				{"h|help", "Show this message and exit", h => shouldShowHelp = h != null},
			};

			List<string> extra;
			try
			{
				// parse the command line
				extra = options.Parse(args);
			}
			catch (OptionException e)
			{
				// output some error message
				Console.Write("serialpcap: ");
				Console.WriteLine(e.Message);
				Console.WriteLine("Try `serialpcap --help' for more information.");
				return;
			}

			if (shouldShowHelp || extra.Count != 1)
			{
				ShowHelp(options);
				return;
			}

			portName = extra[0];

			if (outputFilePrefix == "")
			{
				var s = portName.Split(new char[] { '/', '\\' });
				outputFilePrefix = s[s.Length - 1];
			}

			outputFile = outputFilePrefix + "-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + ".pcap";

			try
			{
				RunCapture();
			}
			catch (Exception e)
			{
				Console.WriteLine("Error occurred: " + e.Message);
			}
		}
	}
}
