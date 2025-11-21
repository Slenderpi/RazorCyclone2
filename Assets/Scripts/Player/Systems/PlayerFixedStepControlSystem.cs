using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Apply inputs that need to be read at a fixed rate.
/// It is necessary to handle this as part of the fixed step group, in case your framerate is lower than the fixed step rate.
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
[BurstCompile]
partial struct PlayerFixedStepControlSystem : ISystem {

	[BurstCompile]
	public void OnCreate(ref SystemState state) {
		state.RequireForUpdate<FixedTickSystem.Singleton>();
		state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerComponent, PlayerInputsComponent>().Build());
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state) {
		uint tick = SystemAPI.GetSingleton<FixedTickSystem.Singleton>().Tick;

		foreach (var (inputs, player) in SystemAPI.Query<PlayerInputsComponent, PlayerComponent>().WithAll<Simulate>()) {
			if (SystemAPI.HasComponent<PlayerCharacterControl>(player.ControlledCharacter)) {
				PlayerCharacterControl characterControl = SystemAPI.GetComponent<PlayerCharacterControl>(player.ControlledCharacter);

				quaternion characterRotation = SystemAPI.GetComponent<LocalTransform>(player.ControlledCharacter).Rotation;

				// Weapon info
				characterControl.UpdateVacuumInput(inputs.VacuumState.IsDown(tick), inputs.VacuumState.IsUp(tick));
				characterControl.UpdateCannonInput(inputs.CannonState.IsDown(tick), inputs.CannonState.IsUp(tick));

				// Orientation info
				// Only set charCtrl orientation if new one is different && non-zero
				if (!Util.IsNearZero(inputs.OrientationInput) && !Util.IsNearEqual(characterControl.OrientationInput, inputs.OrientationInput)) {
					characterControl.OrientationInput = inputs.OrientationInput;
					characterControl.DidOrientationInputChange = true;
				} else {
					characterControl.DidOrientationInputChange = false;
				}

				// Acceleration vector
				//PlayerCharacterComponent charComp = SystemAPI.GetComponent<PlayerCharacterComponent>(player.ControlledCharacter);
				//quaternion camrot = charComp.ViewLocalRotation;
				//characterControl.AccelVector = math.mul(camrot, characterControl.OrientationInput);
				//Util.D_DrawArrowCenteredAt(SystemAPI.GetComponent<LocalToWorld>(player.ControlledCharacter).Position, characterControl.AccelVector, 5, Color.magenta, SystemAPI.Time.DeltaTime, true);
				//float3 characterForward = MathUtilities.GetForwardFromRotation(characterRotation);
				//float3 characterRight = MathUtilities.GetRightFromRotation(characterRotation);
				//float3 characterUp = MathUtilities.GetUpFromRotation(characterRotation);
				//if (!Util.IsNearZero(characterControl.OrientationInput))
				//	characterControl.AccelVector = math.normalizesafe(
				//		characterControl.OrientationInput.z * characterForward
				//		+ characterControl.OrientationInput.x * characterRight
				//		+ characterControl.OrientationInput.y * characterUp
				//	);
				//else
				//	characterControl.AccelVector = float3.zero;


				//if (playerInputs.IsVacuumDown || playerInputs.IsCannonDown) {
				//    //float3 orientationNorm = MathUtilities.ClampToMaxLength(characterControl.OrientationInput, 1f);
				//    float force = 0f;
				//    if (playerInputs.IsVacuumDown)
				//        force = 5f;
				//    if (playerInputs.IsCannonDown)
				//        force += 10f;
				//    //characterControl.AccelVector = orientationNorm.z * characterForward + orientationNorm.x * characterRight + orientationNorm.y * characterUp;
				//    characterControl.AccelVector = MathUtilities.ClampToMaxLength(
				//        characterControl.OrientationInput.z * characterForward
				//        + characterControl.OrientationInput.x * characterRight
				//        + characterControl.OrientationInput.y * characterUp,
				//        force
				//    );

				//    //characterControl.AccelVector = (playerInputs.OrientationInput.y * characterForward) + (playerInputs.OrientationInput.x * characterRight);
				//    //characterControl.AccelVector = MathUtilities.ClampToMaxLength(characterControl.AccelVector, 1f);

				//    characterControl.Vacuum = playerInputs.VacuumPressed.IsSet(tick);

				//    //// Jump
				//    //characterControl.Jump = playerInputs.JumpPressed.IsSet(tick);

				//}

				SystemAPI.SetComponent(player.ControlledCharacter, characterControl);
			}
		}
	}

}
