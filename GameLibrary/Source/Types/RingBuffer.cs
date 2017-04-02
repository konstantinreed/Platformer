namespace GameLibrary
{
	internal class RingStepBuffer<T> where T : class, IStepState, new()
	{
		internal struct RewindResult
		{
			public bool WasRewinded;
			public int Steps;
		}

		private readonly T[] buffer;

		public int CurrentStep { get; private set; }
		public T this[int step] =>
			step < 0 || step <= CurrentStep - buffer.Length || step > CurrentStep ?
			null :
			buffer[step % buffer.Length];

		public RingStepBuffer(int capacity)
		{
			buffer = new T[capacity];
			for (var i = 0; i < capacity; i++) {
				buffer[i] = new T();
			}
		}

		public void RewindForward(int step)
		{
			if (step < 0 || step <= CurrentStep) {
				return;
			}

			for (var i = CurrentStep + 1; i <= step; i++) {
				var index = i % buffer.Length;
				buffer[index].Reset();
				buffer[index].Step = i;
			}
			CurrentStep = step;
		}

		public RewindResult RewindToStep(T state)
		{
			var maxStep = CurrentStep;
			RewindForward(state.Step);
			if (state.Step < 0 || state.Step <= CurrentStep - buffer.Length) {
				return new RewindResult();
			}

			var index = state.Step % buffer.Length;
			buffer[index] = state;
			return new RewindResult() {
				WasRewinded = true,
				Steps = state.Step - maxStep
			};
		}
	}
}
