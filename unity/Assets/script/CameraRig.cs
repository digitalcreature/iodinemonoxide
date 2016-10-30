using UnityEngine;

public class CameraRig : SingletonBehaviour<CameraRig> {

	public float focusSmoothing = 10;
	public float focusMargin = 5;
	public float minFocusRadius = 3;
	public Gradient backgroundColors;

	Vector3 focusTarget;
	float distanceTarget;

	public Color backgroundColor {
		get {
			return cam.backgroundColor;
		}
	}

	private Vector3 initialPosition;
	private Vector3 initalCameraPosition;

	Camera cam;

	void Awake() {
		cam = GetComponentInChildren<Camera>();
		initialPosition = transform.position;
		initalCameraPosition = cam.transform.localPosition;
	}

	public void Initialize() {
		cam = GetComponentInChildren<Camera>();
		cam.backgroundColor = backgroundColors.Evaluate(Random.value);
		transform.position = initialPosition;
		cam.transform.localPosition = initalCameraPosition;
		focusTarget = transform.position;
		distanceTarget = cam.transform.localPosition.z;
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
		float focusRadius = Mathf.Max(molecule.boundingRadius + focusMargin, minFocusRadius);
		distanceTarget = -(focusRadius
			 / Mathf.Sin(Mathf.Deg2Rad * cam.fieldOfView / 2));
		focusTarget = molecule.center;
	}

}
