using UnityEngine;

/// <summary>
/// WASD/Arrows for movement
/// Q to ascend, E to Descend
/// Shift moves faster, Ctrl moves slower
/// L toggles cursor locking to screen
/// </summary>
public class FreeFlyCamera : MonoBehaviour
{
	public float cameraSensitivity = 90.0f;
	
	public float normalMoveSpeed = 5.0f;
	public float verticalNormalSpeed = 4.0f;

	public float slowMoveFactor = 0.5f;
	public float fastMoveFactor = 2.0f;

	private float rotationX = 0.0f;
	private float rotationY = 0.0f;
	private bool ready = false;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		
	}

	private void Update()
	{
        if (ready)
        {
			UpdateRotation();
			UpdatePosition();

			if (Input.GetKeyDown(KeyCode.L))
			{
				Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Locked : CursorLockMode.None;
			}
		}
		
	}

	private void UpdateRotation()
	{
		rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
		rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
		rotationY = Mathf.Clamp(rotationY, -90.0f, 90.0f);

		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
	}

	private void UpdatePosition()
	{
		var moveSpeed = normalMoveSpeed;
		var verticalSpeed = verticalNormalSpeed;

		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			moveSpeed *= fastMoveFactor;
			verticalSpeed *= fastMoveFactor;
		}
		else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
		{
			moveSpeed *= slowMoveFactor;
			verticalSpeed *= slowMoveFactor;
		}
		transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
		transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;

		if (Input.GetKey(KeyCode.Q))
		{
			transform.position += transform.up * verticalSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.E))
		{
			transform.position -= transform.up * verticalSpeed * Time.deltaTime;
		}
	}
}