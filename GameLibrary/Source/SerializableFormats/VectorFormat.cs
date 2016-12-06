using Microsoft.Xna.Framework;
using Yuzu;

namespace GameLibrary.Source.SerializableFormats
{
	public struct VectorFormat
	{
		[YuzuMember]
		public float X;

		[YuzuMember]
		public float Y;

		public Vector2 Vector2 => new Vector2(X, Y);

		public VectorFormat(float x, float y)
		{
			X = x;
			Y = y;
		}
	}
}
