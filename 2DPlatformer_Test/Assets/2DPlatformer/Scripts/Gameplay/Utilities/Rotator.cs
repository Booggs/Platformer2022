namespace GSGD2.Gameplay
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Simple component that can be used to rotate an object on itself.
	/// It can be setup to reset rotation at OnEnable or OnDisable
	/// </summary>
	public class Rotator : MonoBehaviour
	{
		//[SerializeField]
		//private Vector3 _rotationForces = Vector3.up;

		[SerializeField]
		private Space _space = 0;

		[SerializeField]
		private bool _resetWorldRotationAtOnEnable = true;

		[SerializeField]
		private bool _resetWorldRotationAtOnDisable = true;

		[SerializeField]
		private SlingshotHandler _slingshotHandler = null;

		private float _angleLimit = 45f;
		private Vector3 lastValidRotation = new Vector3(360, 0, 0);
		private Quaternion _worldCachedRotationAtStart = Quaternion.identity;

		public void Play()
		{
			enabled = true;
		}

		public void Stop()
		{
			enabled = false;
		}

		private void Awake()
		{
			_worldCachedRotationAtStart = transform.rotation;
			_angleLimit = _slingshotHandler.AngleLimit;
		}

		private void OnEnable()
		{
			if (_resetWorldRotationAtOnEnable == true)
			{
				transform.rotation = _worldCachedRotationAtStart;
			}
		}

		private void OnDisable()
		{
			if (_resetWorldRotationAtOnDisable == true)
			{
				transform.rotation = _worldCachedRotationAtStart;
			}
		}

		private void Update()
		{
			/*Vector3 currentRotation = transform.localRotation.eulerAngles;
			print("last valid rotation : " + lastValidRotation + " | current rotation : " + currentRotation);
			if (currentRotation.x < 360 - _angleLimit)
            {
				currentRotation = lastValidRotation;
            }
			else if (currentRotation.x < -_angleLimit && currentRotation.x > -90)
            {
				currentRotation.x = Mathf.Clamp(currentRotation.x, -_angleLimit, 0);
				lastValidRotation = currentRotation;
            }
			else if (currentRotation.x > -180 + _angleLimit && currentRotation.x < -90)
            {
				currentRotation.x = Mathf.Clamp(currentRotation.x, -180, -180 + _angleLimit);
				lastValidRotation = currentRotation;
			}
			else lastValidRotation = currentRotation;
			transform.localRotation = Quaternion.Euler(currentRotation);*/
			//transform.Rotate(_rotationForces * Time.deltaTime, _space);
		}
	}
}