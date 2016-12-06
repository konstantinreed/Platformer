using System.Collections.Generic;
using Yuzu;

namespace GameLibrary.Source.SerializableFormats
{
	public class LevelFormat
	{
		[YuzuMember]
		public List<PlatformFormat> StaticPlatforms = new List<PlatformFormat>();

		[YuzuMember]
		public VectorFormat SpawnPosition;
	}
}
