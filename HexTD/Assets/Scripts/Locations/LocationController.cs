using UnityEngine;
using Zenject;

namespace Locations
{
	public class LocationController : MonoBehaviour, ILocationController
	{
		[Inject]
		public void Construct()
		{
		}
	}
}