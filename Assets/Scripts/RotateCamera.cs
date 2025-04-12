using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCamera : MonoBehaviour
{
	[SerializeField] private float _sensitivity = 0.1f;

	private void Update()
	{
		var val = Mouse.current.delta.ReadValue();
		val = new(-val.y, val.x);
		transform.Rotate(val * _sensitivity);
		var rotation = transform.localEulerAngles;
		rotation.z = 0f;
		transform.rotation = Quaternion.Euler(rotation);
	}
}
