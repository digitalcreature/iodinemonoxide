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

	public Dictionary<Bond, Atom> bonds { get; private set; }

	public IEnumerable<Atom> potentialBonds {
		get {
			MoleculeManager molecule = MoleculeManager.instance;
			int bondCount = bonds.Count;
			foreach (Atom atom in molecule.atoms) {
				if (bondCount >= element.maxBonds) {
					break;
				}
				else if (atom != this) {
					if (Bond.CanBond(this, atom, bondCount)) {
						Vector3 posA = atom.transform.position;
						Vector3 posB = transform.position;
						float bondDistance = molecule.bondDistance;
						if ((posA - posB).sqrMagnitude < bondDistance * bondDistance) {
							bondCount ++;
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
		bonds = new Dictionary<Bond, Atom>();
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
