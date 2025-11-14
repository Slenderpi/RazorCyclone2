using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;

[DisallowMultipleComponent]
class PlayerCharacterAuthoring : MonoBehaviour {

    public GameObject PivotEntity;
    public GameObject ViewEntity;
    
    public float VacuumMoveForce = 3f;
    public float CannonMoveForce = 10f;

    public float PivotRotationSpeed = 25f;




	class Baker : Baker<PlayerCharacterAuthoring> {
        public override void Bake(PlayerCharacterAuthoring auth) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.WorldSpace);
            AddComponent(entity, new PlayerCharacterComponent() {
                VacuumMoveForce = auth.VacuumMoveForce,
                CannonMoveForce = auth.CannonMoveForce,

                PivotRotationSpeed = auth.PivotRotationSpeed,


				PivotEntity = GetEntity(auth.PivotEntity, TransformUsageFlags.Dynamic),

                ViewEntity = GetEntity(auth.ViewEntity, TransformUsageFlags.Dynamic),

                PivotGoalRotation = quaternion.identity
            });
            AddComponent(entity, new PlayerCharacterControl() {
                AccelVector = math.forward(),
                OrientationInput = math.forward()
            });
        }
    }
    
}