using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SessionManager : SingletonBehaviour<SessionManager> {

	public string username = "Barry M";
	public int userid = 1;
	public Canvas loginScreen;
	public InputField nameField;

	public bool sessionIsActive { get; private set; }

	void Awake() {
		nameField.text = username;
		sessionIsActive = false;
		Initialize();
	}

	public void Initialize() {
		sessionIsActive = false;
		MotionManager motion = MotionManager.instance;
		MoleculeManager molecule = MoleculeManager.instance;
		CameraRig rig = CameraRig.instance;
		motion.Initialize();
		molecule.Initialize();
		rig.Initialize();
		molecule.editorUI.gameObject.SetActive(false);
		loginScreen.gameObject.SetActive(true);
		CaptureCamera capture = CaptureCamera.instance;
		capture.Initialize();
	}

	public void StartSession() {
		username = nameField.text;
		MoleculeManager.instance.editorUI.gameObject.SetActive(true);
		MotionManager.instance.SetInputEnabled(true);
		loginScreen.gameObject.SetActive(false);
		CaptureCamera.instance.PostScreenshot(username, userid);
		sessionIsActive = true;
	}

	public void EndSession() {
		Initialize();
		nameField.Select();
		nameField.ActivateInputField();
	}

	void Update() {
		if (sessionIsActive) {
			if (Input.GetKeyDown("escape")) {
				EndSession();
			}
		}
		else {
			if (Input.GetKeyDown("i")) {
				userid = 1;
			}
			if (Input.GetKeyDown("o")) {
				userid = 2;
			}
		}
	}
}
