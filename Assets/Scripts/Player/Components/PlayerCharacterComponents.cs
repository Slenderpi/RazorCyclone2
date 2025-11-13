using System;
using Unity.Entities;
using Unity.Mathematics;



[Serializable]
public struct PlayerCharacterComponent : IComponentData {
	public float VacuumMoveForce;
	public float CannonMoveForce;

	public float PivotRotationSpeed;

	public Entity PivotEntity;

	public Entity ViewEntity;

	public quaternion PivotGoalRotation;

	/// <summary>
	/// The euler rotation the camera should currently have.
	/// </summary>
	public float3 CurrCamRotationDeg;

	public quaternion ViewLocalRotation;
}

[Serializable]
public struct PlayerCharacterControl : IComponentData {
	public float3 AccelVector;
	/// <summary>
	/// Orientation input. Not normalized.
	/// </summary>
	public float3 OrientationInput;
	public bool DidOrientationInputChange;
	public float2 LookDegreesDelta;

	bool VacuumDown;
	bool VacuumUp;
	bool VacuumHeld;
	bool CannonDown;
	bool CannonUp;
	bool CannonHeld;

	/// <summary>
	/// Determines if the Vacuum key is currently held down.
	/// </summary>
	/// <returns>True if held down this frame.</returns>
	public bool IsVacuumActive() {
		return VacuumHeld;
	}

	/// <summary>
	/// Determines if the Vacuum key was pressed down this frame.
	/// </summary>
	/// <returns>True if down this frame.</returns>
	public bool IsVacuumDownThisFrame() {
		return VacuumDown;
	}

	/// <summary>
	/// Determines if the Vacuum key was released this frame.
	/// </summary>
	/// <returns>True if released this frame.</returns>
	public bool IsVacuumUpThisFrame() {
		return VacuumUp;
	}

	/// <summary>
	/// Determines if the Cannon key is currently held down.
	/// </summary>
	/// <returns>True if held down this frame.</returns>
	public bool IsCannonActive() {
		return CannonHeld;
	}

	/// <summary>
	/// Determines if the Cannon key was pressed down this frame.
	/// </summary>
	/// <returns>True if down this frame.</returns>
	public bool IsCannonDownThisFrame() {
		return CannonDown;
	}

	/// <summary>
	/// Determines if the Cannon key was released this frame.
	/// </summary>
	/// <returns>True if released this frame.</returns>
	public bool IsCannonUpThisFrame() {
		return CannonUp;
	}

	/// <summary>
	/// Updates internal Vacuum event variables.<br/>
	/// Nothing will be set for either of the following cases:<br/>
	/// - down is true but the Vacuum is already held down<br/>
	/// - up is true but the Vacuum is not held down<br/>
	/// The parameters are considered within an if-else. This means that if
	/// both are true, only one parameter will be considered. For game feel,
	/// down is prioritized over up.
	/// </summary>
	/// <param name="downThisFrame">Set as true if the Vacuum key was down this frame.</param>
	/// <param name="upThisFrame">Set as true if the Vacuum key was up this frame.</param>
	public void UpdateVacuumInput(bool downThisFrame, bool upThisFrame) {
		if (downThisFrame) {
			if (!VacuumHeld) {
				VacuumDown = true;
				VacuumHeld = true;
			}
		} else if (upThisFrame) {
			if (VacuumHeld) {
				VacuumUp = true;
				VacuumHeld = false;
			}
		}
	}

	/// <summary>
	/// Updates internal Cannon event variables.<br/>
	/// Nothing will be set for either of the following cases:<br/>
	/// - down is true but the Cannon is already held down<br/>
	/// - up is true but the Cannon is not held down<br/>
	/// The parameters are considered within an if-else. This means that if
	/// both are true, only one parameter will be considered. For game feel,
	/// down is prioritized over up.
	/// </summary>
	/// <param name="downThisFrame">Set as true if the Cannon key was down this frame.</param>
	/// <param name="upThisFrame">Set as true if the Cannon key was up this frame.</param>
	public void UpdateCannonInput(bool downThisFrame, bool upThisFrame) {
		if (downThisFrame) {
			if (!CannonHeld) {
				CannonDown = true;
				CannonHeld = true;
			}
		} else if (upThisFrame) {
			if (CannonHeld) {
				CannonUp = true;
				CannonHeld = false;
			}
		}
	}

	/// <summary>
	/// Resets up/down key events.
	/// </summary>
	public void ResetEvents() {
		VacuumDown = false;
		VacuumUp = false;
		CannonDown = false;
		CannonUp = false;
	}
}

[Serializable]
public struct PlayerCharacterView : IComponentData {
	public Entity CharacterEntity;
}