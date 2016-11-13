namespace GameLibrary
{
	public interface IStepState
	{
		bool WasInitialized { get; set; }
		int Step { get; set; }
	}
}
