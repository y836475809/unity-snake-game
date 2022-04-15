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

	//bool col_ava = true;

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
		// Move the Snake every 300ms
		//InvokeRepeating("Move", 0.3f, 0.3f);
    }

	// Update is called once per frame
	void FixedUpdate () {
		if (!isDied) {
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
			for (int i = 0; i < tail.Count; i++) {
				var pos = head.position;
				var diff_pos = pos - tail[i].position;
				var new_r = getAngle(diff_pos.x, diff_pos.y);
				var rad = getRad(diff_pos.x, diff_pos.y);
				var new_x = pos.x - Mathf.Cos(rad) * 2.5f;
				var new_y = pos.y - Mathf.Sin(rad) * 2.5f;
				tail[i].position = new Vector3(new_x, new_y, 0);
				tail[i].rotation = Quaternion.Euler(0, 0, -(90f - new_r));
				head = tail[i];
			}
		} else {
			if (Input.GetKey(KeyCode.R)){
				//clear the tail
				foreach(var t in tail) {
					Destroy(t.gameObject);
				}
				tail.Clear();

				//reset to origin
				transform.position = new Vector3(0, 0, 0);
				transform.rotation = Quaternion.Euler(0f, 0f, 0f);

				//make snake alive
				isDied = false;
			}
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
		if (!isDied) {
			if (ate) {
				//col_ava = false;
				Vector2 v = transform.position;
				var qq = transform.rotation;
				float speed = 3f;
				Vector3 velocity = transform.rotation * new Vector3(0, speed, 0);
				Vector2 v2 = new Vector2(v.x - velocity.x, v.y - velocity.y);
                if (tail.Count > 0) {
					var ll = tail[tail.Count - 1];
					v = ll.position;
					qq = ll.rotation;
					velocity = ll.rotation * new Vector3(0, speed, 0);
					v2 = new Vector2(v.x - velocity.x, v.y - velocity.y);
				}
				// Load Prefab into the world
				GameObject g = (GameObject)Instantiate (tailPrefab,
								  v2,
								  qq);
                tail.Add(g.transform);
                // Reset the flag
                ate = false;
            } else if (tail.Count > 0) {	// Do we have a Tail?
					//// Move last Tail Element to where the Head was
					//tail.Last ().position = v;

					//// Add to front of list, remove from the back
					//tail.Insert (0, tail.Last ());
					//tail.RemoveAt (tail.Count - 1);
			}
        }
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
			//if (col_ava) {
			//	//isDied = true;
			//}
		}
	}
}
 