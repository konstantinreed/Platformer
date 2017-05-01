using UnityEngine;

namespace PlatformerToolkit
{
	public static class MathUtility
	{
		public static Vector2 Orthogonal(Vector2 v)
		{
			return new Vector2(-v.y, v.x);
		}
	}
}
