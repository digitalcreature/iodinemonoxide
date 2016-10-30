using UnityEngine;
using Leap;
using System.Collections.Generic;

public class MotionManager : SingletonBehaviour<MotionManager> {

	public HandCursor cursorPrefab;

	private Leap.Controller controller;
	private HashSet<HandCursor> allCursors;
	private HashSet<HandCursor> activeCursors;

	void Awake() {
		controller = new Leap.Controller();
		allCursors = new HashSet<HandCursor>();
		activeCursors = new HashSet<HandCursor>();
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
			cursor.transform.parent = transform;
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
			HandCursor cursor = GetCursor();
			cursor.OnFrame(hand);
		}
		//hide all inactive cursors
		foreach (HandCursor cursor in allCursors) {
			if (!activeCursors.Contains(cursor)) {
				cursor.gameObject.SetActive(false);
				cursor.Release();
			}
		}
		MoleculeManager.instance.OnFrame(activeCursors);
	}


}
