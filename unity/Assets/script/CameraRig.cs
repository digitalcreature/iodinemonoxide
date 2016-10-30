using UnityEngine;

public class CameraRig : SingletonBehaviour<CameraRig> {

	public float focusSmoothing = 10;
	public float focusMargin = 5;
	public float distanceFactor = 1.5f;
	Vector2 look;

	Vector3 focusTarget;
	float distanceTarget;

	Camera cam;

	void Awake() {
		cam = GetComponentInChildren<Camera>();
		focusTarget = transform.position;
	}

	void Update() {
		Vector3 pos = transform.position;
		pos = Vector3.Lerp(pos, focusTarget, Time.deltaTime * focusSmoothing);
		transform.position = pos;
		Vector3 camPos = cam.transform.localPosition;
		camPos.z = Mathf.Lerp(camPos.z, distanceTarget, Time.deltaTime * focusSmoothing);
		cam.transform.localPosition = camPos;
	}

	public void Refocus() {
		MoleculeManager molecule = MoleculeManager.instance;
		distanceTarget = -((molecule.boundingRadius + focusMargin)
			 / Mathf.Tan(Mathf.Deg2Rad * cam.fieldOfView)) * distanceFactor;
		focusTarget = molecule.center;
	}

}
