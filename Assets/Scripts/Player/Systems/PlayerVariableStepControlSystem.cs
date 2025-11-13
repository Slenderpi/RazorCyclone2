using Unity.Burst;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Update CharacterControl with inputs that are to be consumed in
/// Update() timing. E.g.: mouse delta.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerVariableStepControlSystem : ISystem {
	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerComponent, PlayerInputsComponent>().Build());
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		foreach (var (inputs, player) in SystemAPI.Query<PlayerInputsComponent, PlayerComponent>().WithAll<Simulate>()) {
			if (SystemAPI.HasComponent<PlayerCharacterControl>(player.ControlledCharacter)) {
				PlayerCharacterControl characterControl = SystemAPI.GetComponent<PlayerCharacterControl>(player.ControlledCharacter);

				characterControl.LookDegreesDelta = inputs.LookInput;

				SystemAPI.SetComponent(player.ControlledCharacter, characterControl);
			}
		}
	}
}