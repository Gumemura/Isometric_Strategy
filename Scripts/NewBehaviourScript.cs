using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NewBehaviourScript : MonoBehaviour
{
	public Tilemap tm;
	Vector3 mouse_to_screen;

	// Update is called once per frame
	void Update()
	{
		mouse_to_screen = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if(Input.GetMouseButtonDown(0)){
			print(tm.HasTile(tm.WorldToCell(mouse_to_screen)));
		}	
	}
}
