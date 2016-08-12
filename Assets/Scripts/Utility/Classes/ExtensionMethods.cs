using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

public static class RigidbodyExtension {
    /// <summary>
    /// Disables/Reenables the rigidbody.
    /// Deacting will disable the gravity and collision detection, as well as resetting the linear and angular velocity.
    /// Reactivating will do the opposite, except will not affect the velocity.
    /// </summary>
    /// <param name="body"></param>
    /// <param name="state">True to activate, False to deactivate</param>
    public static void SetEnabled(this Rigidbody body, bool state) {
        body.useGravity = state;
        body.detectCollisions = state;
		body.isKinematic = !state;
		body.interpolation = state ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;

        if (state == false) {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    public static bool IsEnabled(this Rigidbody rbody) {
        return rbody.detectCollisions && rbody.useGravity;
    }
}

public static class VectorExtensions {

	public static Vector2 new_x(this Vector2 vec, float new_x) {
		return new Vector2 (new_x, vec.y);
	}
	public static Vector2 new_y(this Vector2 vec, float new_y) {
		return new Vector2 (vec.x, new_y);
	}

	public static Vector3 new_x(this Vector3 vec, float new_x) {
		return new Vector3 (new_x, vec.y, vec.z);
	}
	public static Vector3 new_y(this Vector3 vec, float new_y) {
		return new Vector3 (vec.x, new_y, vec.z);
	}
	public static Vector3 new_z(this Vector3 vec, float new_z) {
		return new Vector3 (vec.x, vec.y, new_z);
	}

	public static Vector2 xy(this Vector2 vec) {
		return vec;
	}
	public static Vector2 yx(this Vector2 vec) {
		return new Vector2 (vec.y, vec.x);
	}

	public static Vector2 xy(this Vector3 vec) {
		return new Vector2 (vec.x, vec.y);
	}
	public static Vector2 xz(this Vector3 vec) {
		return new Vector2 (vec.x, vec.z);
	}
	public static Vector2 yz(this Vector3 vec) {
		return new Vector2 (vec.y, vec.z);
	}
	public static Vector2 yx(this Vector3 vec) {
		return new Vector2 (vec.y, vec.x);
	}
	public static Vector2 zx(this Vector3 vec) {
		return new Vector2 (vec.z, vec.x);
	}
	public static Vector2 zy(this Vector3 vec) {
		return new Vector2 (vec.z, vec.y);
	}

	public static Vector3 xyz(this Vector3 vec) {
		return vec;
	}
	public static Vector3 xzy(this Vector3 vec) {
		return new Vector3 (vec.x, vec.z, vec.y);
	}
	public static Vector3 yxz(this Vector3 vec) {
		return new Vector3 (vec.y, vec.x, vec.z);
	}
	public static Vector3 yzx(this Vector3 vec) {
		return new Vector3 (vec.y, vec.z, vec.x);
	}
	public static Vector3 zxy(this Vector3 vec) {
		return new Vector3 (vec.z, vec.x, vec.y);
	}
	public static Vector3 zyx(this Vector3 vec) {
		return new Vector3 (vec.z, vec.y, vec.x);
	}

	public static Vector3 xyz(this Vector2 vec, float z) {
		return new Vector3 (vec.x, vec.y, z);
	}
	public static Vector3 xzy(this Vector2 vec, float z) {
		return new Vector3 (vec.x, z, vec.y);
	}
	public static Vector3 yxz(this Vector2 vec, float z) {
		return new Vector3 (vec.y, vec.x, z);
	}
	public static Vector3 yzx(this Vector2 vec, float z) {
		return new Vector3 (vec.y, z, vec.x);
	}
	public static Vector3 zxy(this Vector2 vec, float z) {
		return new Vector3 (z, vec.x, vec.y);
	}
	public static Vector3 zyx(this Vector2 vec, float z) {
		return new Vector3 (z, vec.y, vec.x);
	}

	public static float ToDegrees(this Vector2 vec) {
		return Mathf.Atan2 (vec.y, vec.x) * Mathf.Rad2Deg;
	}
	public static float ToRadians(this Vector2 vec) {
		return Mathf.Atan2 (vec.y, vec.x);
	}

	public static Vector2 FromRadians(this float rad) {
		return new Vector2 (Mathf.Cos (rad), Mathf.Sin (rad));
	}
	public static Vector2 FromRadians(this float rad, float radius) {
		return new Vector2 (Mathf.Cos (rad), Mathf.Sin (rad)) * radius;
	}

	public static Vector2 FromDegrees(this float deg) {
		return new Vector2 (Mathf.Cos (deg * Mathf.Deg2Rad), Mathf.Sin (deg * Mathf.Deg2Rad));
	}
	public static Vector2 FromDegrees(this float deg, float radius) {
		return new Vector2 (Mathf.Cos (deg * Mathf.Deg2Rad), Mathf.Sin (deg * Mathf.Deg2Rad)) * radius;
	}

	public static Vector2 FromRadians(this int rad) {
		return new Vector2 (Mathf.Cos (rad), Mathf.Sin (rad));
	}
	public static Vector2 FromRadians(this int rad, float radius) {
		return new Vector2 (Mathf.Cos (rad), Mathf.Sin (rad)) * radius;
	}

	public static Vector2 FromDegrees(this int deg) {
		return new Vector2 (Mathf.Cos (deg * Mathf.Deg2Rad), Mathf.Sin (deg * Mathf.Deg2Rad));
	}
	public static Vector2 FromDegrees(this int deg, float radius) {
		return new Vector2 (Mathf.Cos (deg * Mathf.Deg2Rad), Mathf.Sin (deg * Mathf.Deg2Rad)) * radius;
	}

	public static float MaxValue(this Vector3 vector) {
		return Mathf.Max(vector.x, vector.y, vector.z);
	}

	public static Vector2 Average(this Vector2[] vectors) {
		if (vectors.Length == 0)
			return Vector2.zero;

		Vector2 total = Vector2.zero;
		for (int i = 0; i < vectors.Length; i++) {
			total += vectors[i];
		}

		return total / vectors.Length;
	}

	public static Vector3 Average(this Vector3[] vectors) {
		if (vectors.Length == 0)
			return Vector3.zero;

		Vector3 total = Vector3.zero;
		for (int i = 0; i < vectors.Length; i++) {
			total += vectors[i];
		}

		return total / vectors.Length;
	}

	public static Vector2 Abs(this Vector2 vector) {
		return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
	}
	public static Vector3 Abs(this Vector3 vector) {
		return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
	}
}

public static class ListExtensions {

	public static void Shuffle<T> (this IList<T> list) {
		RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider ();
		int n = list.Count;
		while (n > 1) {
			byte[] box = new byte[1];
			do
				provider.GetBytes (box);
			while (!(box [0] < n * (Byte.MaxValue / n)));
			int k = (box [0] % n);
			n--;
			T value = list [k];
			list [k] = list [n];
			list [n] = value;
		} 
	}

	public static int IndexOf<T>(this T[] list, T item) {
		for (int index = 0; index < list.Length; index++) {
			if ((item == null && list[index] == null) || (item != null && item.Equals(list[index]))) {
				return index;
			}
		}
		return -1;
	}

	public static int LastIndexOf<T>(this T[] list, T item) {
		for (int index = list.Length-1; index >= 0; index--) {
			if ((item == null && list[index] == null) || (item != null && item.Equals(list[index]))) {
				return index;
			}
		}
		return -1;
	}

	public static string ToFancyString<T>(this T[] list) {
		string s = "";
		for (int index = 0; index < list.Length; index++) {
			s += "[" + index + "]=" + (list[index] == null ? "null" : list[index].ToString()) + ";";
		}

		return list.ToString() + "{ " + s + " }";
	}

	public static T GetRandom<T>(this T[] list) {
		return list[UnityEngine.Random.Range(0, list.Length)];
	}

	public static T GetRandom<T>(this List<T> list) {
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static int GetRandomIndex<T>(this List<T> list) {
		if (list.Count == 0) return -1;
		return UnityEngine.Random.Range(0, list.Count);
	}

	public static int GetRandomIndex<T>(this List<T> list, System.Predicate<T> match) {
		List<int> valid = new List<int>();

		for (int i = 0; i < list.Count; i++) {
			if (match(list[i])) valid.Add(i);
		}

		if (valid.Count == 0) return -1;
		return valid[UnityEngine.Random.Range(0, valid.Count)];
	}

	public static T Pop<T>(this List<T> list) {
		return list.Pop(list.Count - 1);
	}

	public static T Pop<T>(this List<T> list, int index) {
		if (list.Count <= index || index < 0) throw new System.Exception("Invalid index");
		T value = list[index];
		list.RemoveAt(index);
		return value;
	}

}

public static class TransformExitensions {
	/// <summary>
	/// Get the full hierarchy path of a transfrom.
	/// Recursive.
	/// </summary>
	public static string GetPath(this Transform self) {
		// Recursive
		if (self.parent == null)
			return self.name;

		return self.parent.GetPath() + "/" + self.name;
	}

	/// <summary>
	/// Is the transform selected in the heirarchy?
	/// </summary>
	public static bool IsSelected(this Transform self, bool includeParents = true) {
#if UNITY_EDITOR
		foreach (Transform t in UnityEditor.Selection.transforms) {
			if (t == self || (includeParents && self.IsChildOf(t))) return true;
		}
		return false;
#else
		return false;
#endif
	}
}

public static class ColliderExstensions {
	public static GameObject GetMainObject(this Collider col) {
		return col.attachedRigidbody ? col.attachedRigidbody.gameObject : col.gameObject;
	}
}

public static class GameObjectExtensions {
	public static GameObject Clone(this GameObject go) {
		GameObject clone = GameObject.Instantiate(go);
		clone.transform.localPosition = go.transform.localPosition;
		clone.transform.localRotation = go.transform.localRotation;
		clone.transform.localScale = go.transform.localScale;
		return clone;
	}
}
