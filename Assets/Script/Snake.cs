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

	bool col_ava = true;

	// Tail Prefab
	public GameObject tailPrefab;

	// Current Movement Direction
	// (by default it moves to the right)
	Vector2 dir = Vector2.right;

	// Keep Track of Tail
	List<Transform> tail = new List<Transform>();

	class PP {
		public Quaternion q;
		public Vector3 pos;

		public PP(Quaternion q, Vector3 pos) {
			this.q = q;
			this.pos = pos;
        }
    };
	List<PP> pp_list = new List<PP>();
	PP old_pp;

	int raysCount = 9;
	public float rayLength = 4;
	Vector3[] starts;
	Vector3[] ends;
	Vector3 sp_size;
	const int dist = 15;

	float rot = 0;
	public float mm = 0;
	float s_size = 0;
	//Vector3? velocityold = null;

	// Use this for initialization
	void Start () {
		starts = new Vector3[raysCount];
		ends = new Vector3[raysCount];

		sp_size = GetComponent<SpriteRenderer>().bounds.size;
		s_size = sp_size.magnitude * 1f;
		// Move the Snake every 300ms
		//InvokeRepeating("Move", 0.3f, 0.3f);
    }

	// Update is called once per frame
	void FixedUpdate () {
		if (!isDied) {
			rot = 0f;
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
			float speed = 15;
			//Vector3? velocityold = null;
			//if (rot != 0) {
				var velocityold = transform.rotation * new Vector3(0, 1,0);
			//}
			Vector3 anvec = gameObject.transform.localEulerAngles;
			var b_r = new Quaternion(
				transform.rotation.x, transform.rotation.y, 
				transform.rotation.z, transform.rotation.w);
			var b_p = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			var old_go = transform.rotation * new Vector3(0, speed, 0) * Time.deltaTime;
			transform.Rotate(0, 0, rot*5);
			var nn = transform.rotation;
            if (b_r.Equals(nn)) {
				var mm = 0;
            }
			//var rotatedVector = transform.rotation * Vector3.forward;

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

            if (tail.Count > 0) {
				//tail[0].Rotate(0, 0, za);
				//if (rot != 0) {
				//                Vector3 v2 = tail[0].rotation * new Vector3(0, speed, 0);
				//tail[0].position = b_p;

				//Vector3 v2 = b_r * new Vector3(0, speed, 0);
				//tail[0].position += v2 * Time.deltaTime;
				//	tail[0].position += v2 * Time.deltaTime ;
				//if (ss % 1 == 0) {
				//if (velocityold != null) {
				//	//tail[0].rotation = Quaternion.Euler(0,0,za + transform.localEulerAngles.z);
				//	//Vector3 v2 = tail[0].rotation * new Vector3(0, speed, 0);
				//	//tail[0].position += v2 * Time.deltaTime;
				//	var ddd = b_p - transform.position;
				var dd = b_p - tail[0].position;
				var za = getAngle(dd.x, dd.y);
				//tail[0].rotation = b_r;
				var t0v = tail[0].rotation * new Vector3(0, 1, 0);


				float rad = getRad(dd.x, dd.y);
				//tail[0].position += t0v.normalized * speed * Time.deltaTime;

				var dn0x = Mathf.Cos(rad) * 0.1f;
				var dn0y = Mathf.Sin(rad) * 0.1f;

				var n0x = b_p.x - Mathf.Cos(rad) * 2.5f;
				var n0y = b_p.y - Mathf.Sin(rad) * 2.5f;
				tail[0].position = new Vector3(n0x, n0y, 0);
				//tail[0].position += t0v * speed * mm * Time.deltaTime;
				tail[0].rotation = Quaternion.Euler(0, 0, -za);


                //tail[0].position = b_p - velocityold * Time.deltaTime * 30f;
                //	tail[0].position = b_p;
                //}
                //float sss = za;
                for (int i = 1; i < tail.Count; i++) {
					var b_r2 = tail[i - 1].rotation;
					var b_p2 = tail[i - 1].position;

					var dd2 = b_p2 - tail[i].position;
					var za2 = getAngle(dd2.x, dd2.y);
					//tail[i].rotation = b_r2;
					float rad2 = getRad(dd2.x, dd2.y);

					var dn0x2 = dd2.x * 0.2f;
					var dn0y2 = dd2.y * 0.2f;
					//tail[0].position += t0v.normalized * speed * Time.deltaTime;
					var n0x2 = b_p2.x - Mathf.Cos(rad2) * 2.5f;
					var n0y2 = b_p2.y - Mathf.Sin(rad2) * 2.5f;
					tail[i].position = new Vector3(n0x2, n0y2, 0);
					//tail[i].position += tail[i].rotation * new Vector3(0, speed, 0) * Time.deltaTime;
					tail[i].rotation = Quaternion.Euler(0, 0, -za2);
					//sss += za2;
					//tail[i].rotation = Quaternion.Euler(0, 0, za2);

					//var velocityold2 = tail[i - 1].rotation * new Vector3(0, speed, 0);

					//tail[i].position = b_p2 - velocityold2 * Time.deltaTime * 30f;
				}

                //tail[0].rotation = b_r;
                //tail[0].rotation = Quaternion.Euler(0, 0, za);
                //}
                //for (int i = 0; i < tail.Count; i++) {
                //                var c = pp_list.Count - 1;
                //	tail[i].rotation = pp_list[c - i].q;
                //                tail[i].position = pp_list[c - i].pos;
                //            }
                //           for (int i = 1; i < tail.Count; i++) {
                //tail[i].position = tail[i - 1].position 
                //	- tail[i - 1].rotation * new Vector3(0, speed, 0) * Time.deltaTime * 10f;
                //tail[i].rotation = tail[i-1].rotation;

                //           }
            }
			//ss++;

			//if(ss == 0f) {
			//	old_pp = new PP(transform.rotation, transform.position);
			//}
			//         ss += (velocity * Time.deltaTime).magnitude;
			//         if (ss <= s_size) {
			//	if (tail.Count > 0 && pp_list.Count > 0) {
			//		for (int i = 0; i < pp_list.Count; i++) {
			//			var c = pp_list.Count - 1;
			//			tail[i].rotation = pp_list[c - i].q;
			//			tail[i].position = pp_list[c - i].pos;
			//		}
			//	}
			//             if (ate) {
			//		ate = false;
			//		//col_ava = true;
			//	}
			//	return;

			//         } else {
			//	ss = 0f;
			//}

			//pp_list.Add(new PP(transform.rotation, transform.position));
			//pp_list.Add(old_pp);
			//if (tail.Count > 0) {
			//	for (int i = 0; i < tail.Count; i++) {
			//		var c = pp_list.Count-1;
			//		tail[i].rotation = pp_list[c-i].q;
			//                 tail[i].position = pp_list[c-i].pos;
			//             }
			//}
			//if (pp_list.Count > tail.Count) {
			//	pp_list.RemoveAt(0);
			//}
			//if (ate) {
			//	ate = false;

			//}
			//col_ava = true;

		} else {
			if (Input.GetKey(KeyCode.R)){
				//clear the tail
				tail.Clear();
				pp_list.Clear();

				//reset to origin
				transform.position = new Vector3(0, 0, 0);

				//make snake alive
				isDied = false;
			}
		}
	}

	float getAngle(float x, float y) {
        //return Mathf.Atan2(y, x);
        //return ((float)(Mathf.Atan2(x, y) * 180 / Mathf.PI));

		float rad = Mathf.Atan2(x, y);
		float degree = rad * Mathf.Rad2Deg;

		if (degree < 0) {
			degree += 360;
		}
		return degree;
	}
	float getRad(float x, float y) {
        return Mathf.Atan2(y, x);
        //return ((float)(Mathf.Atan2(y, x) * 180 / Mathf.PI));
	}
	void Move() {
		if (!isDied) {
			// Save current position (gap will be here)
			//Vector2 v = transform.position;

			// Move head into new direction (now there is a gap)
			//transform.Translate (dir);

			// Ate something? Then insert new Element into gap


			if (ate) {
				col_ava = false;
				//if (pp_list.Count < ((tail.Count+1) * dist+15)) {
				//    return;
				//}
				//if (pp_list.Count < (tail.Count + 1)) {
				//	return;
				//}
				//transform.Rotate(0, 0, rot);
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
				//gameObject.transform.position += velocity*0.1f;// * Time.deltaTime;

				// Load Prefab into the world
				GameObject g = (GameObject)Instantiate (tailPrefab,
								  v2,
								  qq);
				//g.GetInstanceID
				if(tail.Count > 0) {
					var mm = 0;
                }
                // Keep track of it in our tail list
                //tail.Insert(0, g.transform);
                tail.Add(g.transform);
                // Reset the flag
                ate = false;
                //col_ava = true;

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
            if (col_ava) {
				//isDied = true;
			}
		}
	}
}
 