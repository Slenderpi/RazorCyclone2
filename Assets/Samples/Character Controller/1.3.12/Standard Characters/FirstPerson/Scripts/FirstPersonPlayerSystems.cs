using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using Unity.CharacterController;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
public partial class FirstPersonPlayerInputsSystem : SystemBase {

    protected override void OnCreate()
    {
        RequireForUpdate<FixedTickSystem.Singleton>();
        RequireForUpdate(SystemAPI.QueryBuilder().WithAll<FirstPersonPlayer, FirstPersonPlayerInputs>().Build());
    }

    protected override void OnUpdate()
    {
        uint tick = SystemAPI.GetSingleton<FixedTickSystem.Singleton>().Tick;

#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        Mouse ms = Mouse.current;
        foreach (var (playerInputs, player) in SystemAPI.Query<RefRW<FirstPersonPlayerInputs>, FirstPersonPlayer>())
        {
            playerInputs.ValueRW.OrientationInput = new float3
            {
                x = (kb.dKey.isPressed ? 1f : 0f) + (kb.aKey.isPressed ? -1f : 0f),
                y = (kb.spaceKey.isPressed ? 1f : 0f) + (kb.shiftKey.isPressed ? -1f : 0f),
                z = (kb.wKey.isPressed ? 1f : 0f) + (kb.sKey.isPressed ? -1f : 0f)
			};

            playerInputs.ValueRW.LookInput = ms.delta.ReadValue() * player.LookInputSensitivity;

            //if (Keyboard.current.spaceKey.wasPressedThisFrame)
            //{
            //    playerInputs.ValueRW.JumpPressed.Set(tick);
            //}
            playerInputs.ValueRW.IsVacuumDown = ms.leftButton.isPressed;
            playerInputs.ValueRW.IsCannonDown = ms.rightButton.isPressed;
        }
#endif
    }
}

/// <summary>
/// Apply inputs that need to be read at a variable rate
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct FirstPersonPlayerVariableStepControlSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<FirstPersonPlayer, FirstPersonPlayerInputs>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerInputs, player) in SystemAPI.Query<FirstPersonPlayerInputs, FirstPersonPlayer>().WithAll<Simulate>())
        {
            if (SystemAPI.HasComponent<FirstPersonCharacterControl>(player.ControlledCharacter))
            {
                FirstPersonCharacterControl characterControl = SystemAPI.GetComponent<FirstPersonCharacterControl>(player.ControlledCharacter);

                characterControl.LookDegreesDelta = playerInputs.LookInput;

                SystemAPI.SetComponent(player.ControlledCharacter, characterControl);
            }
        }
    }
}

/// <summary>
/// Apply inputs that need to be read at a fixed rate.
/// It is necessary to handle this as part of the fixed step group, in case your framerate is lower than the fixed step rate.
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
[BurstCompile]
public partial struct FirstPersonPlayerFixedStepControlSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FixedTickSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<FirstPersonPlayer, FirstPersonPlayerInputs>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        uint tick = SystemAPI.GetSingleton<FixedTickSystem.Singleton>().Tick;

        foreach (var (playerInputs, player) in SystemAPI.Query<FirstPersonPlayerInputs, FirstPersonPlayer>().WithAll<Simulate>())
        {
            if (SystemAPI.HasComponent<FirstPersonCharacterControl>(player.ControlledCharacter))
            {
                FirstPersonCharacterControl characterControl = SystemAPI.GetComponent<FirstPersonCharacterControl>(player.ControlledCharacter);

                quaternion characterRotation = SystemAPI.GetComponent<LocalTransform>(player.ControlledCharacter).Rotation;

                // Move
                float3 characterForward = MathUtilities.GetForwardFromRotation(characterRotation);
                float3 characterRight = MathUtilities.GetRightFromRotation(characterRotation);
                float3 characterUp = MathUtilities.GetUpFromRotation(characterRotation);
				//Debug.Log($"{playerInputs.OrientationInput} | Vacuum: {playerInputs.IsVacuumDown} | Cannon: {playerInputs.IsCannonDown}");
				if (!Util.IsNearZero(playerInputs.OrientationInput) && !Util.IsNearEqual(characterControl.OrientationInput, playerInputs.OrientationInput))
                    characterControl.OrientationInput = playerInputs.OrientationInput;

                if (!Util.IsNearZero(characterControl.OrientationInput))
                    characterControl.AccelVector = math.normalizesafe(
                        characterControl.OrientationInput.z * characterForward
                        + characterControl.OrientationInput.x * characterRight
                        + characterControl.OrientationInput.y * characterUp
                    );

				// Weapons
				characterControl.Vacuum = playerInputs.IsVacuumDown;
				characterControl.Cannon = playerInputs.IsCannonDown;
				
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