using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script For the Individual Swords That Make Up the Sword Wheel and Sword Wall Hazards 

public class SwordScript : MonoBehaviour
{
	public Rigidbody2D rb;
	public float moveSpeed, lifeTime;
	public bool active, spinning;
    // Start is called before the first frame update
    void Start()
    {
        lifeTime = 10;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(active == true){

            //Launches the Sword Forward When They Are Activated by the Sword Wheel and Wall Script

        	rb.AddForce(-transform.up * moveSpeed);
        	lifeTime -= Time.deltaTime;

            //Deletes the Swords After a Set Time When They Are Offscreen

        	if(lifeTime <= 0){
        		Destroy(gameObject);
        	}
        }
    }

    void Activate(){
        if(spinning == false){
    	   active = true;
        }
    }
}
