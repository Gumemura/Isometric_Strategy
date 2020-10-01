using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camera_Control_Sim : MonoBehaviour
{
	//Velocity of the camera movemente
	public float velocity;
	public Tilemap tilemapFloor;

	public float cameraZoom;
	private float cameraNoZoom = 5;
	Camera cam;

	//CAMERA LIMITATION ALONG MAP EDGE - NEEDS IMPLEMENTING
		//Limits to the camera movement based on the tilemap size
		// private Vector3 homePlate;
		// private Vector3 firstBase;
		// private Vector3 secondBase;
		// private Vector3 thirdBase;

	private float limit_left;
	private float limit_right;
	private float limit_top;
	private float limit_bottom;

	Vector3 camPos;
	Vector3 vecZero;

	void Start(){
		CameraMovement_Limits();
		cam = transform.GetComponent<Camera>();
	}

	void Update () {
		CameraMovement();
		CameraZoom();
	}

	void CameraMovement(){
		if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0){
			vecZero = Vector3.zero;
			camPos = transform.position;

			if((Input.GetAxis("Horizontal") < 0 && transform.position.x > limit_left) || (Input.GetAxis("Horizontal") > 0 && transform.position.x < limit_right)){
				vecZero.x = Input.GetAxis("Horizontal");
			}
			if((Input.GetAxis("Vertical") < 0 && transform.position.y > limit_bottom) || (Input.GetAxis("Vertical") > 0 && transform.position.y < limit_top)){
				vecZero.y = Input.GetAxis("Vertical");
			}

			//CAMERA LIMITATION ALONG MAP EDGE - NEEDS IMPLEMENTING
				// if(Input.GetAxis("Horizontal") < 0){
				// 	if(CheckCameraPosition(homePlate, thirdBase, camPos) && !CheckCameraPosition(secondBase, thirdBase, camPos)){
				// 		vecZero.x = Input.GetAxis("Horizontal");
				// 	}
				// }else{
				// 	if(CheckCameraPosition(homePlate, firstBase, camPos) && !CheckCameraPosition(firstBase, secondBase, camPos)){
				// 		vecZero.x = Input.GetAxis("Horizontal");
				// 	}
				// }

				// if(Input.GetAxis("Vertical") < 0){
				// 	if(CheckCameraPosition(homePlate, thirdBase, camPos) && CheckCameraPosition(homePlate, firstBase, camPos)){
				// 		vecZero.y = Input.GetAxis("Vertical");
				// 	}
				// }else{
				// 	if(!CheckCameraPosition(secondBase, thirdBase, camPos) && !CheckCameraPosition(firstBase, secondBase, camPos)){
				// 		vecZero.y = Input.GetAxis("Vertical");
				// 	}
				// }

			transform.position += vecZero * velocity * Time.deltaTime;
		}
	}

	// bool CheckCameraPosition(Vector3 p1, Vector3 p2, Vector3 pos){
	// 	float a = (p1.y - p2.y) / (p1.x - p2.x);
	// 	float ind = pos.y - p1.y - (a * (pos.x - p1.x));

	// 	return ind > 0;
	// }

	void CameraMovement_Limits(){
		Vector3 tileOrigin = tilemapFloor.GetCellCenterWorld(tilemapFloor.origin);
		float gridHip = ((tilemapFloor.size.x - 1) * tilemapFloor.cellSize.y);
		tileOrigin.z = -10;
		transform.position = tileOrigin;

		limit_left = tileOrigin.x - gridHip;
		limit_right = tileOrigin.x + gridHip;
		limit_top = tileOrigin.y + gridHip;
		limit_bottom = tileOrigin.y;

		//CAMERA LIMITATION ALONG MAP EDGE - NEEDS IMPLEMENTING
			// Vector3 gridOrigin = tilemapFloor.GetCellCenterWorld(tilemapFloor.origin);
			// float gridHip = ((tilemapFloor.size.x - 1) * tilemapFloor.cellSize.y);

			// homePlate = gridOrigin + new Vector3(0, 0, -10);
			// firstBase = gridOrigin + new Vector3(gridHip, gridHip/2, -10);
			// secondBase = gridOrigin + new Vector3(0, gridHip, -10);
			// thirdBase = gridOrigin + new Vector3(-gridHip, gridHip/2, -10);
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
