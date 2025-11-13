using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

[Serializable]
public struct FirstPersonPlayer : IComponentData
{
    public Entity ControlledCharacter;
    [FormerlySerializedAs("LookRotationSpeed")] public float LookInputSensitivity;
}

[Serializable]
public struct FirstPersonPlayerInputs : IComponentData
{
    public float3 OrientationInput;

    public FixedInputState VacuumState;
    public FixedInputState CannonState;

	public bool IsVacuumDown;
    public bool IsCannonDown;

    public float2 LookInput;
    public FixedInputEvent JumpPressed;
}