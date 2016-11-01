using UnityEngine;
using Leap;
using System.Collections.Generic;

public class MotionManager : SingletonBehaviour<MotionManager> {

	public HandCursor cursorPrefab;

	private Leap.Controller controller;
	private HashSet<HandCursor> allCursors;
	private HashSet<HandCursor> activeCursors;

	public bool inputEnabled { get; private set; }

	public void Initialize() {
		if (controller == null) {
			controller = new Leap.Controller();
		}
		if (allCursors == null) {
			allCursors = new HashSet<HandCursor>();
		}
		if (activeCursors == null) {
			activeCursors = new HashSet<HandCursor>();
		}
		SetInputEnabled(false);
	}

	public void SetInputEnabled(bool enabled) {
		inputEnabled = enabled;
		if (!enabled) {
			activeCursors.Clear();
			foreach (HandCursor cursor in allCursors) {
				Destroy(cursor.gameObject);
			}
			allCursors.Clear();
		}
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
		if (inputEnabled) {
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

}


public static class LeapE {

	public static Vector3 Vec3(this Leap.Vector vec) {
		return new Vector3(
			vec.x,
			vec.y,
			- vec.z
		);
	}

}
