using System;

namespace GameLibrary
{
	public class ClientManager
	{
		public readonly ClientInstance[] Clients;

		public ClientManager(GameApplication application, Action<ClientInstance, InputState> onInput, int instancesCount)
		{
			Clients = new ClientInstance[instancesCount];
			for (var i = 0; i < instancesCount; i++) {
				var player = application.Level.SpawnPlayer();
				Clients[i] = new ClientInstance(player, onInput);
			}
		}
	}
}
