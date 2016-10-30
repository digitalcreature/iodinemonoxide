using UnityEngine;

public class HandCursor : MonoBehaviour {

	public Color openColor = Color.white;
	public Color pinchColor = Color.green;
	public float pinchThreshold = 0.5f;
	public AnimationCurve pinchScale;
	public AnimationCurve pinchAlpha;
	public Transform model;

	public Transform grabbedItem { get; private set; }
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
	private static HandCursor dragging; // the cursoor currently executing a drag, if any

	void Awake() {
		render = model.GetComponent<Renderer>();
	}

	public void OnFrame(Vector3 position, float pinch, Vector3 delta) {
		this.wasPinching = this.isPinching;
		this.lastPosition = this.position;
		this.position = position;
		this.delta = delta;
		this.isPinching = pinch > pinchThreshold;
		transform.localPosition = position;
		Material mat = render.material;
		Color color = isPinching ? pinchColor : openColor;
		// color.a = pinchAlpha.Evaluate(pinch);
		mat.color = color;
		render.material = mat;
		// model.localScale = Vector3.one * pinchScale.Evaluate(pinch);
		MotionManager manager = MotionManager.instance;
		if (isPinching && !wasPinching) {
			if (position.y < manager.binHeight) {
				Atom atom = manager.atomPrefab.CreateNew(manager.binElement);
				grabbedItem = atom.transform;
				grabOffset = Vector3.zero;
			}
		}
		if (!isPinching && grabbedItem != null) {
			grabbedItem = null;
		}
	}

	void Update() {
		if (grabbedItem != null) {
			grabbedItem.transform.position = transform.position + grabOffset;
		}
	}

}
