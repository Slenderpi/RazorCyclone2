using System;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Handle inputs given by CharacterControl that are meant to be applied
/// in Update() timing. E.g.: mouse look rotation.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PlayerVariableStepControlSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
[BurstCompile]
partial struct PlayerCharacterVariableUpdateSystem : ISystem {
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<FixedTickSystem.Singleton>();
		state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            PlayerCharacterComponent,
            PlayerCharacterControl,
            PhysicsVelocity,
            PhysicsMass
        >().Build());
	}
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ComponentLookup<PlayerCharacterComponent> characterLookup = SystemAPI.GetComponentLookup<PlayerCharacterComponent>(true);

        foreach (var (
            charComponent,
            characterControl
            ) in SystemAPI.Query<
            RefRW<PlayerCharacterComponent>,
            RefRW<PlayerCharacterControl>
            >().WithAll<Simulate>()) {
			HandleCameraRotation(ref state, charComponent, characterControl, out quaternion camrot);
			UpdatePivotGoalRotation(charComponent, characterControl.ValueRO, camrot);
			SlerpPivotRotation(ref state, charComponent.ValueRO);
		}
	}

	[BurstCompile]
#pragma warning disable IDE0251 // Make member 'readonly'
#pragma warning disable IDE0060 // Remove unused parameter
	private bool HandleCameraRotation(ref SystemState state, RefRW<PlayerCharacterComponent> charComponent, RefRW<PlayerCharacterControl> characterControl, out quaternion camrot) {
		float3 camrotEuler = charComponent.ValueRO.CurrCamRotationDeg;
		float3 converted;
		// Only update camera rotation and related values if the mouse has actually moved
		if (!Util.IsZero(characterControl.ValueRO.LookDegreesDelta)) {
			// Compute and clamp pitch
			float newPitchDegValue = charComponent.ValueRO.CurrCamRotationDeg.x + characterControl.ValueRO.LookDegreesDelta.y;
			if (newPitchDegValue > 90f)
				newPitchDegValue = 90f;
			else if (newPitchDegValue < -90f)
				newPitchDegValue = -90f;

			// Save new cam rotation values
			camrotEuler.y += characterControl.ValueRO.LookDegreesDelta.x; // Yaw
			camrotEuler.x = newPitchDegValue; // Pitch
			charComponent.ValueRW.CurrCamRotationDeg = camrotEuler;
			Util.DegToRadiansNegPitch(camrotEuler, out converted);
			camrot = quaternion.Euler(converted);

			// Apply camrot to AccelVector
			characterControl.ValueRW.AccelVector = math.mul(camrot, characterControl.ValueRO.OrientationInput);

			// Apply rotation to camera
			LocalTransform camtrans = SystemAPI.GetComponent<LocalTransform>(charComponent.ValueRO.ViewEntity);
			camtrans.Rotation = camrot;
			SystemAPI.SetComponent(charComponent.ValueRW.ViewEntity, camtrans);
			return true;
		}
		Util.DegToRadiansNegPitch(camrotEuler, out converted);
		camrot = quaternion.Euler(converted);
		return false;
	}
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0251 // Make member 'readonly'

	private readonly void UpdatePivotGoalRotation(RefRW<PlayerCharacterComponent> charComponent, PlayerCharacterControl characterControl, quaternion camrot) {
		float3 lookVector = characterControl.OrientationInput;
		Util.UpForLookRotation(lookVector, out float3 up);
		charComponent.ValueRW.PivotGoalRotation = quaternion.LookRotation(
			math.mul(camrot, lookVector),
			math.mul(camrot, up)
		);
	}

	[BurstCompile]
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0251 // Make member 'readonly'
	private void SlerpPivotRotation(ref SystemState state, in PlayerCharacterComponent charComponent) {
		LocalTransform pivtrans = SystemAPI.GetComponent<LocalTransform>(charComponent.PivotEntity);
		pivtrans.Rotation = math.slerp(
			pivtrans.Rotation,
			charComponent.PivotGoalRotation,
			math.min(SystemAPI.Time.DeltaTime * charComponent.PivotRotationSpeed, 1)
		);
		SystemAPI.SetComponent(charComponent.PivotEntity, pivtrans);
	}
#pragma warning restore IDE0251 // Make member 'readonly'
#pragma warning restore IDE0060 // Remove unused parameter
}
