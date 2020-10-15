using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camera_Control : MonoBehaviour
{
	//Velocity of the camera movemente
	public float velocity;
	public Tilemap tilemapFloor;

	public float cameraZoom;
	public bool diagonalMovement = false; //Debug purpose
	private float cameraNoZoom = 5;
	Camera cam;

	//DIAGONAL LIMITATION
	private Vector2 homePlate;
	private Vector2 firstBase;
	private Vector2 secondBase;
	private Vector2 thirdBase;

	//SQUARE LIMITATION
	private float limit_left;
	private float limit_right;
	private float limit_top;
	private float limit_bottom;

	Vector2 camPos;
	Vector2 vecZero;

	void Start(){
		CameraMovement_Limits();
		cam = transform.GetComponent<Camera>();
	}

	void Update () {
		if(!diagonalMovement){
			CameraMovement();
		}else{
			CameraDiagonalMovement();
		}
		CameraZoom();
	}

	//SQUARE LIMITATION - WORKAROUND BUT WORKING of course its working its easy as hell
		void CameraMovement(){
			if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0){
				vecZero = Vector2.zero;
				camPos = (Vector2)transform.position;

				if((Input.GetAxis("Horizontal") < 0 && transform.position.x > limit_left) || (Input.GetAxis("Horizontal") > 0 && transform.position.x < limit_right)){
					vecZero.x = Input.GetAxis("Horizontal");
				}
				if((Input.GetAxis("Vertical") < 0 && transform.position.y > limit_bottom) || (Input.GetAxis("Vertical") > 0 && transform.position.y < limit_top)){
					vecZero.y = Input.GetAxis("Vertical");
				}

				Vector3 newPos = new Vector3(vecZero.x, vecZero.y, 0);

				transform.position += newPos * velocity * Time.deltaTime;
			}
		}

	//DIAGONAL MOVEMENT, JUST LIKE A ISOMETRIC GAME'S CAMERA SHOULD BE
		Vector3 newPos;
		void CameraDiagonalMovement(){
			vecZero = Vector2.zero;
			camPos = (Vector2)transform.position;
			if(Input.GetAxis("Vertical") < 0){
				if(CheckCameraPosition(homePlate, thirdBase, camPos) && CheckCameraPosition(homePlate, firstBase, camPos)){
					vecZero.y = Input.GetAxis("Vertical");
				}else if(!CheckCameraPosition(homePlate, thirdBase, camPos) && CheckCameraPosition(homePlate, firstBase, camPos)){
					vecZero.x = Input.GetAxis("Vertical") * .66f * -1;
					vecZero.y = Input.GetAxis("Vertical") * .33f;
				}else if(CheckCameraPosition(homePlate, thirdBase, camPos) && !CheckCameraPosition(homePlate, firstBase, camPos)){
					vecZero.x = Input.GetAxis("Vertical") * .66f;
					vecZero.y = Input.GetAxis("Vertical") * .33f;
				}
			}

			if(Input.GetAxis("Vertical") > 0){
				if(!CheckCameraPosition(secondBase, thirdBase, camPos) && !CheckCameraPosition(firstBase, secondBase, camPos)){
					vecZero.y = Input.GetAxis("Vertical");
				}else if(CheckCameraPosition(secondBase, thirdBase, camPos) && !CheckCameraPosition(firstBase, secondBase, camPos)){
					vecZero.x = Input.GetAxis("Vertical") * .66f;
					vecZero.y = Input.GetAxis("Vertical") * .33f;
				}else if(!CheckCameraPosition(secondBase, thirdBase, camPos) && CheckCameraPosition(firstBase, secondBase, camPos)){
					vecZero.x = Input.GetAxis("Vertical") * .66f * -1;
					vecZero.y = Input.GetAxis("Vertical") * .33f;
				}
			}

			if(Input.GetAxis("Horizontal") < 0){
				if(CheckCameraPosition(homePlate, thirdBase, camPos) && !CheckCameraPosition(secondBase, thirdBase, camPos)){
					vecZero.x = Input.GetAxis("Horizontal");
				}else if(!CheckCameraPosition(homePlate, thirdBase, camPos) && !CheckCameraPosition(secondBase, thirdBase, camPos)){
					vecZero.x = Input.GetAxis("Horizontal") * .66f;
					vecZero.y = Input.GetAxis("Horizontal") * .33f * -1;
				}else if(CheckCameraPosition(homePlate, thirdBase, camPos) && CheckCameraPosition(secondBase, thirdBase, camPos)){
					vecZero.x = Input.GetAxis("Horizontal") * .66f;
					vecZero.y = Input.GetAxis("Horizontal") * .33f;
				}
			}

			if(Input.GetAxis("Horizontal") > 0){
				if(CheckCameraPosition(homePlate, firstBase, camPos) && !CheckCameraPosition(firstBase, secondBase, camPos)){
					vecZero.x = Input.GetAxis("Horizontal");
				}else if(CheckCameraPosition(homePlate, firstBase, camPos) && CheckCameraPosition(firstBase, secondBase, camPos)){
					vecZero.x = Input.GetAxis("Horizontal") * .66f;
					vecZero.y = Input.GetAxis("Horizontal") * .33f * -1;
				}else if(!CheckCameraPosition(homePlate, firstBase, camPos) && !CheckCameraPosition(firstBase, secondBase, camPos)){
					vecZero.x = Input.GetAxis("Horizontal") * .66f;
					vecZero.y = Input.GetAxis("Horizontal") * .33f;
				}
			}

			newPos = new Vector3(vecZero.x, vecZero.y, 0);
			transform.position += newPos * velocity * Time.deltaTime;
		}

		bool CheckCameraPosition(Vector2 p1, Vector2 p2, Vector2 pos){
			float a = (p1.y - p2.y) / (p1.x - p2.x);
			float ind = pos.y - p2.y - (a * (pos.x - p2.x));

			return ind >= 0;
		}

	void CameraMovement_Limits(){
		if(!diagonalMovement){
			Vector3 tileOrigin = tilemapFloor.GetCellCenterWorld(tilemapFloor.origin);
			float gridHypotenuse = ((tilemapFloor.size.x - 1) * tilemapFloor.cellSize.y);
			tileOrigin.z = -10;
			transform.position = tileOrigin;

			limit_left = tileOrigin.x - gridHypotenuse;
			limit_right = tileOrigin.x + gridHypotenuse;
			limit_top = tileOrigin.y + gridHypotenuse;
			limit_bottom = tileOrigin.y;
		}else{
			//CAMERA LIMITATION ALONG MAP EDGE
			Vector3 gridOrigin = tilemapFloor.GetCellCenterWorld(tilemapFloor.origin);
			float gridHypotenuse = ((tilemapFloor.size.x - 1) * tilemapFloor.cellSize.y);

			homePlate = gridOrigin + new Vector3(0, 0, -10);
			firstBase = gridOrigin + new Vector3(gridHypotenuse, gridHypotenuse/2, -10);
			secondBase = gridOrigin + new Vector3(0, gridHypotenuse, -10);
			thirdBase = gridOrigin + new Vector3(-gridHypotenuse, gridHypotenuse/2, -10);

			// Debug.DrawLine(homePlate, homePlate + Vector3.down, Color.red, 100f);
			// Debug.DrawLine(firstBase, firstBase + Vector3.right, Color.blue, 100f);
			// Debug.DrawLine(secondBase, secondBase + Vector3.up, Color.green, 100f);
			// Debug.DrawLine(thirdBase, thirdBase + Vector3.left, Color.white, 100f);
		}
	}

	void CameraZoom(){
		if(Input.GetAxis("Mouse ScrollWheel") != 0){
			if (Input.GetAxis("Mouse ScrollWheel") > 0 && cam.orthographicSize > cameraNoZoom - cameraZoom){
				cam.orthographicSize -= cameraZoom;
			}else if (Input.GetAxis("Mouse ScrollWheel") < 0 && cam.orthographicSize < cameraNoZoom + cameraZoom){
				cam.orthographicSize += cameraZoom;
			}
			velocity = Mathf.Pow(cam.orthographicSize, 2);
		}
	}
}