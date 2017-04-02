using System.Collections.Generic;
using GameLibrary;

namespace Scripts
{
	public class TestLevel01 : SampleLevel
	{
		public override Level GetLevel()
		{
			return new Level() {
				SpawnPosition = new Vector(3f, 4.5f),
				StaticPlatforms = new List<Platform>() {
					new Platform() {
						Position = new Vector(-5.5f, 1f),
						Vertices = new List<Vector>() {
							new Vector(-5f, -0.5f),
							new Vector(5f, -0.5f),
							new Vector(5f, 0.5f),
							new Vector(-5f, 0.5f),
						}
					},
					new Platform() {
						Position = new Vector(26f, -1f),
						Vertices = new List<Vector>() {
							new Vector(-25f, -0.5f),
							new Vector(25f, -0.5f),
							new Vector(25f, 0.5f),
							new Vector(-25f, 0.5f),
						}
					},
					new Platform() {
						Position = new Vector(12f, 4f),
						Vertices = new List<Vector>() {
							new Vector(-10f, -0.5f),
							new Vector(10f, -0.5f),
							new Vector(10f, 0.5f),
							new Vector(-10f, 0.5f),
						}
					},
					new Platform() {
						Position = new Vector(39f, 4f),
						Vertices = new List<Vector>() {
							new Vector(-10f, -0.5f),
							new Vector(10f, -0.5f),
							new Vector(10f, 0.5f),
							new Vector(-10f, 0.5f),
						}
					},
					new Platform() {
						Position = new Vector(-10f, 9.25f),
						Vertices = new List<Vector>() {
							new Vector(-0.5f, -7.75f),
							new Vector(0.5f, -7.75f),
							new Vector(0.5f, 7.75f),
							new Vector(-0.5f, 7.75f),
						}
					},
					new Platform() {
						Position = new Vector(50.5f, 4.5f),
						Vertices = new List<Vector>() {
							new Vector(-0.5f, -5f),
							new Vector(0.5f, -5f),
							new Vector(0.5f, 5f),
							new Vector(-0.5f, 5f),
						}
					},
					new Platform() {
						Position = new Vector(-6f, 6.5f),
						Vertices = new List<Vector>() {
							new Vector(-0.5f, -1f),
							new Vector(0.5f, -1f),
							new Vector(0.5f, 1f),
							new Vector(-0.5f, 1f),
						}
					},
					new Platform() {
						Position = new Vector(1.5f, 16.5f),
						Vertices = new List<Vector>() {
							new Vector(-11f, -0.5f),
							new Vector(11f, -0.5f),
							new Vector(11f, 0.5f),
							new Vector(-11f, 0.5f),
						}
					},
					new Platform() {
						Position = new Vector(10.5f, 11f),
						Vertices = new List<Vector>() {
							new Vector(-2f, -0.5f),
							new Vector(2f, -0.5f),
							new Vector(2f, 0.5f),
							new Vector(-2f, 0.5f),
						}
					},
					new Platform() {
						Position = new Vector(13f, 11.75f),
						Vertices = new List<Vector>() {
							new Vector(-0.5f, -5.25f),
							new Vector(0.5f, -5.25f),
							new Vector(0.5f, 5.25f),
							new Vector(-0.5f, 5.25f),
						}
					},
					new Platform() {
						Position = new Vector(2f, 11f),
						Vertices = new List<Vector>() {
							new Vector(-3.5f, -0.5f),
							new Vector(3.5f, -0.5f),
							new Vector(3.5f, 0.5f),
							new Vector(-3.5f, 0.5f),
						}
					},
					new Platform() {
						Position = new Vector(0.383884f, -0.090991f),
						Vertices = new List<Vector>() {
							new Vector(0.883883f, -1.59099f),
							new Vector(1.59099f, -0.883883f),
							new Vector(-0.883883f, 1.59099f),
							new Vector(-1.59099f, 0.883883f),
						}
					},
					new Platform() {
						Position = new Vector(-3.61f, 8.67f),
						Vertices = new List<Vector>() {
							new Vector(-2.828427f, -2.12132f),
							new Vector(-2.12132f, -2.828427f),
							new Vector(2.828427f, 2.12132f),
							new Vector(2.12132f, 2.828427f),
						}
					},
				}
			};
		}
	}
}
