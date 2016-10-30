using UnityEngine;
using Leap;
using System.Collections.Generic;

public class MotionManager : SingletonBehaviour<MotionManager> {

	public HandCursor cursorPrefab;
	public float binHeight = 10; // the height of the virtual "bin" of atoms
	public Element binElement;
	public Atom atomPrefab;

	private Leap.Controller controller;
	private HashSet<HandCursor> allCursors;
	private HashSet<HandCursor> activeCursors;

	void Awake() {
		controller = new Leap.Controller();
		allCursors = new HashSet<HandCursor>();
		activeCursors = new HashSet<HandCursor>();
		new GameObject("atom").AddComponent<Atom>();
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
		if (allCursors.Count - activeCursors.Count == 0) {
			cursor = Instantiate(cursorPrefab);
			cursor.name = cursorPrefab.name;
			allCursors.Add(cursor);
			activeCursors.Add(cursor);
		}
		else {
			foreach (HandCursor c in allCursors) {
				if (!activeCursors.Contains(c)) {
					cursor = c;
					break;
				}
			}
			activeCursors.Add(cursor);
			cursor.gameObject.SetActive(true);
		}
		return cursor;
	}

	void FixedUpdate() {
		activeCursors.Clear();
		Frame frame = controller.Frame();
		HandList hands = frame.Hands;
		for (int h = 0; h < hands.Count; h ++) {
			Hand hand = hands[h];
			Vector3 pos = ConvertVector(hand.PalmPosition) / 10; // convert from mm to cm
			Vector3 dpos = ConvertVector(hand.PalmVelocity) / 10;
			dpos *= Time.deltaTime;
			float pinch = hand.PinchStrength;
			HandCursor cursor = GetCursor();
			cursor.OnFrame(pos, pinch, dpos);
			cursor.transform.parent = transform;
		}
		//hide all inactive cursors
		foreach (HandCursor cursor in allCursors) {
			if (!activeCursors.Contains(cursor)) {
				cursor.gameObject.SetActive(false);
			}
		}
		//process inputs
		bool dragging = false;	//only one cursor can drag at a time (for now)
		foreach (HandCursor cursor in activeCursors) {
			if (cursor.isPinching) {
				if (cursor.position.y < binHeight) {
					if (!cursor.wasPinching) {

					}
				}
			}
		}
	}


}
