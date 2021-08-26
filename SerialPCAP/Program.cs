using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
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
		static char parity = 'n';
		static int stopbits = 1;
		static bool pipe = false;
		static bool stdout = false;

		static void ShowHelp(OptionSet options)
		{
			Console.WriteLine("{0} v{1}", AssemblyExtensions.Description(), AssemblyExtensions.Version());
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

		static void Capture(CaptureSerial capture, BinaryWriter writer)
		{
			Pcap.Header.Write(writer, dlt);
			writer.Flush();
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

		static void RunCapture()
		{
			if (stdout == false)
			{
				Console.WriteLine("Serial port: " + portName);
				Console.WriteLine("Baud rate: " + baudRate + " Bd");
				Console.WriteLine("Parity: " + parity);
				Console.WriteLine("Stopbits: " + stopbits);
				Console.WriteLine("Frame gap: " + frameGapMs + " ms");
				Console.WriteLine("DLT: " + dlt);
				Console.WriteLine("Output file: " + outputFile);
				Console.WriteLine();
				Console.WriteLine("Starting capture (press Ctrl+c to stop)");
			}

			using (var capture = new CaptureSerial(portName, baudRate, parity, stopbits, frameGapMs))
			{
				if (stdout)
				{
					using (BinaryWriter writer = new BinaryWriter(Console.OpenStandardOutput()))
					{
						Capture(capture, writer);
					}
				}
				else if (pipe && Environment.OSVersion.Platform != PlatformID.Unix)
				{
					using (BinaryWriter writer = new BinaryWriter(new NamedPipeServerStream(outputFile, PipeDirection.Out)))
					{
						Capture(capture, writer);
					}
				}
				else
				{
					using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
					{
						Capture(capture, writer);
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
				{"y|parity=", "o (=odd) | e (=even) | n (=none) (defaul none)", (char y) => parity = y},
				{"p|stopbits=", "1 | 2 (defaul 1)", (int p) => stopbits = p},
				{"g|gap=", "Inter frame gap in miliseconds (default 10)", (int g) => frameGapMs = g},
				{"d|dlt=", "Data link type in pcap format (default 147)", (uint d) => dlt = d},
				{"o|output=", "Output file prefix or pipe (defalut port name)", o => outputFilePrefix = o},
				{"pipe", "Use named pipe instead of file", p => pipe = p != null},
				{"h|help", "Show this message and exit", h => shouldShowHelp = h != null},
				{"s|stdout", "Use stdout instead of file", s => stdout = s != null},
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

			if (pipe)
			{
				if (outputFilePrefix == "")
				{
					outputFilePrefix = "serialpcap-pipe";
				}
				outputFile = outputFilePrefix;
			}
			else
			{
				if (outputFilePrefix == "")
				{
					var s = portName.Split(new char[] { '/', '\\' });
					outputFilePrefix = s[s.Length - 1];
				}

				outputFile = outputFilePrefix + "-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + ".pcap";
			}
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
