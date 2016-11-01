using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MoleculeManager : SingletonBehaviour<MoleculeManager> {

	public float binHeight = 10; // the height of the virtual "bin" of atoms
	public Element[] binElements;
	public int binElementIndex = 0;
	public Canvas editorUI;
	public Text elementNameText;
	public Atom atomPrefab;
	public Bond bondPrefab;
	public Element binElement {
		get {
			return binElements[binElementIndex];
		}
	}

	public float bondDistance = 10;

	public HashSet<Atom> atoms { get; private set; }
	public HashSet<Bond> bonds { get; private set; }

	public Vector3 center { get; private set; }
	public float boundingRadius { get; private set; }

	public void Initialize() {
		if (atoms != null) {
			foreach (Atom atom in atoms) {
				atom.gameObject.SetActive(false);
				Destroy(atom.gameObject);
			}
		}
		if (bonds != null) {
			foreach (Bond bond in bonds) {
				bond.gameObject.SetActive(false);
				Destroy(bond.gameObject);
			}
		}
		atoms = new HashSet<Atom>();
		bonds = new HashSet<Bond>();
		center = Vector3.zero;
		boundingRadius = 0;
	}

	public void AddAtom(Atom newAtom) {
		atoms.Add(newAtom);
		center = Vector3.zero;
		Bounds bounds = new Bounds();
		int i = 0;
		foreach (Atom atom in atoms) {
			if (i == 0) {
				bounds.center = atom.transform.position;
			}
			else {
				bounds.Encapsulate(atom.transform.position);
			}
			i ++;
		}
		center = bounds.center;
		boundingRadius = 0;
		foreach (Atom atom in atoms) {
			float sqrRadius = (atom.transform.position - center).sqrMagnitude;
			if (sqrRadius > boundingRadius) {
				boundingRadius = sqrRadius;
			}
		}
		boundingRadius = Mathf.Sqrt(boundingRadius);
		CameraRig.instance.Refocus();
		CaptureCamera.instance.OnMoleculeChange();
	}

	public void OnFrame(HashSet<HandCursor> activeCursors) {
		foreach (Atom atom in atoms) {
			atom.outlineThickness = 0;
		}
		foreach (HandCursor cursor in activeCursors) {
			if (cursor.isPinching) {
				if (!cursor.wasPinching) {
					if (cursor.position.y < binHeight) {
						Atom atom = atomPrefab.CreateNew(binElement);
						cursor.Grab(atom);
					}
				}
				if (cursor.grabbedAtom != null) {
					cursor.UpdateGrab();
					cursor.grabbedAtom.OnFrame();
				}
			}
			else {
				cursor.Release();
			}
		}
	}

	void Update() {
		elementNameText.text = binElement.name;
		if (Input.GetKeyDown("left")) {
			PreviousElement();
		}
		if (Input.GetKeyDown("right")) {
			NextElement();
		}
	}

	public void NextElement() {
		binElementIndex = (binElementIndex + 1) % binElements.Length;
	}

	public void PreviousElement() {
		binElementIndex --;
		while (binElementIndex < 0) {
			binElementIndex += binElements.Length;
		}
	}

	void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(center, boundingRadius);
	}

}
