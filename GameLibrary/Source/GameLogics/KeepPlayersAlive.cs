namespace GameLibrary
{
	internal class KeepPlayersAlive : GameLogic
	{
		private int? playersDeadStep;

		public override void Updated()
		{
			var isPlayersDead = true;
			foreach (var player in The.Application.Players) {
				if (!player.PhysicsPlayer.State.IsDead) {
					isPlayersDead = false;
					break;
				}
			}

			if (isPlayersDead && !playersDeadStep.HasValue) {
				playersDeadStep = The.Application.CurrentStep;
            } else if (!isPlayersDead && playersDeadStep.HasValue) {
				playersDeadStep = null;
            }

			if (playersDeadStep.HasValue && The.Application.CurrentStep > playersDeadStep.Value + Settings.PlayersRevivalStep) {
				foreach (var player in The.Application.Players) {
					player.PhysicsPlayer.SetPosition(The.Application.Physics.Level.SpawnPosition.Vector2);
                }
			}
        }
	}
}
