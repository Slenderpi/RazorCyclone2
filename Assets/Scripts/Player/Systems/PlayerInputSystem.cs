using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
public partial class PlayerInputSystem : SystemBase {

	protected override void OnCreate() {
		RequireForUpdate<FixedTickSystem.Singleton>();
		RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerComponent, PlayerInputsComponent>().Build());
	}

	protected override void OnUpdate() {
		uint tick = SystemAPI.GetSingleton<FixedTickSystem.Singleton>().Tick;
		PlayerInputActions.GameplayActions ga = InputHandler.InputActions.Gameplay;
		float3 orientInput = new() {
			x = ga.OrientX.ReadValue<float>(),
			y = ga.OrientY.ReadValue<float>(),
			z = ga.OrientZ.ReadValue<float>()
		};
		bool vacPressed = ga.Vacuum.WasPressedThisFrame();
		bool vacReleased = ga.Vacuum.WasReleasedThisFrame();
		bool canPressed = ga.Cannon.WasPressedThisFrame();
		bool canReleased = ga.Cannon.WasReleasedThisFrame();
		float2 mdelta = (float2)ga.MouseDelta.ReadValue<Vector2>();

		foreach (var (inputs, player) in SystemAPI.Query<RefRW<PlayerInputsComponent>, PlayerComponent>()) {
			inputs.ValueRW.OrientationInput = orientInput;

			if (vacPressed)
				inputs.ValueRW.VacuumState.Down(tick);
			else if (vacReleased)
				inputs.ValueRW.VacuumState.Up(tick);
			if (canPressed)
				inputs.ValueRW.CannonState.Down(tick);
			else if (canReleased)
				inputs.ValueRW.CannonState.Up(tick);

			inputs.ValueRW.LookInput = mdelta * player.LookInputSensitivity;
		}
	}

}

[UpdateInGroup(typeof(PresentationSystemGroup), OrderLast = true)]
public partial struct PlayerCharacterControlInputResetSystem : ISystem {

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<PlayerCharacterControl>();
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		foreach (var pcc in SystemAPI.Query<RefRW<PlayerCharacterControl>>()) {
			pcc.ValueRW.ResetEvents();
		}
	}

}