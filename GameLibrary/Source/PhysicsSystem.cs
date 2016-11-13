using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	/*
	Сервер:
		Физика просчитывается на сервере.
		Просчитывается благодаря потоку ввода с клиентов.
		Время измеряется в шагах симуляции.
		Мир изменяется при поступлении неизвестного ранее инпута, не старше N шага симуляции.
		Необходимо сохранять состояние и инпут динамических объектов, не старше N шага симуляции.
	Клиент:
		Физика просчитывается на клиенте постоянно, просчитывается своим потоком инпута.
		Время измеряется в шагах симуляции.
		Мир изменяется при поступлении неизвестного ранее или изменённого состояния с инпутом, не старше N шага симуляции.
		Необходимо сохранять состояние и инпут динамических объектов, не старше N шага симуляции.
	*/

	public class PhysicsSystem
	{
		private readonly Vector2 gravityForce = new Vector2(0f, -9.82f);

		public World World { get; private set; }
		public readonly List<PhysicsBody> Objects = new List<PhysicsBody>();

		public PhysicsSystem()
		{
			World = new World(gravityForce);
		}

		public void FixedUpdate()
		{
			foreach (var physicsObject in Objects) {
				physicsObject.FixedUpdate(Settings.SimulationStepF);
			}

			World.Step(Settings.SimulationStepF);
		}
	}
}
