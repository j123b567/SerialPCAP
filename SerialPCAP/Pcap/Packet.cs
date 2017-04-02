using System;
using System.IO;
using System.Collections.Generic;

namespace Pcap
{
	public class Packet
	{
		public DateTime Timestamp;
		public byte[] Data;

		public Packet()
		{
			Timestamp = DateTime.UtcNow;
			Data = new byte[0];
		}

		public Packet(BinaryReader reader)
		{
			Read(reader);
		}

		public void Write(BinaryWriter writer)
		{
			double unixTimestamp = ConvertToUnixTimestamp(Timestamp);
			var ts_sec = (uint)Math.Floor(unixTimestamp);
			var ts_usec = (uint)((unixTimestamp - ts_sec)*1000000);
			writer.Write(ts_sec);
			writer.Write(ts_usec);

			writer.Write(Data.Length);
			writer.Write(Data.Length);
			writer.Write(Data);
		}

		public void Read(BinaryReader reader)
		{
			uint ts_sec = reader.ReadUInt32();
			uint ts_usec = reader.ReadUInt32();
			int length = reader.ReadInt32();
			reader.ReadUInt32(); // orig length

			Data = reader.ReadBytes(length);
			Timestamp = ConvertToDateTime(ts_sec, ts_usec);
		}

		static double ConvertToUnixTimestamp(DateTime date)
		{
			var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan diff = date.ToUniversalTime() - origin;
			return diff.TotalSeconds;
		}

		static DateTime ConvertToDateTime(uint sec, uint usec)
		{
			var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			double seconds = sec + usec / 1000000.0;
			return origin.AddSeconds(seconds);
		}
	}
}

