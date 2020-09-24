using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConstructionMenu : MonoBehaviour
{
	//Boolean that will inform if it can instantiate the blueprint
	private bool place_blueprint;

	//Gameobject instance that will serve as a blueprint: it will follow the mouse and serve as a visual parameter
	private GameObject blueprint;

	//Building to be raised. Will be passed as a paremeter trought a UI's button
	private GameObject building_to_be_raised;

	//Vector3 that will store the mouse position covnerted to screen position
	private Vector3 mouse_to_screen;

	//TileMap. In this case, the tilemap is acting as a support grid. We wont be placing any tile on him, only using it to get coordenates to place gameobjects
	public Tilemap tm;

	//offset used to ajust the ajust the blueprint to the grid
	private float offset;

	//Called by the construction button on UI
	public void Create_Blueprint(GameObject building){
		//Turning it true so we can execute the proper function on update
		place_blueprint = true;

		//Creating one gameobject that will serve as the blueprint that will follow the mouse
		blueprint = Instantiate(building, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Quaternion.identity, this.transform);

		building_to_be_raised = building;
	}

	void Start(){
		offset = .41f;
		place_blueprint = false;
	}

	void Set_Building_Location(GameObject building){
		//Moves the blueprint along with the mouse
		//Determine where to place the gameobject and place it

		mouse_to_screen = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

		Vector3 blueprint_pos = tm.GetCellCenterWorld(tm.LocalToCell(mouse_to_screen));
		blueprint_pos.y += offset;

		blueprint.transform.position = blueprint_pos;

		if(Input.GetMouseButton(0)){
			Instantiate(building, blueprint_pos, Quaternion.identity, this.transform);

			if(!Input.GetKey(KeyCode.LeftShift)){
				Destroy(blueprint);
				place_blueprint = false;
			}
		}else if(Input.GetMouseButtonDown(1)){
			Destroy(blueprint);
			place_blueprint = false;
		}
	}

	void Update(){
		if (place_blueprint){
			Set_Building_Location(building_to_be_raised);
		}
	}
}
