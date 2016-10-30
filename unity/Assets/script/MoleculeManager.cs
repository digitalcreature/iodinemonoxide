using UnityEngine;
using System.Collections.Generic;

public class MoleculeManager : SingletonBehaviour<MoleculeManager> {

	public float binHeight = 10; // the height of the virtual "bin" of atoms
	public Element binElement;
	public Atom atomPrefab;
	public Bond bondPrefab;

	public float bondDistance = 10;

	public HashSet<Atom> atoms { get; private set; }

	void Awake() {
		atoms = new HashSet<Atom>();
	}

	public Vector3 center { get; private set; }
	public float boundingRadius { get; private set; }

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
					cursor.grabbedAtom.OnFrame();
				}
			}
			else {
				cursor.Release();
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(center, boundingRadius);
	}

}
