using System;

namespace GameLibrary
{
	public class ClientInstance
	{
		public readonly PhysicsPlayer Player;
		public readonly RingStepBuffer<InputState> InputStates = new RingStepBuffer<InputState>(Settings.SavedStatesCount);
		public InputState Input { get; set; }

		private readonly Action<ClientInstance, InputState> onInput;

		public ClientInstance(PhysicsPlayer player, Action <ClientInstance, InputState> onInput)
		{
			Player = player;
			Player.Owner = this;
			this.onInput = onInput;
		}

		public void SendInput(InputState state)
		{
			onInput(this, state);
		}
	}
}
