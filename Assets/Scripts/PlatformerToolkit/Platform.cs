using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerToolkit
{
	public class Platform : MonoBehaviour
	{
		public List<PathNode> Path = new List<PathNode>();
		public bool Closed;
		public float Thickness = 1.0f;
		public bool CorrectSharpCorners = true;
		public PlatformMaterial Material;

		public class PathNode
		{
			public Vector2 Pos;
			public float Scale = 1.0f;
		}
	}
}