using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

[Serializable]
public struct PlayerComponent : IComponentData {
	public Entity ControlledCharacter;
	[FormerlySerializedAs("LookRotationSpeed")] public float LookInputSensitivity;
}

[Serializable]
public struct PlayerInputsComponent : IComponentData {
	public float3 OrientationInput;
	public FixedInputState VacuumState;
	public FixedInputState CannonState;

	public float2 LookInput;
}
