using System.Collections.Generic;
using Yuzu;

namespace GameLibrary
{
	public class Platform
	{
		[YuzuMember]
		public Vector Position;

		[YuzuMember]
		public List<Vector> Vertices = new List<Vector>();
	}
}
