namespace GameLibrary
{
	internal static class Settings
	{
		public const double Step = 0.04d;
		public const double SimulationStep = 0.02d;
		public const double StepsPerSimulations = SimulationStep / Step;
		public const float SimulationStepF = 0.02f;
		public const int SavedStatesCount = 12;
		public const int PlayersCount = 1; // TODO: Rework
	}
}
