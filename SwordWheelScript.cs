using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script Controls a Sword Wheel Hazard, the Swords Spawn in the Center of the Screen in a Circular Pattern Before Launching Outward

public class SwordWheelScript : MonoBehaviour
{

    //Add the Individual Swords to a List

	public GameObject[] swords;
	public float rotationSpeed, startTimer, startTimerMax;
	public bool spinning;
    // Start is called before the first frame update
    void Start()
    {

        //Attaches the Swords to the Wheel

    	startTimer = startTimerMax;
    	foreach (GameObject sword in swords){
    		sword.transform.parent = gameObject.transform;
    	}
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Makes Each of the Swords Appear at a Set Offset

    	startTimer -= Time.deltaTime;
    	if(startTimer <= startTimerMax - .3f){
    		swords[0].GetComponent<Animator>().enabled = true;
    	}
    	if(startTimer <= startTimerMax - .6f){
    		swords[1].GetComponent<Animator>().enabled = true;
    	}
    	if(startTimer <= startTimerMax - .9f){
    		swords[2].GetComponent<Animator>().enabled = true;
    	}
    	if(startTimer <= startTimerMax - 1.2f){
    		swords[3].GetComponent<Animator>().enabled = true;
    	}

        //Adds a Fifth Sword When the Fight Reaches the Halfway Mark
        if (swords.Length >= 5)
        {
            if (startTimer <= startTimerMax - 1.5f)
            {
                swords[4].GetComponent<Animator>().enabled = true;
            }
        }

        //Makes the Wheel Start Spinning After All the Swords Have Spawned

        if (startTimer <= startTimerMax - 1.8f)
        {
    		spinning = true;
    	}


    	if(startTimer <= 0){
    		foreach(GameObject sword in swords){
    			sword.GetComponent<SwordScript>().active = true;
    			sword.transform.parent = null;
    		}
    		Destroy(gameObject);
    	}
        else
        {
    		foreach(GameObject sword in swords){
    			sword.transform.position = gameObject.transform.position;
    		}
    	}


        transform.Rotate(transform.forward, rotationSpeed);
    }
}
