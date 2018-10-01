using UnityEngine;
using System.Collections;

public class Util : MonoBehaviour {

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            Debug.Log(pos.x + "____" + pos.y);
        }
	}
}
