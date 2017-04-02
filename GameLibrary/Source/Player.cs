namespace GameLibrary
{
	public class Player
	{
		private readonly InputState lastInputState = new InputState();

		internal readonly PhysicsPlayer PhysicsPlayer;
		internal readonly RingStepBuffer<InputState> InputStates = new RingStepBuffer<InputState>(Settings.SavedStatesCount);
		internal InputState Input { get; private set; }
		internal bool InputModified { get; set; }

		internal Player(PhysicsPlayer physicsPlayer)
		{
			PhysicsPlayer = physicsPlayer;
			PhysicsPlayer.Owner = this;
			Input = InputStates[0];
		}

		public PlayerState GetState()
		{
			return PhysicsPlayer.GetState(The.Application.CurrentStepProgress);
		}

		public void SetInput(InputState state)
		{
			if (Input.IsDiffersFrom(state)) {
				Input.Merge(state);
				Input.SetPreviousInput(InputStates[InputStates.CurrentStep - 1]);
				lastInputState.Copy(Input);
				InputModified = true;
            } else {
				lastInputState.Copy(state);
				lastInputState.SetPreviousInput(Input);
			}
        }

		internal void RewindForward()
		{
			InputStates.RewindForward(InputStates.CurrentStep + 1);
			Input = InputStates[InputStates.CurrentStep];
			Input.Merge(lastInputState);
			InputModified = false;
		}
	}
}
