using UnityEngine;
using System.Collections.Generic;

public class Manipulable : MonoBehaviour {

	public float animationSmoothing = 10;

	protected Renderer render;

	public float outlineThickness {
		get {
			return render.material.GetFloat("_Thickness");
		}
		set {
			Material mat = render.material;
			mat.SetFloat("_Thickness", value);
			render.material = mat;
		}
	}
	public Color outlineColor {
		get {
			return render.material.GetColor("_OutlineColor");
		}
		set {
			Material mat = render.material;
			mat.SetColor("_OutlineColor", value);
			render.material = mat;
		}
	}

	protected virtual void Awake() {
		render = GetComponent<Renderer>();
	}

}
