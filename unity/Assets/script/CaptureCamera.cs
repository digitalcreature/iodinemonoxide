using UnityEngine;

public class CaptureCamera : SingletonBehaviour<CaptureCamera> {

	public int imageWidth = 360;
	public int imageHeight = 360;

	private Camera cam;
	private RenderTexture tex;

	void Awake() {
		cam = GetComponent<Camera>();
		tex = new RenderTexture(imageWidth, imageHeight, 24);
		cam.targetTexture = tex;
	}

	public void OnMoleculeChange() {
		
	}

}
