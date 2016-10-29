using UnityEngine;
using Leap;
using System.Collections.Generic;

public class MotionManager : SingletonBehaviour<MotionManager> {

	public HandCursor cursorPrefab;

	private Leap.Controller controller;
	private HashSet<HandCursor> allCursors;
	private HashSet<HandCursor> freeCursors;

	void Awake() {
		controller = new Leap.Controller();
		allCursors = new HashSet<HandCursor>();
		freeCursors = new HashSet<HandCursor>();
	}

	private Vector3 ConvertVector(Leap.Vector v) {
		return new Vector3(
			v.x,
			v.y,
			-v.z
		);
	}

	HandCursor GetCursor() {
		HandCursor cursor = null;
		if (freeCursors.Count == 0) {
			cursor = Instantiate(cursorPrefab);
			cursor.name = cursorPrefab.name;
			allCursors.Add(cursor);
		}
		else {
			foreach (HandCursor free in freeCursors) {
				cursor = free;
				break;
			}
			freeCursors.Remove(cursor);
			cursor.gameObject.SetActive(true);
		}
		return cursor;
	}

	void Update() {
		if (controller != null) {
			Frame frame = controller.Frame();
			HandList hands = frame.Hands;
			for (int h = 0; h < hands.Count; h ++) {
				Hand hand = hands[h];
				Vector3 pos = ConvertVector(hand.PalmPosition) / 10; // convert from mm to cm
				float pinch = hand.PinchStrength;
				HandCursor cursor = GetCursor();
				cursor.UpdatePosition(pos, pinch);
			}
		}
		foreach (HandCursor cursor in allCursors) {
			if (freeCursors.Contains(cursor)) {
				cursor.gameObject.SetActive(false);
			}
			else {
				freeCursors.Add(cursor);
			}
		}
	}


}
