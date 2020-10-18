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

	Vector2 camPos;
	Vector3 vecZero;

	void Start(){
		CameraMovement_Limits();
		cam = transform.GetComponent<Camera>();
	}

	void Update () {
		CameraDiagonalMovement();
		CameraZoom();
	}

	//DIAGONAL MOVEMENT, JUST LIKE A ISOMETRIC GAME'S CAMERA SHOULD BE
	bool CheckCameraPosition(Vector2 p1, Vector2 p2, Vector2 pos){
		//This is what this function do:
		//find the linear equation for the line formed by 'p1' and 'p2' coordinates
		//insert 'pos' coordinate on the equation nad stores the result in 'ind'
		//if 'ind' is positive, it means that 'pos' is above the line
		//if its negative, its above
		//if its zero, its on the line
		//resuming, this funtion tells if 'pos' is above or below a line
		float a = (p1.y - p2.y) / (p1.x - p2.x);
		float ind = pos.y - p2.y - (a * (pos.x - p2.x));

		return ind >= 0;
	}

	int count = 0;
	void CameraDiagonalMovement(){
		if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0){
			camPos = transform.position;

			//(CheckCameraPosition(homePlate, firstBase, camPos) && !CheckCameraPosition(firstBase, secondBase, camPos) && !CheckCameraPosition(secondBase, thirdBase, camPos) && CheckCameraPosition(thirdBase, homePlate, camPos)){
			vecZero.x = Input.GetAxisRaw("Horizontal");
			vecZero.y = Input.GetAxisRaw("Vertical");

			if(!CheckCameraPosition(homePlate, firstBase, camPos)){
				if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") < 0){
					count++;
				}
				if(vecZero.y < 0){
					vecZero.x = vecZero.y * .666f;
					vecZero.y *= .333f;
				}
				if(vecZero.x > 0){
					vecZero.y = vecZero.x * .333f;
					vecZero.x *= .666f;
				}
			}else if(CheckCameraPosition(secondBase, thirdBase, camPos)){
				if(Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") > 0){
					count++;
				}
				if(vecZero.y > 0){
					vecZero.x = vecZero.y * .666f;
					vecZero.y *= .333f;
				}
				if(vecZero.x < 0){
					vecZero.y = vecZero.x * .333f;
					vecZero.x *= .666f;
				}
			}

			if(CheckCameraPosition(firstBase, secondBase, camPos)){
				if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0){
					count++;
				}
				if(vecZero.y > 0){
					vecZero.x = vecZero.y * .666f * -1;
					vecZero.y *= .333f;
				}
				if(vecZero.x > 0){
					vecZero.y = vecZero.x * .333f * -1;
					vecZero.x *= .666f;
				}
			}else if(!CheckCameraPosition(thirdBase, homePlate, camPos)){
				if(Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0){
					count++;
				}
				if(vecZero.y < 0){
					vecZero.x = vecZero.y * .666f * -1;
					vecZero.y *= .333f;
				}
				if(vecZero.x < 0){
					vecZero.y = vecZero.x * .333f * -1;
					vecZero.x *= .666f;
				}
			}

			if(count != 2){
				transform.position += vecZero * velocity * Time.deltaTime;
			}
			vecZero = Vector3.zero;
			count = 0;
		}
	}



	void CameraMovement_Limits(){
		//CAMERA LIMITATION ALONG MAP EDGE
		Vector3 gridOrigin = tilemapFloor.GetCellCenterWorld(tilemapFloor.origin);
		float gridHypotenuse = ((tilemapFloor.size.x - 1) * tilemapFloor.cellSize.y);

		homePlate = gridOrigin + new Vector3(0, 0, -10);
		firstBase = gridOrigin + new Vector3(gridHypotenuse, gridHypotenuse/2, -10);
		secondBase = gridOrigin + new Vector3(0, gridHypotenuse, -10);
		thirdBase = gridOrigin + new Vector3(-gridHypotenuse, gridHypotenuse/2, -10);
	}

	public int[] velocidadesZoom = new int[4];
	int zoomVelocidadeIndex = 1;
	void CameraZoom(){
		if(Input.GetAxis("Mouse ScrollWheel") != 0){
			if (Input.GetAxis("Mouse ScrollWheel") > 0 && cam.orthographicSize > cameraNoZoom - cameraZoom){
				cam.orthographicSize -= cameraZoom;
				zoomVelocidadeIndex--;
			}else if (Input.GetAxis("Mouse ScrollWheel") < 0 && cam.orthographicSize < cameraNoZoom + cameraZoom){
				cam.orthographicSize += cameraZoom;
				zoomVelocidadeIndex++;
			}

			velocity = velocidadesZoom[zoomVelocidadeIndex];
		}
	}
}