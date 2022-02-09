using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject _rotator = null;

    [SerializeField]
    private float _rotationSpeed = 5f;

    private void Update()
    {
        _rotator.transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
    }
}
