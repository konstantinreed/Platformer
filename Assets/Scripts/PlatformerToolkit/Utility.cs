using UnityEngine;

namespace PlatformerToolkit
{
	public static class Utility
	{
		public static float Cross(Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.y - v1.y * v2.x;
		}

		public static Vector2 Orthogonal(Vector2 v)
		{
			return new Vector2(-v.y, v.x);
		}

		public static Vector2 Miter(Vector2 dir1, Vector2 dir2)
		{
			var miter = Orthogonal(dir1 + dir2) * 0.5f;
			miter /= miter.sqrMagnitude;
			return miter;
		}
	}
}
