namespace GameLibrary
{
	public struct InputState : IStepState
	{
		public bool WasInitialized { get; set; }
		public int Step { get; set; }

		public bool IsLeftPressed;
		public bool IsRightPressed;
		public bool IsJumpPressed;
	}
}
