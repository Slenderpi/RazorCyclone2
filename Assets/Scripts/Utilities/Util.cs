using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public static class Util {

	public readonly static Unity.Mathematics.Random rng = new(1);

	public static readonly float NEAR_ZERO = 1.0e-10f;



	//public static float3 StepWanderPoint2D(float3 prevWanderPoint, float lastWanderStepTime, float wanderMinimumDelay, float wanderLimitRadius, float wanderChangeDist) {
	//	return BoidSteerer.StepWanderPoint2D(prevWanderPoint, wanderLimitRadius, wanderChangeDist);
	//}

	public static uint GenerateSeed(Transform transform) {
		// uh um random weird code go
		float3 pos = transform.position;
		float val = (
			pos.x +
			pos.y * -pos.y +
			pos.z * pos.z * pos.z
		);
		if (val < 0) val = 1 + -val * 3;
		if (val < 1) {
			float3 rot = math.EulerXYZ(transform.rotation);
			val = 1 + (
				47 * (math.abs(rot.x) + math.abs(pos.y) + 1) +
				11 * (math.abs(rot.y) + math.abs(pos.z) + 1) +
				71 * (math.abs(rot.z) + math.abs(pos.z) + 1
			));
		}
		return (uint)val;
	}

	/// <summary>
	/// A value of 0 is cold, a value of 1 is hot.<br/>
	/// The heatmap is in 5 colors. In cold to hot:<br/>
	/// 0-------0.25-----0.5--------0.75------1<br/>
	/// Blue -> Cyan -> Green -> Yellow -> Red
	/// </summary>
	/// <param name="perc"></param>
	/// <returns></returns>
	public static Color MakeHeatmapColor(float heat) {
		if (heat < 0.25f)
			return Color.Lerp(Color.blue, Color.cyan, heat / 0.25f);
		else if (heat < 0.5f)
			return Color.Lerp(Color.cyan, Color.green, (heat - 0.25f) / 0.25f);
		else if (heat < 0.75f)
			return Color.Lerp(Color.green, Color.yellow, (heat - 0.5f) / 0.25f);
		else
			return Color.Lerp(Color.yellow, Color.red, (heat - 0.75f) / 0.25f);
	}

	/// <summary>
	/// Returns an up vector for use in:
	/// <code>quaternion.LookRotation(lookVector, UpForLookRotation(lookVector)).</code>
	/// </summary>
	/// <param name="lookVector">The look vector you plan to use for quaternion.LookRotation().</param>
	/// <returns>Normally, math.up().<br/>
	/// However, if lookVector is too close to that and not close to a full zero vector,
	/// returns math.back() (or math.forward() if lookVector.y is negative) instead.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void UpForLookRotation(in float3 lookVector, out float3 upVector) {
		if (IsNearUpOnly(lookVector))
			upVector = math.back() * math.sign(lookVector.y);
		else
			upVector = math.up();
	}

	/// <summary>
	/// Determines if a vector is an up vector (i.e. x and z are near 0 and y is not).
	/// </summary>
	/// <param name="v">Vector to test.</param>
	/// <returns>True if the vector is nearly an up vector.<br/>
	/// However, a near-zero vector does not count as an up vector.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNearUpOnly(in float3 v) {
		return math.abs(v.z) < NEAR_ZERO &&
			math.abs(v.x) < NEAR_ZERO &&
			math.abs(v.y) > NEAR_ZERO;
	}

	/// <summary>
	/// Tests if a vector's square length is near 0 (1.0e-10f).
	/// </summary>
	/// <param name="v">Vector to test.</param>
	/// <returns>True if near zero.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNearZero(in float3 v) {
		return math.lengthsq(v) <= NEAR_ZERO;
	}

	/// <summary>
	/// Tests if a vector's square length is near 0 (1.0e-10f).
	/// </summary>
	/// <param name="v">Vector to test.</param>
	/// <returns>True if near zero.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNearZero(in float2 v) {
		return math.lengthsq(v) <= NEAR_ZERO;
	}

	/// <summary>
	/// Tests if all components of the vector are exactly equal to 0.
	/// </summary>
	/// <param name="v">Vector to test.</param>
	/// <returns>True if all components are exactly 0.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsZero(in float3 v) {
		return v.x == 0 && v.y == 0 && v.z == 0;
	}

	/// <summary>
	/// Tests if all components of the vector are exactly equal to 0.
	/// </summary>
	/// <param name="v">Vector to test.</param>
	/// <returns>True if all components are exactly 0.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsZero(in float2 v) {
		return v.x == 0 && v.y == 0;
	}

	/// <summary>
	/// Tests if two vectors are nearly equal.
	/// The exact math is lengthsq(v1 - v2) &lt;= -1.0e-10f
	/// </summary>
	/// <param name="v1">First operand.</param>
	/// <param name="v2">Second operand.</param>
	/// <returns>True if near equal.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNearEqual(in float3 v1, in float3 v2) {
		return IsNearZero(v1 - v2);
	}

	/// <summary>
	/// Tests if two vectors are nearly equal.<br/>
	/// The exact math is lengthsq(v1 - v2) &lt;= -1.0e-10f
	/// </summary>
	/// <param name="v1">First operand.</param>
	/// <param name="v2">Second operand.</param>
	/// <returns>True if near equal.</returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNearEqual(in float2 v1, in float2 v2) {
		return IsNearZero(v1 - v2);
	}

	/// <summary>
	/// Converts float3 of euler degrees into radians. Additionally,
	/// the pitch value (x) is negated.
	/// </summary>
	/// <param name="eulers"></param>
	/// <returns></returns>
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DegToRadiansNegPitch(in float3 eulers, out float3 converted) {
		converted = new() {
			x = -math.radians(eulers.x), // Negate pitch
			y = math.radians(eulers.y),
			z = math.radians(eulers.z)
		};
	}



	///////////////////////////////////////////////////// DEBUGGING

	[BurstCompile]
	public static void D_DrawPoint(in float3 position, in Color c) {
		D_DrawPoint(position, c, 9999999f);
	}

	[BurstCompile]
	public static void D_DrawPoint(in float3 position, in Color c, float t) {
		D_DrawPoint(position, c, t, 0.15f, false);
	}

	[BurstCompile]
	public static void D_DrawPoint(in float3 position, in Color c, float t, float radius, bool depthTest) {
		Debug.DrawRay(position + math.forward() * radius, 2 * radius * math.back(), c, t, depthTest);
		Debug.DrawRay(position + math.right() * radius, 2 * radius * math.left(), c, t, depthTest);
	}

	[BurstCompile]
	public static void D_DrawBox(in float3 centerPosition, in float3 size, in Color c) {
		D_DrawBox(centerPosition, size, c, 9999999f);
	}

	[BurstCompile]
	public static void D_DrawBox(in float3 centerPosition, in float3 size, in Color c, float t) {
		D_DrawBox(centerPosition, size, c, t, true);
	}

	[BurstCompile]
	public static void D_DrawBox(in float3 centerPosition, in float3 size, in Color c, float t, bool depthTest) {
		float3 topRightFront = centerPosition + size / 2f;
		float3 topLeftFront = topRightFront + math.left() * size.x;
		float3 topRightBack = topRightFront + math.back() * size.z;
		float3 topLeftBack = topLeftFront + math.back() * size.z;
		float3 botRightFront = topRightFront + math.down() * size.y;
		float3 botLeftFront = topLeftFront + math.down() * size.y;
		float3 botRightBack = topRightBack + math.down() * size.y;
		float3 botLeftBack = topLeftBack + math.down() * size.y;

		/* Top rect */
		Debug.DrawLine(topRightFront, topLeftFront, c, t, depthTest); // top front
		Debug.DrawLine(topRightFront, topRightBack, c, t, depthTest); // top right
		Debug.DrawLine(topLeftFront, topLeftBack, c, t, depthTest); // top left
		Debug.DrawLine(topRightBack, topLeftBack, c, t, depthTest); // top back

		/* Bottom rect */
		Debug.DrawLine(botRightFront, botLeftFront, c, t, depthTest); // bot front
		Debug.DrawLine(botRightFront, botRightBack, c, t, depthTest); // bot right
		Debug.DrawLine(botLeftFront, botLeftBack, c, t, depthTest); // bot left
		Debug.DrawLine(botRightBack, botLeftBack, c, t, depthTest); // bot back

		/* 4 Poles */
		Debug.DrawLine(topRightFront, botRightFront, c, t, depthTest); // front right
		Debug.DrawLine(topLeftFront, botLeftFront, c, t, depthTest); // front left
		Debug.DrawLine(topRightBack, botRightBack, c, t, depthTest); // back right
		Debug.DrawLine(topLeftBack, botLeftBack, c, t, depthTest); // back left
	}

	[BurstCompile]
	public static void D_DrawArrowCenteredAt(in float3 position, in float3 direction, float length, in Color c) {
		D_DrawArrowCenteredAt(position, direction, length, c, 9999999f);
	}

	[BurstCompile]
	public static void D_DrawArrowCenteredAt(in float3 position, in float3 direction, float length, in Color c, float t) {
		D_DrawArrowCenteredAt(position, direction, length, c, t, true);
	}

	[BurstCompile]
	public static void D_DrawArrowCenteredAt(in float3 position, in float3 direction, float length, in Color c, float t, bool depthTest) {
		if (math.lengthsq(direction) == 0) {
			D_DrawPoint(position, Color.white, t, length, depthTest);
			return;
		}
		float3 normalizedDir = math.normalize(direction);
		float3 lengthenedDir = normalizedDir * length;

		float3 crosser = math.up();
		if (direction.x == 0 && direction.y != 0 && direction.z == 0)
			crosser = math.forward();
		float3 norm = math.normalize(math.cross(normalizedDir, crosser)) * length * 0.3f;
		float3 tipPosition = position + lengthenedDir * 0.5f;
		float3 alongPosition = position + lengthenedDir * 0.5f * 0.3f;
		float3 rightTipPosition = alongPosition + norm;
		float3 leftTipPosition = alongPosition - norm;

		Debug.DrawLine(position - lengthenedDir * 0.5f, tipPosition, c, t, depthTest);
		Debug.DrawLine(tipPosition, rightTipPosition, c, t, depthTest);
		Debug.DrawLine(tipPosition, leftTipPosition, c, t, depthTest);
	}

	//[BurstCompile]
	//public static void D_VisualizePointCloud(in NativeArray<bool> pointCloud, in PointCloudConfig pcc) {
	//	float3 pointOffset = new(pcc.DistBetweenPoints / 2f);
	//	float3 pointBoxSize = new(pcc.PointRadius * 2f);
	//	int YZ = pcc.numY * pcc.numZ;
	//	float3 size = new float3(pcc.numX, pcc.numY, pcc.numZ) * pcc.DistBetweenPoints;
	//	D_DrawBox(pcc.cornerPosition + size / 2f, size, Color.cyan);
	//	for (int x = 0; x < pcc.numX; x++)
	//		for (int y = 0; y < pcc.numY; y++)
	//			for (int z = 0; z < pcc.numZ; z++) {
	//				bool isVisible = pointCloud[x * YZ + y * pcc.numZ + z];
	//				//if (isVisible)
	//				//	continue;
	//				float3 pos = pcc.cornerPosition + new float3(x, y, z) * pcc.DistBetweenPoints + pointOffset;
	//				D_DrawPoint(
	//					pos,
	//					isVisible ? Color.green : Color.red,
	//					9999999f,
	//					0.15f,
	//					true
	//				);
	//				if (!isVisible)
	//					D_DrawBox(pos, pointBoxSize, Color.red);
	//			}
	//}

}