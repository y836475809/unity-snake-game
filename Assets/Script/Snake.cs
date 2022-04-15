using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Snake : MonoBehaviour {
	// Did the snake eat something?
	bool ate = false;

	//Did user died?
	bool isDied = false;

	// Tail Prefab
	public GameObject tailPrefab;

	// Current Movement Direction
	// (by default it moves to the right)
	Vector2 dir = Vector2.right;

	// Keep Track of Tail
	List<Transform> tail = new List<Transform>();

	int raysCount = 9;
	public float rayLength = 4;
	Vector3[] starts;
	Vector3[] ends;

	// Use this for initialization
	void Start () {
		starts = new Vector3[raysCount];
		ends = new Vector3[raysCount];
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (isDied) {
			if (Input.GetKey(KeyCode.R)) {
				//clear the tail
				foreach (var t in tail) {
					Destroy(t.gameObject);
				}
				tail.Clear();

				//reset to origin
				transform.position = new Vector3(0, 0, 0);
				transform.rotation = Quaternion.Euler(0f, 0f, 0f);

				//make snake alive
				isDied = false;
			}
			return;
		}

		var rot = 0f;
		// Move in a new Direction?
		if (Input.GetKey(KeyCode.RightArrow)) {
			dir = Vector2.right;
			rot = -0.3f;
		} else if (Input.GetKey(KeyCode.DownArrow)) {
			dir = -Vector2.up;    // '-up' means 'down'
		} else if (Input.GetKey(KeyCode.LeftArrow)) {
			dir = -Vector2.right; // '-right' means 'left'
			rot = 0.3f;
		} else if (Input.GetKey(KeyCode.UpArrow)) {
			dir = Vector2.up;
		}
		float speed = 10;
		transform.Rotate(0, 0, rot*5);
		Vector3 velocity = transform.rotation * new Vector3(0, 1, 0);
		transform.position += velocity* speed * Time.deltaTime;

		CalculateEnds();

		for (int i = 0; i < raysCount; i++) {
			var hit = Physics2D.Linecast(starts[i], ends[i]);
			if (hit.collider != null) {
				Debug.DrawLine(starts[i], ends[i], Color.red);
            } else {
				Debug.DrawLine(starts[i], ends[i], Color.white);
			}
		}

		Move();

		var head = transform;
		foreach(var t in tail) {
			var pos = head.position;
			var diff_pos = pos - t.position;
			var new_r = getAngle(diff_pos.x, diff_pos.y);
			var rad = getRad(diff_pos.x, diff_pos.y);
			var new_x = pos.x - Mathf.Cos(rad) * 2.5f;
			var new_y = pos.y - Mathf.Sin(rad) * 2.5f;
			t.position = new Vector3(new_x, new_y, 0);
			t.rotation = Quaternion.Euler(0, 0, -(90f - new_r));
			head = t;
		}

	}

	float getAngle(float x, float y) {
		float rad = Mathf.Atan2(y, x);
		float degree = rad * Mathf.Rad2Deg;

		if (degree < 0) {
			degree += 360;
		}
		return degree;
	}
	float getRad(float x, float y) {
        return Mathf.Atan2(y, x);
	}
	void Move() {
		if(isDied) {
			return;
		}

		if (ate) {
			AddTail();
			// Reset the flag
			ate = false;
        }
	}

	void AddTail() {
		var pre_pos = transform.position;
		var pre_rot = transform.rotation;
		if (tail.Count > 0) {
			var pre_tail = tail[tail.Count - 1];
			pre_pos = pre_tail.position;
			pre_rot = pre_tail.rotation;
		}

		const float speed = 3f;
		var velocity = pre_rot * new Vector3(0, speed, 0);
		var pos = new Vector3(pre_pos.x - velocity.x, pre_pos.y - velocity.y, 0);

		GameObject g = (GameObject)Instantiate(tailPrefab, pos, pre_rot);
		tail.Add(g.transform);
	}


	void CalculateEnds() {
		float step = 180f / (raysCount - 1);
		for (int i = 0; i < raysCount; i++) {
			var x = Mathf.Cos(i * step * Mathf.Deg2Rad);
			var y = Mathf.Sin(i * step * Mathf.Deg2Rad);
			var dist = transform.rotation * new Vector3(x, y, 0);
			starts[i] = transform.position + dist * 2f;
			ends[i] = transform.position + dist * rayLength;
		}
	}

	//void OnDrawGizmos() {
	//	if (ends == null)
	//		return;
	//	for (int i = 0; i < ends.Length; i++) {
	//		Gizmos.DrawLine(transform.position, ends[i]);
	//	}
	//}

	void OnTriggerEnter2D(Collider2D coll) {
		// Food?
		if (coll.name.StartsWith("Food")) {
			// Get longer in next Move call
			ate = true;

			// Remove the Food
			Destroy(coll.gameObject);
		} else {    // Collided with Tail or Border
			isDied = true;
		}
	}
}
 