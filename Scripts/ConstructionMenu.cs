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

	//Stores the initial color of the blueprint
	private Color initial_color;

	//TileMap. In this case, the tilemap is acting as a support grid. We wont be placing any tile on him, only using it to get coordenates to place gameobjects
	public Tilemap tm;

	//Tile that will serve to fill a cell on the percolation grid
	public TileBase percolation_obstacle_tilebase;

	//offset used to ajust the ajust the blueprint to the grid
	private float offset;

	//Called by the construction button on UI
	public void Create_Blueprint(GameObject building){
		//Turning it true so we can execute the proper function on update
		place_blueprint = true;

		mouse_to_screen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouse_to_screen.z = 0;

		//Creating one gameobject that will serve as the blueprint that will follow the mouse
		blueprint = Instantiate(building, mouse_to_screen, Quaternion.identity, this.transform);
		initial_color = blueprint.transform.GetComponent<SpriteRenderer>().color; 

		building_to_be_raised = building;
	}

	void Start(){
		offset = .4f;
		place_blueprint = false;
	}

	void Set_Building_Location(GameObject building){
		//Moves the blueprint along with the mouse
		//Determine where to place the gameobject and place it

		mouse_to_screen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouse_to_screen.z = 0;

		Vector3 blueprint_pos = tm.GetCellCenterWorld(tm.LocalToCell(mouse_to_screen));

		// this offset raises the gameobject position so it fits perfectly the grid
		blueprint_pos.y += offset;

		//Moving the blueprint
		blueprint.transform.position = blueprint_pos;

		if(tm.HasTile(tm.WorldToCell(mouse_to_screen))){
			blueprint.transform.GetComponent<SpriteRenderer>().color = Color.red; 
		}else{
			blueprint.transform.GetComponent<SpriteRenderer>().color = initial_color;
			if(Input.GetMouseButtonDown(0)){

				//Here we are forcing the construction to be placed a little bit higher than the mouse coordenates. The purpose of this is to ensure that the
				//blueprint instace will always be above
				blueprint_pos.y += .0001f;
				Instantiate(building, blueprint_pos, Quaternion.identity, this.transform);
				tm.SetTile(tm.WorldToCell(mouse_to_screen), percolation_obstacle_tilebase);

				if(!Input.GetKey(KeyCode.LeftShift)){
					Destroy(blueprint);
					place_blueprint = false;
				}
			}
		}

		if(Input.GetMouseButtonDown(1)){
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
