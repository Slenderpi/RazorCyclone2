using System;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// MonoBehaviour for reading input with the Input System.
/// Singleton.
/// </summary>
public class InputHandler : MonoBehaviour {

	public static InputHandler Instance;

	/// <summary>
	/// The PlayerInputActions object that input is read from.
	/// </summary>
	public static PlayerInputActions InputActions { get => Instance._pia; private set => Instance._pia = value; }
	PlayerInputActions _pia;

	/// <summary>
	/// Represents the currently-enabled input action maps.
	/// </summary>
	public static InputMap EnabledInputMaps { get => Instance._eim; private set => Instance._eim = value; }
	InputMap _eim;



	private void Awake() {
		Instance = this;
		InputActions = new();
		_eim = InputMap.None;
	}

	private void Start() {
		EnableInputActions(InputMap.Gameplay);
	}

	/// <summary>
	/// Enable input action map(s).
	/// Attempting to re-enable already-enabled input action maps will do nothing.
	/// </summary>
	/// <param name="map"></param>
	public static void EnableInputActions(InputMap map) {
		if (map == InputMap.None) {
			Debug.LogError("ERROR: Attempt to set EnableInputActions() with value InputMap.None. This is not allowed. Input can be disabled with function DisableInputActions()");
			return;
		}
		Instance.SetInputActionsEnabled(map, true);
		EnabledInputMaps |= map;
	}

	/// <summary>
	/// Disable input action map(s).
	/// Attempting to re-disable already-disabled input action maps will do nothing.
	/// </summary>
	/// <param name="map"></param>
	public static void DisableInputActions(InputMap map) {
		if (map == InputMap.None) {
			Debug.LogError("ERROR: Attempt to set DisableInputActions() with value InputMap.None. This is not allowed.");
			return;
		}
		Instance.SetInputActionsEnabled(map, false);
		EnabledInputMaps &= ~map;
	}

	private void SetInputActionsEnabled(InputMap map, bool newEnabled) {
		int i = (int)map;
		while (i != 0) {
			InputMap m = (InputMap)(i & -i); // Isolates right-most flag
			i &= i - 1; // Removes right-most flag
			
			// Validation
			if ((EnabledInputMaps & m) == m) {
				if (newEnabled) // Check if trying to enable
					continue; // Map already enabled, skip it
			} else if (!newEnabled) // At this point, the map is not enabled, so check if we're trying to disable
				continue; // Map already disabled, skip it

			switch (m) {
				case InputMap.Gameplay:
					SetEnabledGameplay(newEnabled);
					break;
			}
		}
	}

	private void SetEnabledGameplay(bool newEnabled) {
		if (newEnabled) {
			InputActions.Enable();
		} else {
			InputActions.Disable();
		}
	}

}

[Flags]
public enum InputMap {
	None = 0,

	Gameplay = 1,		// 0001

	All = Gameplay
}