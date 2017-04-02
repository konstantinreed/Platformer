namespace GameLibrary
{
	internal interface IStepState
	{
		int Step { get; set; }

		void Reset();
	}
}
