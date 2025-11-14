using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Handle movement and collision resolution in this FixedUpdate system.<br/>
/// Data for this is stored in the PlayerCharacterControl component.
/// </summary>
[UpdateInGroup(typeof(KinematicCharacterPhysicsUpdateGroup))]
[BurstCompile]
partial struct PlayerCharacterPhysicsUpdateSystem : ISystem {
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            PlayerCharacterComponent,
            PlayerCharacterControl
        >().Build());
		state.RequireForUpdate<PhysicsWorldSingleton>();
	}
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
		foreach (var (
			charComponent,
			characterControl,
			trans,
			pv,
			pm
			) in SystemAPI.Query<
			RefRW<PlayerCharacterComponent>,
			RefRO<PlayerCharacterControl>,
			RefRW<LocalTransform>,
			RefRW<PhysicsVelocity>,
			RefRW<PhysicsMass>
			>().WithAll<Simulate>()) {
			pm.ValueRW.InverseInertia = float3.zero;

			// TODO: Figure out a way to do this scalably with roguelike effects that can change weapon behaviors
			// TODO: Activate a Vacuum component for collision, vfx, sound, etc. to read and act on
			if (characterControl.ValueRO.IsVacuumActive()) {
				pv.ValueRW.Linear = characterControl.ValueRO.AccelVector * charComponent.ValueRO.VacuumMoveForce + pv.ValueRO.Linear;
			}
			//if (characterControl.ValueRO.IsVacuumDownThisFrame()) {
			//	Debug.Log($"Vacuum down | {SystemAPI.Time.ElapsedTime}");
			//}
			//if (characterControl.ValueRO.IsVacuumUpThisFrame()) {
			//	Debug.Log($"Vacuum up | {SystemAPI.Time.ElapsedTime}");
			//}
			//if (characterControl.ValueRO.IsCannonDownThisFrame()) {
			//	Debug.Log($"Cannon down | {SystemAPI.Time.ElapsedTime}");
			//}
			//if (characterControl.ValueRO.IsCannonUpThisFrame()) {
			//	Debug.Log($"Cannon up | {SystemAPI.Time.ElapsedTime}");
			//}
		}

	}
    
}
