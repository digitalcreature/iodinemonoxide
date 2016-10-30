using UnityEngine;

public class Bond : Manipulable {

	public float lengthScale = 0.5f;

	public Atom a { get; private set; }
	public Atom b { get; private set; }

	public static bool CanBond(Atom a, Atom b, int aBondCount, int bBondCount) {
		return (aBondCount < a.element.maxBonds) && (bBondCount < b.element.maxBonds)
		 && !a.bonds.ContainsValue(b) && !b.bonds.ContainsValue(a);
	}
	public static bool CanBond(Atom a, Atom b, int aBondCount) {
		return CanBond(a, b, aBondCount, b.bonds.Count);
	}
	public static bool CanBond(Atom a, Atom b) {
		return CanBond(a, b, a.bonds.Count, b.bonds.Count);
	}

	public Bond CreateNew(Atom a, Atom b) {
		if (CanBond(a, b)) {
			Bond bond = Instantiate(this);
			MoleculeManager molecule = MoleculeManager.instance;
			molecule.bonds.Add(bond);
			bond.transform.parent = molecule.transform;
			bond.a = a;
			bond.b = b;
			a.bonds[bond] = b;
			b.bonds[bond] = a;
			bond.name = name;
			Vector3 aPos = a.transform.position;
			Vector3 bPos = b.transform.position;
			bond.transform.position = (aPos + bPos) / 2;
			Vector3 scale = bond.transform.localScale;
			scale.y = (aPos - bPos).magnitude * lengthScale;
			bond.transform.localScale = scale;
			bond.transform.up = aPos - bPos;
			return bond;
		}
		else {
			return null;
		}
	}

	public void Break() {
		a.bonds.Remove(this);
		b.bonds.Remove(this);
		Destroy(gameObject);
		MoleculeManager.instance.bonds.Remove(this);
	}

}
