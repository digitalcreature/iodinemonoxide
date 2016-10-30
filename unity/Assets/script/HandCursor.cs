using UnityEngine;
using Leap;

public class HandCursor : MonoBehaviour {

	public Gradient pinchColor;
	public float pinchThreshold = 0.5f;
	public AnimationCurve pinchScale;
	public Transform model;

	public Leap.Hand hand { get; private set; }

	public Atom grabbedAtom { get; private set; }
	public Vector3 grabOffset { get; private set; }

	public bool wasPinching { get; private set; }
	public bool isPinching { get; private set; }
	public Vector3 lastPosition { get; private set;}
	public Vector3 position { get; private set; }
	public Vector3 delta { get; private set; }

	public Vector3 worldPosition {
		get {
			return transform.parent.TransformPoint(this.position);
		}
	}

	public Vector3 lastWorldPosition {
		get {
			return transform.parent.TransformPoint(this.lastPosition);
		}
	}

	private Renderer render;
	private Rigidbody body;
	private static HandCursor dragging; // the cursoor currently executing a drag, if any

	void Awake() {
		render = model.GetComponent<Renderer>();
		body = gameObject.AddComponent<Rigidbody>();
		body.isKinematic = true;
		body.interpolation = RigidbodyInterpolation.Interpolate;
	}

	public void OnFrame(Leap.Hand hand) {
		FingerList fingers = hand.Fingers;
		Vector3 pos = Vector3.zero;
		int fingerCount = 0;
		foreach (Finger finger in fingers) {
			if ((int) finger.Type < 2) {
				fingerCount ++;
				Vector3 tip = finger.TipPosition.Vec3() / 10;
				pos += tip;
				// for (int i = 0; i < 4; i ++) {
				// 	Bone bone = finger.Bone((Bone.BoneType) i);
				// 	Vector3 a = bone.PrevJoint.Vec3() / 10;
				// 	Vector3 b = bone.NextJoint.Vec3() / 10;
				// 	a = transform.parent.TransformPoint(a);
				// 	b = transform.parent.TransformPoint(b);
				// 	Debug.DrawLine(a, b);
				// }
			}
		}
		pos /= fingerCount;
		float pinch = hand.PinchStrength;

		this.hand = hand;
		this.wasPinching = this.isPinching;
		this.lastPosition = this.position;
		this.position = pos;
		this.delta = delta;
		this.isPinching = pinch > pinchThreshold;
		body.position = this.worldPosition;
		Material mat = render.material;
		mat.color = pinchColor.Evaluate(pinch);
		render.material = mat;
		model.localScale = Vector3.one * pinchScale.Evaluate(pinch);
	}

	void Update() {
		if (grabbedAtom != null) {
			grabbedAtom.transform.position = transform.position + grabOffset;
		}
	}

	public void Grab(Atom atom, Vector3 offset) {
		Release();
		atom.OnGrab(this);
		grabbedAtom = atom;
		grabOffset = offset;
		if (grabbedAtom != null) {
			render.enabled = false;
		}
	}
	public void Grab(Atom atom) { Grab(atom, Vector3.zero); }

	public void Release() {
		if (grabbedAtom != null) {
			grabbedAtom.OnRelease(this);
			grabbedAtom = null;

		}
		render.enabled = true;
	}

}
