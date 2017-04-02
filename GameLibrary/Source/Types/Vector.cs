using Microsoft.Xna.Framework;
using Yuzu;

namespace GameLibrary
{
	public struct Vector
	{
		[YuzuMember]
		public float X;

		[YuzuMember]
		public float Y;

		public Vector2 Vector2 => new Vector2(X, Y);

		public Vector(float x, float y)
		{
			X = x;
			Y = y;
		}
	}
}
