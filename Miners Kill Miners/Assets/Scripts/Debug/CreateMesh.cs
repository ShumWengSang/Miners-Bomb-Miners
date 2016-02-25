using UnityEngine;
using System.Collections;

public class CreateMesh : MonoBehaviour {
    public int speed = 5;
	// Use this for initialization
	void Start () {
	
	}
    public GameObject objectToCreate;
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButtonUp(0))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            {
                RaycastHit hit;

                if(Physics.Raycast(cameraRay,out hit))
                {
                    GameObject created = Instantiate(objectToCreate, hit.point, Quaternion.identity) as GameObject;
                    created.transform.position = new Vector3(created.transform.position.x, created.transform.position.y,objectToCreate.transform.position.z);
                }
            }
        }
        else if(Input.GetMouseButtonUp(1))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            {
                RaycastHit hit;

                if (Physics.Raycast(cameraRay, out hit))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        objectToCreate.transform.position += move * speed * Time.deltaTime;

	}
}
