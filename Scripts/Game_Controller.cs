using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Game_Controller : MonoBehaviour
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

		limitLine = transform.gameObject.GetComponent<LineRenderer>();

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
		dummyGradient = new Gradient();
		Vector3[] limitCorners = {homePlate, firstBase, secondBase, thirdBase};
		limitLine.SetPositions(limitCorners);
		limitLine.loop = true;

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

		limitLine.colorGradient = emptyGrad;
		dummyGradient = emptyGrad;
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
		private LineRenderer limitLine;

		//"Turns off" the line (just set its gradient to zero)
		private Gradient emptyGrad;
		private Gradient dummyGradient;

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

		bool CheckIfInGameArea(Vector3 position){
			//Check if building position is in game area

			if(!CheckLinearPosition(homePlate, firstBase, position)){
				return false;
			}else if(CheckLinearPosition(firstBase, secondBase, position)){
				return false;
			}else if(CheckLinearPosition(secondBase, thirdBase, position)){
				return false;
			}else if(!CheckLinearPosition(thirdBase, homePlate, position)){
				return false;
			}

			return true;
		}

		void MoveBlueprint(GameObject blueprint){
			mouse_to_screen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouse_to_screen.z = 0;

			Vector3Int grid_coord = tilemapPercolation.WorldToCell(mouse_to_screen);
			Vector3Int intTo3 = Vector3Int.zero;

			if(grid_coord.x > tilemapPercolation.origin.x + tilemapPercolation.size.x - 1){
				intTo3.x = tilemapPercolation.origin.x + tilemapPercolation.size.x - 1;
			}else if(grid_coord.x < tilemapPercolation.origin.x){
				intTo3.x = tilemapPercolation.origin.x;
			}else{
				intTo3.x = grid_coord.x;
			}

			if(grid_coord.y > tilemapPercolation.origin.y + tilemapPercolation.size.y - 1){
				intTo3.y = tilemapPercolation.origin.y + tilemapPercolation.size.y - 1;
			}else if(grid_coord.y < tilemapPercolation.origin.y){
				intTo3.y = tilemapPercolation.origin.y;
			}else{
				intTo3.y = grid_coord.y;
			}

			blueprint.transform.position = tilemapPercolation.GetCellCenterWorld(intTo3);
		}

		void Set_Building_Location(GameObject building){
			//Moves the blueprint along with the mouse
			//Determine where to place the gameobject and place it
			MoveBlueprint(blueprint);

			if(!CheckIfInGameArea(mouse_to_screen)){
				blueprint.transform.GetComponent<SpriteRenderer>().color = Color.red;
				Tilt_Gradient();
			}else{
				blueprint.transform.GetComponent<SpriteRenderer>().color = initial_color;
				LineFade();
				if(tilemapPercolation.HasTile(tilemapPercolation.WorldToCell(mouse_to_screen))){
					blueprint.transform.GetComponent<SpriteRenderer>().color = Color.red; 
				}else{
					blueprint.transform.GetComponent<SpriteRenderer>().color = initial_color;
					if(Input.GetMouseButtonDown(0)){
						//Here we are forcing the construction to be placed a little bit higher than the mouse coordenates. The purpose of this is to ensure that the
						//blueprint instace will always be above
						blueprint.transform.position += new Vector3(0, .0001f, 0);

						Instantiate(building, blueprint.transform.position, Quaternion.identity, transform.Find("Construction"));
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

		//Gradulay fades away the limit line
		void LineFade(){
			GradientColorKey[] colorKey;
			GradientAlphaKey[] alphaKey;
			float lineColorFadeAway = limitLine.colorGradient.alphaKeys[0].alpha;

			colorKey = new GradientColorKey[1];
			alphaKey = new GradientAlphaKey[1];

			if(lineColorFadeAway > 0){
				lineColorFadeAway -= .1f;
			}
			colorKey[0].color = Color.white;
			colorKey[0].time = 0;
			alphaKey[0].alpha = lineColorFadeAway;
			alphaKey[0].time = 0;

			dummyGradient.SetKeys(colorKey, alphaKey);
			limitLine.colorGradient = dummyGradient;
		}

		//Makes the limit line blink
		private float alphaTiltColorLine = 0;
		private bool turnAlphaTiltColorLine = false;
		void Tilt_Gradient(){
			GradientColorKey[] colorKey;
			GradientAlphaKey[] alphaKey;

			colorKey = new GradientColorKey[1];
			alphaKey = new GradientAlphaKey[1];

			colorKey[0].color = Color.white;
			colorKey[0].time = 0;
			alphaKey[0].alpha = alphaTiltColorLine;
			alphaKey[0].time = 0;

			if(!turnAlphaTiltColorLine){
				alphaTiltColorLine -= .1f;
				turnAlphaTiltColorLine = (alphaTiltColorLine < 0);
			}else{
				alphaTiltColorLine += .1f;
				turnAlphaTiltColorLine = (alphaTiltColorLine < 3);
			}

			dummyGradient.SetKeys(colorKey, alphaKey);
			limitLine.colorGradient = dummyGradient;
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

			Vector3 a = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(tilemapFloor.WorldToCell(a).x < tilemapFloor.WorldToCell(thirdBase).x){
				print(tilemapFloor.WorldToCell(a));
				print("passou do limite");
			}



		if (place_blueprint){
			Set_Building_Location(building_to_be_raised);
		}else{
			LineFade();
		}
	}
}
