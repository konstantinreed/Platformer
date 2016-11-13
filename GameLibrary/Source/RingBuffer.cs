namespace GameLibrary
{
	public class RingStepBuffer<T> where T : struct, IStepState
	{
		public struct RewindResult
		{
			public bool WasRewinded;
			public int Steps;
		}

		private readonly T[] buffer;

		public int CurrentStep { get; private set; }
		public T this[int step]
		{
			get
			{
				return
					step < 0 || step <= CurrentStep - buffer.Length || step > CurrentStep ?
					default(T) :
					buffer[step % buffer.Length];
			}
		}

		public RingStepBuffer(int capacity)
		{
			buffer = new T[capacity];
		}

		public void RewindForward(int step)
		{
			if (step < 0 || step <= CurrentStep) {
				return;
			}
			
			for (var i = CurrentStep + 1; i < step; i++) {
				var index = i % buffer.Length;
				buffer[index] = default(T);
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
			if (state.WasInitialized) {
				return new RewindResult();
			}

			buffer[index] = state;
			buffer[index].WasInitialized = true;
			return new RewindResult() {
				WasRewinded = true,
				Steps = state.Step - maxStep
			};
		}
	}
}
