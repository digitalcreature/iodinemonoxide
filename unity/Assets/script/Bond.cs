using UnityEngine;

public class Bond : MonoBehaviour {

	public float lengthScale = 0.5f;

	public Atom a { get; private set; }
	public Atom b { get; private set; }

	public Bond CreateNew(Atom a, Atom b) {
		if (a.canBond && b.canBond) {
			Bond bond = Instantiate(this);
			a.bonds.Add(bond);
			b.bonds.Add(bond);
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

}
