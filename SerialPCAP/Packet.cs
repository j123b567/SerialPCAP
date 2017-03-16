using System;
using System.IO;
using System.Collections.Generic;

namespace SerialPCAP
{
	public class Packet
	{
		public DateTime Timestamp;
		public List<byte> Data;

		public Packet()
		{
			Timestamp = DateTime.Now;
			Data = new List<byte>();
		}

		public void Write(BinaryWriter writer)
		{
			double unixTimestamp = ConvertToUnixTimestamp(Timestamp);
			var ts_sec = (uint)Math.Floor(unixTimestamp);
			var ts_usec = (uint)((unixTimestamp - ts_sec)*1000000);
			writer.Write(ts_sec);
			writer.Write(ts_usec);

			byte[] data = Data.ToArray();
			writer.Write(data.Length);
			writer.Write(data.Length);
			writer.Write(data);
		}

		public static double ConvertToUnixTimestamp(DateTime date)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan diff = date.ToUniversalTime() - origin;
			return diff.TotalSeconds;
		}
	}
}

