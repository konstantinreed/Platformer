using System.Collections.Generic;
using GameLibrary;

namespace Scripts
{
	public class TestLevel02 : SampleLevel
	{
		public override Level GetLevel()
		{
			return new Level() {
				SpawnPosition = new Vector(-1.16f, 1.2f),
				StaticPlatforms = new List<Platform>() {
					new Platform() {
						Position = new Vector(-2.07f, -0.5f),
						Vertices = new List<Vector>() {
							new Vector(-3.339119f, -0.992022f),
							new Vector(3.339119f, -0.992022f),
							new Vector(3.339119f, 0.992022f),
							new Vector(-3.339119f, 0.992022f),
						}
					},
					new Platform() {
						Position = new Vector(13.52f, -0.16f),
						Vertices = new List<Vector>() {
							new Vector(-5.714245f, -1.385062f),
							new Vector(5.714245f, -1.385062f),
							new Vector(5.714245f, 1.385062f),
							new Vector(-5.714245f, 1.385062f),
						}
					},
					new Platform() {
						Position = new Vector(15.53f, 2.63f),
						Vertices = new List<Vector>() {
							new Vector(-3.743466f, -1.49596f),
							new Vector(3.743466f, -1.49596f),
							new Vector(3.743466f, 1.49596f),
							new Vector(-3.743466f, 1.49596f),
						}
					},
				}
			};
		}
	}
}
