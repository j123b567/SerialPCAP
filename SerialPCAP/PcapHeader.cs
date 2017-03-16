using System;
using System.IO;
namespace SerialPCAP
{
	public static class PcapHeader
	{
		public static void Write(BinaryWriter writer)
		{
			writer.Write((uint)0xa1b2c3d4); /* magic number */
			writer.Write((ushort)2); /* major version number */
			writer.Write((ushort)4); /* minor version number */
			writer.Write((int)0); /* GMT to local correction */
			writer.Write((uint)0); /* accuracy of timestamps */
			writer.Write((uint)1024); /* max length of captured packets, in octets */
			writer.Write((uint)147); /* data link type */
		}
	}
}

