using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;

class PlayerCharacterViewAuthoringBaker : MonoBehaviour {

	public GameObject Character;



	class Baker : Baker<PlayerCharacterViewAuthoringBaker> {
		public override void Bake(PlayerCharacterViewAuthoringBaker auth) {
			if (auth.transform.parent != auth.Character.transform) {
				UnityEngine.Debug.LogError("ERROR: the Character View must be a direct 1st-level child of the character authoring GameObject. Conversion will be aborted");
				return;
			}
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new PlayerCharacterView {
				CharacterEntity = GetEntity(auth.Character, TransformUsageFlags.Dynamic)
			});
		}
	}

}