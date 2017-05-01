using System.Collections.Generic;
using UnityEngine;

namespace PlatformerToolkit
{
	public class Platform : MonoBehaviour
	{
		public List<Vector2> Path = new List<Vector2>();
		public bool Closed;
	}
}