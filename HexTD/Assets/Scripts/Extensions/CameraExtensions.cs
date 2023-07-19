using System.Linq;
using UnityEngine;

namespace Extensions
{
	public static class CameraExtensions
	{
		public static bool TryGetBounds(this GameObject obj, out Bounds bounds)
		{
			var renderers = obj.GetComponentsInChildren<Renderer>();
			return renderers.TryGetBounds(out bounds);
		}

		public static bool TryGetBounds(this GameObject[] gameObjects, out Bounds bounds)
		{
			var renderers = gameObjects.Where(g => g).SelectMany(g => g.GetComponentsInChildren<Renderer>()).ToArray();
			return renderers.TryGetBounds(out bounds);
		}

		public static bool TryGetBounds(this Renderer[] renderers, out Bounds bounds)
		{
			bounds = default;

			if (renderers.Length == 0)
			{
				return false;
			}

			bounds = renderers[0].bounds;

			for (var i = 1; i < renderers.Length; i++)
			{
				bounds.Encapsulate(renderers[i].bounds);
			}

			return true;
		}

		// Facto how far away the camera should be 
		private const float cameraDistance = 0.38f;

		public static bool TryGetFocusTransforms(this Camera camera, GameObject targetGameObject,
			out Vector3 targetPosition, out Quaternion targetRotation)
		{
			targetPosition = default;
			targetRotation = default;
			
			if (!targetGameObject.TryGetBounds(out var bounds))
			{
				return false;
			}

			TryGetFocusTransforms(camera, bounds, out targetPosition, out targetRotation);
			return true;
		}

		public static bool TryGetFocusTransforms(this Camera camera, Bounds bounds,
			out Vector3 targetPosition, out Quaternion targetRotation)
		{
			var objectSizes = bounds.max - bounds.min;
			var objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
			// Visible height 1 meter in front
			var cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
			// Combined wanted distance from the object
			var distance = cameraDistance * objectSize / cameraView;
			// Estimated offset from the center to the outside of the object
//			distance += 0.5f * objectSize;
			targetPosition = bounds.center - distance * camera.transform.forward;

			targetRotation = Quaternion.LookRotation(bounds.center - targetPosition);

			return true;
		}

//		public static void FocusOn(Camera camera, GameObject object)
//		{
//			
//		}

		public static void FocusOn(this Camera camera, Bounds bounds)
		{
			
		}
		
		public static void FocusOn2(this Camera camera, Bounds bounds, float marginPercentage)
		{
			Vector3 centerAtFront = new(bounds.center.x, bounds.max.y, bounds.center.z);
			Vector3 centerAtFrontTop = new(bounds.center.x, bounds.max.y, bounds.max.z);
			float centerToTopDist = (centerAtFrontTop - centerAtFront).magnitude;
			float minDistanceY = centerToTopDist * marginPercentage / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
 
			Vector3 centerAtFrontRight = new(bounds.max.x, bounds.center.y, bounds.max.z);
			float centerToRightDist = (centerAtFrontRight - centerAtFront).magnitude;
			float minDistanceX = centerToRightDist * marginPercentage / Mathf.Tan(camera.fieldOfView * camera.aspect * Mathf.Deg2Rad);
 
			float minDistance = Mathf.Max(minDistanceX, minDistanceY);
 
			camera.transform.position = new Vector3(bounds.center.x, bounds.center.y + minDistance, bounds.center.z);
			camera.transform.LookAt(bounds.center);
		}
		
		public static void SetPosition(this Camera camera, Bounds bounds)
		{
			float cameraDistance = 0.5f; // Constant factor
			Vector3 objectSizes = bounds.max - bounds.min;
			float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
			float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView); // Visible height 1 meter in front
			float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
			distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
			camera.transform.position = bounds.center - distance * camera.transform.forward;
		}
		

		public static bool TryGetFocusTransforms(this Camera camera, GameObject[] targetGameObjects,
			out Vector3 targetPosition, out Quaternion targetRotation)
		{
			targetPosition = default;
			targetRotation = default;

			if (!targetGameObjects.TryGetBounds(out var bounds))
			{
				return false;
			}

			var objectSizes = bounds.max - bounds.min;
			var objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
			var cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
			var distance = cameraDistance * objectSize / cameraView;
			distance += 0.5f * objectSize;
			targetPosition = bounds.center - distance * camera.transform.forward;

			targetRotation = Quaternion.LookRotation(bounds.center - targetPosition);

			return true;
		}
	}
}