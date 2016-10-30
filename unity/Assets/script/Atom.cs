using UnityEngine;
using System.Collections.Generic;

public class Atom : MonoBehaviour {

	public float scale = 1;

	private Element _element;
	public Element element {
		get {
			return _element;
		}
		set {
			_element = value;
			if (value == null) {
				gameObject.SetActive(false);
			}
			else {
				name = value.name + " atom";
				render.material = value.material;
				transform.localScale = Vector3.one * value.size * scale;
			}
		}
	}

	public HandCursor grabbingCursor;

	public HashSet<Bond> bonds { get; private set; }
	public bool canBond {
		get {
			return bonds.Count < element.maxBonds;
		}
	}

	public IEnumerable<Atom> potentialBonds {
		get {
			MoleculeManager molecule = MoleculeManager.instance;
			int newBonds = 0;
			foreach (Atom atom in molecule.atoms) {
				if (atom != this) {
					if (((bonds.Count + newBonds) < element.maxBonds) && atom.canBond) {
						Vector3 posA = atom.transform.position;
						Vector3 posB = transform.position;
						float bondDistance = molecule.bondDistance;
						if ((posA - posB).sqrMagnitude < bondDistance * bondDistance) {
							newBonds ++;
							yield return atom;
						}
					}
				}
			}
		}
	}

	private Renderer render;

	void Awake() {
		render = GetComponent<Renderer>();
		bonds = new HashSet<Bond>();
	}

	public Atom CreateNew(Element element) {
		Atom atom = Instantiate(this);
		atom.element = element;
		return atom;
	}

	public void OnFrame() {
		MoleculeManager molecule = MoleculeManager.instance;
		foreach (Atom atom in potentialBonds) {
			Debug.DrawLine(transform.position, atom.transform.position);
		}
	}

	public void OnGrab(HandCursor cursor) {
		grabbingCursor = cursor;
	}

	public void OnRelease(HandCursor cursor) {
		grabbingCursor = null;
		MoleculeManager molecule = MoleculeManager.instance;
		HashSet<Atom> atoms = molecule.atoms;
		float binHeight = molecule.binHeight;
		Vector3 pos = transform.position;
		pos = molecule.transform.InverseTransformPoint(pos);
		if (cursor.position.y < binHeight) {
			Debug.Log("dropped in bin");
			atoms.Remove(this);
			Destroy(gameObject);
		}
		else {
			int newBonds = 0;
			foreach (Atom atom in potentialBonds) {
				molecule.bondPrefab.CreateNew(this, atom);
				newBonds ++;
			}
			if (newBonds == 0) {
				if (atoms.Count > 1) {
					Debug.LogFormat("there are {0} atoms", atoms.Count);
					atoms.Remove(this);
					Destroy(gameObject);
				}
			}
		}
	}
}
