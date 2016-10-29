using UnityEngine;
using Leap;

public class MotionManager : MonoBehaviour {

	private Leap.Controller controller;

	void Awake() {
		controller = new Leap.Controller();
	}

	private Vector3 vec(Leap.Vector v) {
		return new Vector3(
			v.x,
			v.y,
			-v.z
		) / 10;
	}

	void Update() {
		if (controller != null) {
			Frame frame = controller.Frame();
			HandList hands = frame.Hands;
			for (int h = 0; h < hands.Count; h ++) {
				Hand hand = hands[h];
				Vector3 pos = vec(hand.PalmPosition);
				Vector3 normal = vec(hand.PalmNormal);
			}
		}
	}


}
