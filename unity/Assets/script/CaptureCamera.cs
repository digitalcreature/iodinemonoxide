using UnityEngine;

public class CaptureCamera : SingletonBehaviour<CaptureCamera> {

	public int imageWidth = 360;
	public int imageHeight = 360;
	public float padding = 10;
	public Shader shader;

	private Camera cam;
	private RenderTexture tex;

	void Awake() {
		cam = GetComponent<Camera>();
		tex = new RenderTexture(imageWidth, imageHeight, 8);
		tex.antiAliasing = 8;
		cam.targetTexture = tex;
		if (shader != null) {
			cam.SetReplacementShader(shader, "RenderType");
		}
	}

	public void OnMoleculeChange() {
		MoleculeManager molecule = MoleculeManager.instance;
		transform.position = molecule.center - Vector3.forward * (molecule.boundingRadius + 50);
		cam.orthographicSize = molecule.boundingRadius + padding;
	}

}
