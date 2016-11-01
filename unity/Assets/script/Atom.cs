using UnityEngine;
using System.Collections.Generic;

public class Atom : Manipulable {

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

	[HideInInspector]
	public HandCursor grabbingCursor;

	public bool invalid { get; private set; }

	public Dictionary<Bond, Atom> bonds { get; private set; }

	public IEnumerable<Atom> potentialBonds {
		get {
			MoleculeManager molecule = MoleculeManager.instance;
			int bondCount = bonds.Count;
			foreach (Atom atom in molecule.atoms) {
				if (bondCount >= element.maxBonds) {
					break;
				}
				else {
					if (Bond.CanBond(this, atom, bondCount)) {
						bondCount ++;
						yield return atom;
					}
				}
			}
		}
	}

	private SphereCollider hull;

	protected override void Awake() {
		base.Awake();
		bonds = new Dictionary<Bond, Atom>();
		hull = GetComponent<SphereCollider>();
	}

	public Atom CreateNew(Element element) {
		Atom atom = Instantiate(this);
		atom.element = element;
		atom.transform.parent = MoleculeManager.instance.transform;
		return atom;
	}

	public void OnFrame() {
		MoleculeManager molecule = MoleculeManager.instance;
		outlineThickness = 0.5f;
		invalid = true;
		if (molecule.atoms.Count == 0) {
			invalid = false;
			outlineColor = Color.green;
		}
		else {
			outlineColor = Color.red;
			if (!Physics.CheckSphere(transform.position, hull.bounds.extents.x,
			-1, QueryTriggerInteraction.Ignore)) {
				foreach (Atom atom in potentialBonds) {
					atom.outlineThickness = 0.1f;
					atom.outlineColor = Color.blue;
					invalid = false;
					outlineColor = Color.green;
				}
			}
		}
	}

	public void OnGrab(HandCursor cursor) {
		grabbingCursor = cursor;
		hull.isTrigger = true;
	}

	public void OnRelease(HandCursor cursor) {
		grabbingCursor = null;
		hull.isTrigger = false;
		if (invalid) {
			Destroy(gameObject);
			return;
		}
		else {
			MoleculeManager molecule = MoleculeManager.instance;
			HashSet<Atom> atoms = molecule.atoms;
			Vector3 pos = transform.position;
			pos = molecule.transform.InverseTransformPoint(pos);
			int newBonds = 0;
			foreach (Atom atom in potentialBonds) {
				molecule.bondPrefab.CreateNew(this, atom);
				newBonds ++;
			}
			outlineThickness = 0;
			molecule.AddAtom(this);
		}
	}
}
