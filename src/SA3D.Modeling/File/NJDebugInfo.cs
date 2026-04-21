using SA3D.Common.IO;
using System.Collections.Generic;

namespace SA3D.Modeling.File
{
	/// <summary>
	/// Debug/diagnostic helpers for NJ container inspection.
	/// </summary>
	public static class NJDebugInfo
	{
		/// <summary>
		/// Read a flat list of NJ blocks from a binary blob.
		/// </summary>
		public static IReadOnlyList<NJBlockInfo> ReadBlocks(byte[] data, uint address)
		{
			using EndianStackReader reader = new(data);
			return ReadBlocks(reader, address);
		}

		/// <summary>
		/// Read a flat list of NJ blocks from a reader.
		/// </summary>
		public static IReadOnlyList<NJBlockInfo> ReadBlocks(EndianStackReader reader, uint address)
		{
			List<NJBlockInfo> result = [];

			reader.PushBigEndian(reader.CheckBigEndian32(address + 4));
			uint blockAddress = address;

			while(blockAddress < reader.Length + 8)
			{
				reader.PushBigEndian(false);
				uint blockHeader = reader.ReadUInt(blockAddress);
				reader.PopEndian();

				uint blockSize = reader.ReadUInt(blockAddress + 4);
				if(blockHeader == 0 || blockSize == 0)
				{
					break;
				}

				result.Add(new(
					blockAddress,
					blockHeader,
					blockSize,
					GetRole(blockHeader)));

				blockAddress += 8 + blockSize;
			}

			reader.PopEndian();
			return result;
		}

		private static string GetRole(uint header)
		{
			if(FileHeaders.ModelBlockHeaders.Contains(header))
			{
				return "model";
			}

			if(FileHeaders.TextureListBlockHeaders.Contains(header))
			{
				return "texture";
			}

			if(FileHeaders.AnimationBlockHeaders.Contains(header))
			{
				return "animation";
			}

			return "none";
		}
	}

	/// <summary>
	/// NJ block information.
	/// </summary>
	public readonly record struct NJBlockInfo(uint Offset, uint Header, uint Size, string Role);
}
