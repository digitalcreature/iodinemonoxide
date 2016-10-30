using UnityEngine;

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

	private Renderer render;

	void Awake() {
		render = GetComponent<Renderer>();
	}

	public Atom CreateNew(Element element) {
		Atom atom = Instantiate(this);
		atom.element = element;
		return atom;
	}

}
