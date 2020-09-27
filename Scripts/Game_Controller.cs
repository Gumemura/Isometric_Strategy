using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Game_Controller : MonoBehaviour
{
	//CONSTRUCTION
	//FOREST RAISING

	//TileMap. In this case, the tilemap is acting as a support grid. We will use it only using it to get coordenates to place gameobjects
	//Also, this tilemap is used to estabilsh pathfinding. Each cell with a tilebase is a obstacle
	//CONSTRUCTION
	//FOREST RAISING
	public Tilemap tilemapPercolation;

	//Used to set tilemap limits
	public Tilemap tilemapFloor;

	//CONSTRUCTION
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

		//Tile that will serve to fill a cell on the percolation grid
		public TileBase percolation_obstacle_tilebase;

		//Called by the construction button on UI
		public void Create_Blueprint(GameObject building){
			//Turning it true so we can execute the proper function on update
			place_blueprint = true;

			mouse_to_screen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouse_to_screen.z = 0;

			if(blueprint){
				Destroy(blueprint);
			}

			//Creating one gameobject that will serve as the blueprint that will follow the mouse
			blueprint = Instantiate(building, mouse_to_screen, Quaternion.identity, this.transform);
			initial_color = blueprint.transform.GetComponent<SpriteRenderer>().color; 

			building_to_be_raised = building;
		}

		void Set_Building_Location(GameObject building){
			//Moves the blueprint along with the mouse
			//Determine where to place the gameobject and place it

			mouse_to_screen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouse_to_screen.z = 0;

			Vector3 blueprint_pos = tilemapPercolation.GetCellCenterWorld(tilemapPercolation.LocalToCell(mouse_to_screen));

			if(tilemapFloor.HasTile(tilemapPercolation.WorldToCell(mouse_to_screen))){
				//Moving the blueprint
				blueprint.transform.position = blueprint_pos;

				if(tilemapPercolation.HasTile(tilemapPercolation.WorldToCell(mouse_to_screen))){
					blueprint.transform.GetComponent<SpriteRenderer>().color = Color.red; 
				}else{
					blueprint.transform.GetComponent<SpriteRenderer>().color = initial_color;
					if(Input.GetMouseButtonDown(0)){
						//Here we are forcing the construction to be placed a little bit higher than the mouse coordenates. The purpose of this is to ensure that the
						//blueprint instace will always be above
						blueprint_pos.y += .0001f;

						Instantiate(building, blueprint_pos, Quaternion.identity, this.transform);
						tilemapPercolation.SetTile(tilemapPercolation.WorldToCell(mouse_to_screen), percolation_obstacle_tilebase);

						if(!Input.GetKey(KeyCode.LeftShift)){
							Destroy(blueprint);
							place_blueprint = false;
						}
					}
				}
			}


			if(Input.GetMouseButtonDown(1)){
				Destroy(blueprint);
				place_blueprint = false;
			}
		}

	//FOREST RAISING
		//Tile that will serve to fill a forest cell on the percolation grid
		public TileBase percolation_forest_tilebase;

		public GameObject[] forests;
		public GameObject grass;
		public GameObject stone;

		void Map_Constructor(){
			int randomNum;
			Vector3 cell_pos;
			foreach (var position in tilemapPercolation.cellBounds.allPositionsWithin)
			{
				if(tilemapPercolation.GetTile(position) == percolation_forest_tilebase){
					cell_pos = tilemapPercolation.CellToWorld(position);
					randomNum = Random.Range(0, 5);
					Instantiate(forests[randomNum], cell_pos, Quaternion.identity, this.transform);
				}
			}
		}

	void Start(){
		Map_Constructor();
		place_blueprint = false;
	}	

	void Update(){
		if (place_blueprint){
			Set_Building_Location(building_to_be_raised);
		}
	}
}
