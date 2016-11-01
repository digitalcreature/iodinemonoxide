using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {

	private static T _instance;
	public static T instance {
		get {
			if (_instance == null) {
				_instance = Object.FindObjectOfType<T>();
			}
			return _instance;
		}
	}

}
