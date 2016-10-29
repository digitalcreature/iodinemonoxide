using UnityEngine;

public class HandCursor : MonoBehaviour {

	public AnimationCurve pinchScale;
	public AnimationCurve pinchAlpha;
	public Transform model;


	private Renderer render;

	void Awake() {
		render = model.GetComponent<Renderer>();
	}

	public void UpdatePosition(Vector3 position, float pinch) {
		transform.position = position;
		Material mat = render.material;
		Color color = mat.color;
		color.a = pinchAlpha.Evaluate(pinch);
		mat.color = color;
		render.material = mat;
		model.localScale = Vector3.one * pinchScale.Evaluate(pinch);
	}

}
