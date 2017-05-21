using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerToolkit
{
	public class Platform : MonoBehaviour
	{
		public List<PathNode> Path = new List<PathNode>();
		public bool Closed;
		public bool CorrectSharpCorners = true;
		public int HSubdivs = 1;
		public int VSubdivs = 1;
		public PlatformMaterial Material;

		public class PathNode
		{
			public Vector2 Pos;
			public float Scale = 1.0f;
		}
	}
}