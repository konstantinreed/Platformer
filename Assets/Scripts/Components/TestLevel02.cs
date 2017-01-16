using System.Collections.Generic;
using GameLibrary.Source.SerializableFormats;

namespace Scripts
{
	public class TestLevel02 : SampleLevel
	{
		public override LevelFormat GetLevel()
		{
			return new LevelFormat() {
				SpawnPosition = new VectorFormat(-1.16f, 1.2f),
				StaticPlatforms = new List<PlatformFormat>() {
					new PlatformFormat() {
						Position = new VectorFormat(-2.07f, -0.5f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-3.339119f, -0.992022f),
							new VectorFormat(3.339119f, -0.992022f),
							new VectorFormat(3.339119f, 0.992022f),
							new VectorFormat(-3.339119f, 0.992022f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(13.52f, -0.16f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-5.714245f, -1.385062f),
							new VectorFormat(5.714245f, -1.385062f),
							new VectorFormat(5.714245f, 1.385062f),
							new VectorFormat(-5.714245f, 1.385062f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(15.53f, 2.63f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-3.743466f, -1.49596f),
							new VectorFormat(3.743466f, -1.49596f),
							new VectorFormat(3.743466f, 1.49596f),
							new VectorFormat(-3.743466f, 1.49596f),
						}
					},
				}
			};
		}
	}
}
