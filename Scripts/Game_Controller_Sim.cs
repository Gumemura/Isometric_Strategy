using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Game_Controller_Sim : MonoBehaviour
{
	//T.O.C.:
	//CONSTRUCTION
	//FOREST RAISING

	//TileMap. In this case, the tilemap is acting as a support grid. We will use it only using it to get coordenates to place gameobjects
	//Also, this tilemap is used to estabilsh pathfinding. Each cell with a tilebase is a obstacle
	//Used in CONSTRUCTION; MAP CONSTRUCTOR
	public Tilemap tilemapPercolation;

	//Used to set tilemap limits
	//Used in CONSTRUCTION; MAP CONSTRUCTOR
	public Tilemap tilemapFloor;

	//Converts the grid on a matrix with the center of every cell as a Vector3Int
	//Used in CONSTRUCTION; MAP CONSTRUCTOR
	private BoundsInt bound;

	//Test purpose only
	public bool debugPrintBalls = false;
	public bool debugPrintBallsCorner = false;
	public GameObject debugBallOrange;
	public GameObject debugBallGreen;

	void Grid_Inicializator(){
		tilemapPercolation.origin = tilemapFloor.origin;
		tilemapPercolation.size = tilemapFloor.size;
		tilemapFloor.CompressBounds();
		tilemapPercolation.CompressBounds();

		bound = tilemapFloor.cellBounds;

		constructLimit = transform.gameObject.GetComponent<LineRenderer>();

		//Getting corners
		Vector3 gridOrigin = tilemapFloor.GetCellCenterWorld(tilemapFloor.origin);
		float gridHip = ((tilemapFloor.size.x - 1) * tilemapFloor.cellSize.y);
		float limitAjustY = tilemapFloor.cellSize.y * (limit - .5f);
		float limitAjustX = tilemapFloor.cellSize.x * (limit - .5f);


		homePlate = gridOrigin + new Vector3(0 , limitAjustY, 0);
		firstBase = gridOrigin + new Vector3(gridHip, gridHip/2, 0) - new Vector3(limitAjustX, 0, 0);
		secondBase = gridOrigin + new Vector3(0, gridHip, 0) - new Vector3(0, limitAjustY, 0);
		thirdBase = gridOrigin + new Vector3(-gridHip, gridHip/2, 0) + new Vector3(limitAjustX, 0, 0);

		if(debugPrintBallsCorner){
			Instantiate(debugBallOrange, homePlate, Quaternion.identity, transform.Find("DEBUG"));
			Instantiate(debugBallGreen, firstBase, Quaternion.identity, transform.Find("DEBUG"));
			Instantiate(debugBallOrange, secondBase, Quaternion.identity, transform.Find("DEBUG"));
			Instantiate(debugBallGreen, thirdBase, Quaternion.identity, transform.Find("DEBUG"));
		}
	}

	void Line_Inicializator(){
		//Setting corners 
		Vector3[] limitCorners = {homePlate, firstBase, secondBase, thirdBase};
		constructLimit.SetPositions(limitCorners);
		constructLimit.loop = true;

		//Creating a "empty" gradient to make line invisible
		emptyGrad = new Gradient();
		GradientColorKey[] colorKey;
		GradientAlphaKey[] alphaKey;

		colorKey = new GradientColorKey[1];
		alphaKey = new GradientAlphaKey[1];

		colorKey[0].color = Color.white;
		colorKey[0].time = 0;
		alphaKey[0].alpha = 0;
		alphaKey[0].time = 0;

		emptyGrad.SetKeys(colorKey, alphaKey);

		constructLimit.colorGradient = emptyGrad;
	}

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

		//Grid corners
		private Vector3 homePlate;
		private Vector3 firstBase;
		private Vector3 secondBase;
		private Vector3 thirdBase;

		//Draws a line that represents the limit of the area where it can be build
		private LineRenderer constructLimit;

		//"Turns off" the line (just set its gradient to zero)
		private Gradient emptyGrad;
		private float grad_alpha = 0;
		private bool turn = true;

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
			blueprint = Instantiate(building, mouse_to_screen, Quaternion.identity, transform.Find("Construction"));
			initial_color = blueprint.transform.GetComponent<SpriteRenderer>().color; 

			building_to_be_raised = building;
		}

		bool CheckLinearPosition(Vector3 p1, Vector3 p2, Vector3 pos){
			//Receive two point (p1 and p2), draw a line between them and check it pos is above (+) or under the line(-)
			float a = (p1.y - p2.y) / (p1.x - p2.x);
			float ind = pos.y - p1.y - (a * (pos.x - p1.x));

			return ind > 0;
		}

		bool CheckIfInGameArea(GameObject building){
			//Check if building position is in game area
			Vector3 goPos = building.transform.position;

			if(!CheckLinearPosition(homePlate, firstBase, goPos)){
				return false;
			}else if(CheckLinearPosition(firstBase, secondBase, goPos)){
				return false;
			}else if(CheckLinearPosition(secondBase, thirdBase, goPos)){
				return false;
			}else if(!CheckLinearPosition(thirdBase, homePlate, goPos)){
				return false;
			}

			return true;
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
				constructLimit.colorGradient = emptyGrad;

				if(CheckIfInGameArea(blueprint)){
					if(tilemapPercolation.HasTile(tilemapPercolation.WorldToCell(mouse_to_screen))){
						blueprint.transform.GetComponent<SpriteRenderer>().color = Color.red; 
					}else{
						blueprint.transform.GetComponent<SpriteRenderer>().color = initial_color;
						if(Input.GetMouseButtonDown(0)){
							//Here we are forcing the construction to be placed a little bit higher than the mouse coordenates. The purpose of this is to ensure that the
							//blueprint instace will always be above
							blueprint_pos.y += .0001f;

							Instantiate(building, blueprint_pos, Quaternion.identity, transform.Find("Construction"));
							tilemapPercolation.SetTile(tilemapPercolation.WorldToCell(mouse_to_screen), percolation_obstacle_tilebase);

							if(!Input.GetKey(KeyCode.LeftShift)){
								Destroy(blueprint);
								place_blueprint = false;
							}
						}
					}
				}else{
					blueprint.transform.GetComponent<SpriteRenderer>().color = Color.red;
					Tilt_Gradient();
				}
			}

			if(Input.GetMouseButtonDown(1)){
				blueprint.transform.GetComponent<SpriteRenderer>().color = Color.red;
				Destroy(blueprint);
				place_blueprint = false;
			}
		}

		void BlueprintPositionHyp(){

		}

		void Tilt_Gradient(){
			Gradient gradient = new Gradient();
			GradientColorKey[] colorKey;
			GradientAlphaKey[] alphaKey;

			colorKey = new GradientColorKey[1];
			alphaKey = new GradientAlphaKey[1];

			colorKey[0].color = Color.white;
			colorKey[0].time = 0;
			alphaKey[0].alpha = grad_alpha;
			alphaKey[0].time = 0;

			if(!turn){
				grad_alpha -= .1f;
				turn = (grad_alpha < 0);
			}else{
				grad_alpha += .1f;
				turn = (grad_alpha < 3);
			}

			gradient.SetKeys(colorKey, alphaKey);

			constructLimit.colorGradient = gradient;
		}

	//MAP CONSTRUCTOR
		//Tile that will serve to fill a forest cell on the percolation grid
		public TileBase percolation_forest_tilebase;

		//Defines the border of the grid where it will not be possible to raise builds
		public int limit;

		public GameObject[] forests;
		public GameObject grass;
		public GameObject stone;

		void Map_Constructor(){
			int randomNum;
			Vector3 cell_pos;

			foreach (var position in bound.allPositionsWithin)
			{
				cell_pos = tilemapFloor.GetCellCenterWorld(position);

				//Spawning forests
				if(tilemapPercolation.GetTile(position) == percolation_forest_tilebase){;
					randomNum = Random.Range(0, 5);
					Instantiate(forests[randomNum], cell_pos, Quaternion.identity, this.transform.Find("Trees"));
				}

				//Debug purpose only
				if(debugPrintBalls){
					if((position.x >= bound.xMax - limit || position.x < bound.xMin + limit) || (position.y >= bound.yMax - limit || position.y < bound.yMin + limit)){
						Instantiate(debugBallOrange, cell_pos, Quaternion.identity, this.transform.Find("DEBUG"));
					}else{
						Instantiate(debugBallGreen, cell_pos, Quaternion.identity, this.transform.Find("DEBUG"));
					}
				}
				
				//Spawning decoration (grass and stone)
				if(tilemapFloor.HasTile(position)){
					randomNum = Random.Range(0, 21);

					if(randomNum == 1){
						Instantiate(stone, cell_pos, Quaternion.identity, this.transform.Find("Decoration"));
					}else if(randomNum == 2 || randomNum == 3){
						Instantiate(grass, cell_pos, Quaternion.identity, this.transform.Find("Decoration"));
					}

				}
			}
		}

	void Awake(){
		Grid_Inicializator();
	}

	void Start(){
		Map_Constructor();
		Line_Inicializator();
		place_blueprint = false;
	}	

	void Update(){
		if (place_blueprint){
			Set_Building_Location(building_to_be_raised);
		}
	}
}
