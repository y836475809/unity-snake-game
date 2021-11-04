using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

	class PP {
		public Quaternion q;
		public Vector3 pos;

		public PP(Quaternion q, Vector3 pos) {
			this.q = q;
			this.pos = pos;
        }
    };
	List<PP> pp_list = new List<PP>();

	int raysCount = 9;
	public float rayLength = 4;
	Vector3[] starts;
	Vector3[] ends;
	Vector3 sp_size;
	const int dist = 15;

	float rot = 0;
	float ss = 0;
	float s_size = 0;


	// Use this for initialization
	void Start () {
		starts = new Vector3[raysCount];
		ends = new Vector3[raysCount];

		sp_size = GetComponent<SpriteRenderer>().bounds.size;
		s_size = sp_size.sqrMagnitude;
		// Move the Snake every 300ms
		InvokeRepeating("Move", 0.3f, 0.3f);
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

			transform.Rotate(0, 0, rot*10);
			float speed = 8;
            Vector3 velocity = transform.rotation * new Vector3(0, speed, 0);
			transform.position += velocity * Time.deltaTime;

			CalculateEnds();

			for (int i = 0; i < raysCount; i++) {
				var hit = Physics2D.Linecast(starts[i], ends[i]);
				if (hit.collider != null) {
					Debug.DrawLine(starts[i], ends[i], Color.red);
                } else {
					Debug.DrawLine(starts[i], ends[i], Color.white);
				}
			}

			//ss += (velocity * Time.deltaTime).sqrMagnitude;
			//if (ss <= s_size) {
			//	return;
			//}

			pp_list.Add(new PP(transform.rotation, transform.position));
			var kk = dist;
			if (tail.Count > 0 && pp_list.Count > dist) {
                for (int i = 0; i < tail.Count; i++) {
					var ii = (pp_list.Count - i * kk) -1;
                    if (i == 0) {
						ii = (pp_list.Count - i * kk) - 15;
					}
					tail[i].rotation = pp_list[ii].q;
                    tail[i].position = pp_list[ii].pos;
                }
            }
			
            if (pp_list.Count > dist * (tail.Count+1)) {
                pp_list.RemoveAt(0);
				//tail[0].GetComponent<BoxCollider2D>().isTrigger = true;

			}

			//Move();


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

	void Move() {
		if (!isDied) {
			// Save current position (gap will be here)
			//Vector2 v = transform.position;

			// Move head into new direction (now there is a gap)
			//transform.Translate (dir);

			// Ate something? Then insert new Element into gap


			if (ate) {
                if (pp_list.Count < ((tail.Count+1) * dist+15)) {
                    return;
                }
                //transform.Rotate(0, 0, rot);
                Vector2 v = transform.position;
				float speed = 1f;
				Vector3 velocity = transform.rotation * new Vector3(0, speed, 0);


				gameObject.transform.position += velocity;// * Time.deltaTime;

				// Load Prefab into the world
				GameObject g = (GameObject)Instantiate (tailPrefab,
					              v,
					              Quaternion.identity);
				//g.GetInstanceID
				if(tail.Count > 0) {
					var mm = 0;
                }
                // Keep track of it in our tail list
                tail.Insert(0, g.transform);
                //tail.Add(g.transform);
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
		} else { 	// Collided with Tail or Border
			isDied = true;
		}
	}
}
 