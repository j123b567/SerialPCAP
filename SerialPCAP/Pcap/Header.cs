using System;
using System.IO;
namespace Pcap
{
	public static class Header
	{
		public static void Write(BinaryWriter writer, uint dlt)
		{
			writer.Write((uint)0xa1b2c3d4); /* magic number */
			writer.Write((ushort)2); /* major version number */
			writer.Write((ushort)4); /* minor version number */
			writer.Write((int)0); /* GMT to local correction */
			writer.Write((uint)0); /* accuracy of timestamps */
			writer.Write((uint)1024); /* max length of captured packets, in octets */
			writer.Write((uint)dlt); /* data link type */
		}

		public static uint Read(BinaryReader reader)
		{
			uint magic = reader.ReadUInt32();
			ushort versionMajor = reader.ReadUInt16();
			ushort versionMinor = reader.ReadUInt16();
			/*int gmtCorr = */reader.ReadInt32();
			/*uint accuracy = */reader.ReadUInt32();
			/*uint maxLength = */reader.ReadUInt32();
			uint dlt = reader.ReadUInt32();

			if (magic != 0xa1b2c3d4) throw new Exception("Invalid header");
			if (versionMajor != 2) throw new Exception("Invalid major version");
			if (versionMinor != 4) throw new Exception("Invalid minor version");

			return dlt;
		}
	}
}

