using UnityEngine;
using System.IO;

public class CaptureCamera : SingletonBehaviour<CaptureCamera> {

	public int imageWidth = 360;
	public int imageHeight = 360;
	public float minRadius = 3;
	public float padding = 10;
	public Shader shader;
	public Gradient backgroundColors;

	private Camera cam;
	private RenderTexture tex;

	void Awake() {
		cam = GetComponent<Camera>();
		cam.backgroundColor = backgroundColors.Evaluate(Random.value);
		tex = new RenderTexture(imageWidth, imageHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		tex.antiAliasing = 8;
		tex.anisoLevel = 0;
		cam.targetTexture = tex;
		if (shader != null) {
			cam.SetReplacementShader(shader, "RenderType");
		}
	}

	public void OnMoleculeChange() {
		MoleculeManager molecule = MoleculeManager.instance;
		transform.position = molecule.center - Vector3.forward * (molecule.boundingRadius + 50);
		cam.orthographicSize = Mathf.Max(molecule.boundingRadius + padding, minRadius);
		SavePNG("screenshot.png");
	}

	public void SavePNG(string fname) {
		RenderTexture current = RenderTexture.active;
		RenderTexture.active = tex;
		cam.Render();
		Texture2D img = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
		img.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
		img.Apply();
		byte[] png = img.EncodeToPNG();
		File.WriteAllBytes(fname, png);
		RenderTexture.active = current;
	}

}
