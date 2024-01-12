using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Script : MonoBehaviour
{
    public GameObject Main_Character;
    // Start is called before the first frame update
    void Start()
    {
        Main_Character = GameObject.Find("Main_Character"); // The player
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Main_Character.transform.position.x, Main_Character.transform.position.y + 2, Main_Character.transform.position.z - 10);
    }
}
