using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour {

    public GameObject ControlledCharacter;
	public float LookInputSensitivity = 30f;



	class Baker : Baker<PlayerAuthoring> {
        public override void Bake(PlayerAuthoring auth) {
			Entity entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new PlayerComponent {
				ControlledCharacter = GetEntity(auth.ControlledCharacter, TransformUsageFlags.Dynamic),
				LookInputSensitivity = auth.LookInputSensitivity,
			});
			AddComponent<PlayerInputsComponent>(entity);
		}
    }
    
}