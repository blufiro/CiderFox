using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour
{
     
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	public Rect bounds;
 
	// Update is called once per frame
	void Update()
	{
		if (target) {
			Vector3 point = Camera.main.WorldToViewportPoint(target.position);
			Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}

		if (bounds.width > 0 && bounds.height > 0) {
			transform.position = new Vector3(
				Mathf.Clamp(transform.position.x, bounds.xMin, bounds.xMax),
				Mathf.Clamp(transform.position.y, bounds.yMin, bounds.yMax),
				transform.position.z);
		}
	}
}