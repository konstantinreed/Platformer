namespace GameLibrary
{
	internal static class Settings
	{
		public const double Step = 0.05d;
		public const double SimulationStep = 0.025d;
		public const double StepsPerSimulations = SimulationStep / Step;
		public const float SimulationStepF = 0.025f;
		public const int SavedStatesCount = 10;
		public const int PlayersCount = 1; // TODO: Rework
	}
}
