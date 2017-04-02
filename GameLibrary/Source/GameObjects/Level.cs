using System.Collections.Generic;
using Yuzu;

namespace GameLibrary
{
	public class Level
	{
		[YuzuMember]
		public List<Platform> StaticPlatforms = new List<Platform>();

		[YuzuMember]
		public Vector SpawnPosition;
	}
}
