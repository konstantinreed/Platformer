using GameLibrary.Source.SerializableFormats;
using UnityEngine;

namespace Scripts
{
	public abstract class SampleLevel : MonoBehaviour
	{
		public abstract LevelFormat GetLevel();
	}
}
