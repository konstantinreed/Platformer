using System;
using UnityEngine;

namespace PlatformerToolkit
{
	public class PlatformMaterial : ScriptableObject
	{
		public Material EdgeMaterial;
		public Material FillMaterial;

		public Rect LeftEdge;
		public Rect RightEdge;
		public Rect TopEdge;
		public Rect BottomEdge;

		public Rect LeftTopCorner;
		public Rect LeftBottomCorner;
		public Rect RightTopCorner;
		public Rect RightBottomCorner;
		public Rect TopLeftCorner;
		public Rect TopRightCorner;
		public Rect BottomLeftCorner;
		public Rect BottomRightCorner;

		public Rect GetEdge(Side side)
		{
			switch (side) {
				case Side.Left:
					return LeftEdge;
				case Side.Right:
					return RightEdge;
				case Side.Top:
					return TopEdge;
				case Side.Bottom:
					return BottomEdge;
				default:
					throw new ArgumentException();
			}
		}

		public Rect GetCorner(Side sideFrom, Side sideTo)
		{
			switch (sideFrom) {
				case Side.Left:
					if (sideTo == Side.Top) {
						return LeftTopCorner;
					} else if (sideTo == Side.Bottom) {
						return LeftBottomCorner;
					}
					break;
				case Side.Right:
					if (sideTo == Side.Top) {
						return RightTopCorner;
					} else if (sideTo == Side.Bottom) {
						return RightBottomCorner;
					}
					break;
				case Side.Top:
					if (sideTo == Side.Left) {
						return TopLeftCorner;
					} else if (sideTo == Side.Right) {
						return TopRightCorner;
					}
					break;
				case Side.Bottom:
					if (sideTo == Side.Left) {
						return BottomLeftCorner;
					} else if (sideTo == Side.Right) {
						return BottomRightCorner;
					}
					break;
			}
			throw new ArgumentException();
		}
	}
}