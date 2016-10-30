using UnityEngine;

public class CameraRig : SingletonBehaviour<CameraRig> {

	Vector2 look;

	void Awake() {
	}

	void SetLookFromRotation() {
		Vector3 euler = transform.eulerAngles;
		look.x = euler.y;
		look.y = -euler.x;
	}

	void SetRotationFromLook() {
		look.y = Mathf.Clamp(look.y, -90, 90);
		transform.rotation = Quaternion.Euler(-look.y, look.x, 0);
	}

	public void Orbit(Vector3 start, Vector3 end) {
		Quaternion rot = Quaternion.FromToRotation(start, end);
		transform.rotation *= rot;
	}

}
