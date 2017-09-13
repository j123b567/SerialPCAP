using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
namespace SerialPCAP
{
	public class CaptureSerial : IDisposable
	{
		readonly static int MAX_BYTES_PER_PACKET = 300;
		readonly SerialPort m_serialPort;
		readonly List<byte> m_buffer = new List<byte>(MAX_BYTES_PER_PACKET);

		public CaptureSerial(string portName, int baudRate, char parity, int stopbits, int readTimeout)
		{
			m_serialPort = new SerialPort();
			m_serialPort.PortName = portName;
			m_serialPort.BaudRate = baudRate;
			switch (Char.ToLower(parity))
			{
				case 'o':
					m_serialPort.Parity = Parity.Odd;
					break;
				case 'e':
					m_serialPort.Parity = Parity.Even;
					break;
				default:
					m_serialPort.Parity = Parity.None;
					break;
			}
			switch (stopbits)
			{
				case 1:
				default:
					m_serialPort.StopBits = StopBits.One;
					break;
				case 2:
					m_serialPort.StopBits = StopBits.Two;
					break;
			}
			m_serialPort.DataBits = 8;
			m_serialPort.Handshake = Handshake.None;
			m_serialPort.ReadTimeout = readTimeout;
			m_serialPort.Open();
		}

		public void Close()
		{
			m_serialPort.Close();
		}

		public Pcap.Packet CapturePacket()
		{
			Pcap.Packet packet = null;

			try
			{
				while (true)
				{
					var b = m_serialPort.ReadByte();
					if (b < 0)
					{
						throw new EndOfStreamException();
					}
					if (packet == null)
					{
						m_buffer.Clear();
						packet = new Pcap.Packet();
					}
					m_buffer.Add((byte)b);
					if (m_buffer.Count >= MAX_BYTES_PER_PACKET)
					{
						throw new TimeoutException();
					}
				}
			}
			catch (TimeoutException)
			{
				if (packet != null)
				{
					packet.Data = m_buffer.ToArray();
				}
			}
			catch (EndOfStreamException)
			{
				if (packet != null)
				{
					packet.Data = m_buffer.ToArray();
				}
				else {
					throw;
				}
			}
			return packet;
		}

		bool disposed = false;
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing)
			{
				m_serialPort.Dispose();
			}

			disposed = true;
		}
	}
}

