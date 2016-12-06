using System.Collections.Generic;
using Yuzu;

namespace GameLibrary.Source.SerializableFormats
{
	public class PlatformFormat
	{
		[YuzuMember]
		public VectorFormat Position;

		[YuzuMember]
		public List<VectorFormat> Vertices = new List<VectorFormat>();
	}
}
