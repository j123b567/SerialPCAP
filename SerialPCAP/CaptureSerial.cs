using System;
using System.IO;
using System.IO.Ports;
namespace SerialPCAP
{
	public class CaptureSerial
	{
		SerialPort m_serialPort;
		Packet m_packet = null;

		public CaptureSerial(string portName, int baudRate, int readTimeout)
		{
			m_serialPort = new SerialPort();
			m_serialPort.PortName = portName;
			m_serialPort.BaudRate = baudRate;
			m_serialPort.Parity = Parity.None;
			m_serialPort.StopBits = StopBits.One;
			m_serialPort.DataBits = 8;
			m_serialPort.Handshake = Handshake.None;
			m_serialPort.ReadTimeout = readTimeout;
			m_serialPort.Open();
		}

		public void Close()
		{
			m_serialPort.Close();
		}

		public bool WritePacket(BinaryWriter writer)
		{
			try
			{
				while (true)
				{
					var b = (byte)m_serialPort.ReadByte();
					if (m_packet == null)
					{
						m_packet = new Packet();
					}
					m_packet.Data.Add(b);
					if (m_packet.Data.Count >= 300)
					{
						throw new TimeoutException();
					}
				}
			}
			catch (TimeoutException)
			{
				if (m_packet != null)
				{
					m_packet.Write(writer);
					m_packet = null;
				}
			}
			return true;
		}
	}
}

