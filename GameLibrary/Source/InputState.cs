namespace GameLibrary
{
	public struct InputState : IStepState
	{
		private const float JumpPressedDurability = 1;

		public bool WasInitialized { get; set; }
		public int Step { get; set; }

		public bool IsLeftPressed;
		public bool IsRightPressed;
		public bool IsJumpPressed;
		public int JumpPressedStep;

		public bool IsJumpJustPressed => IsJumpPressed && JumpPressedStep >= Step - JumpPressedDurability;
	}
}
