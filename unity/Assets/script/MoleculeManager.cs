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

	public void OnFrame(HashSet<HandCursor> activeCursors) {
		foreach (HandCursor cursor in activeCursors) {
			if (cursor.isPinching) {
				if (!cursor.wasPinching) {
					if (cursor.position.y < binHeight) {
						Atom atom = atomPrefab.CreateNew(binElement);
						atoms.Add(atom);
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

}
