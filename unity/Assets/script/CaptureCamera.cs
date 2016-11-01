using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class CaptureCamera : SingletonBehaviour<CaptureCamera> {

	public int imageWidth = 360;
	public int imageHeight = 360;
	public float minRadius = 3;
	public float padding = 10;
	public Shader shader;

	private Camera cam;
	private RenderTexture tex;

	public void Initialize() {
		cam = GetComponent<Camera>();
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
		SessionManager session = SessionManager.instance;
		PostScreenshot(session.username, session.userid);
	}

	public void PostScreenshot(string username, int userid) {
		StartCoroutine(ScreenShotPostRequest(username, userid));
	}

	System.Collections.IEnumerator ScreenShotPostRequest(string username, int userid) {
		string url = "https://still-thicket-59143.herokuapp.com/";
		byte[] png = RenderPNG();
		string base64 = System.Convert.ToBase64String(png);
		Debug.Log(base64);
		WWWForm form = new WWWForm();
		form.AddField("name_field", username);
		form.AddField("finished_field", "true");
		form.AddField("image", base64);
		WWW xhr = new WWW(url + "update/" + userid, form);
		yield return xhr;
		foreach (KeyValuePair<string, string> entry in xhr.responseHeaders) {
			Debug.Log(entry.Key + ": " + entry.Value);
		}
	}

	public byte[] RenderPNG() {
		RenderTexture current = RenderTexture.active;
		RenderTexture.active = tex;
		cam.backgroundColor = CameraRig.instance.backgroundColor;
		cam.Render();
		Texture2D img = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
		img.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
		img.Apply();
		RenderTexture.active = current;
		return img.EncodeToPNG();
	}

}
