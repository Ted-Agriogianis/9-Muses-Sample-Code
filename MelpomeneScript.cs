using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script Interacts With the MuseScript, It Contains All the Unique Elements For This Particular Boss, Such As Attacks and Aesthetic Changes.

public class MelpomeneScript : MonoBehaviour
{
    public float deathWaitTimer, deathWaitMax, attackTimer, attackTimerMax, wingSpeed, swordTimer, swordTimerMax, skullTimer, skullTimerMax,
    teleportTimer, teleportTimerMax, spotlightTimer, spotlightTimerMax;

	public MuseScript muse;

	public bool started, inAttackBox, wingsOut, swordsActive, darkActive, skullsActive, swordWallsActive,
    oneTearOut, twoTearOut, teleportActive, darknessActive;

    public GameObject middlePosition, swordWheelFour, swordWheelFive, swordWall, cryingEye, skull,
    topWallSpawn, leftWallSpawn, rightWallSpawn, bottomWallSpawn;

    public Vector2 magpiePosition;

    public Animator wingsAnim, leftSlash, rightSlash, downSlash, upSlash;

    public SpriteRenderer[] rigPieces;

    public SpriteRenderer leftPupil, rightPupil, mask, leftMask, rightMask, darkness;

    public Sprite normalPupil, whitePupil;

    public GameObject eye1, eye2, spotlight1, spotlight2, spotlight3;

    public Vector2 spotlightTarget1, spotlightTarget2, spotlightTarget3;

    public Color darknessColor;

    public GameObject rain;

    public bool rainActive;

    public float rainTimer,rainTimerMax;

    public Animator lightning;

    public float lightningTimer, lightningTimerMax;

    public AudioSource swordSound, teleportSound, maskCracking, rainSound, lightningSound, entranceSound;

    public CameraScript cameraObject;

    // Start is called before the first frame update
    void Start()
    {

        //Links Main Muse Script
        muse = GetComponent<MuseScript>();

        //Adds the Boss's Dialogue to a String List

        //muse.textLines.Add("Turn back now, foolish girl. Only death and despair await you in my domain.");
        muse.textLines.Add("Don't you see? You are a living fable. A story meant to scare children into humility.");
        muse.textLines.Add("This story is destined to end in tragedy. Defeating me will not change that.");
        muse.textLines.Add("Your father must be punished, and what better instruments of torture than his own daughters.");
        muse.textLines.Add("It will be a shame to cut down such a promising youth. I will get no pleasure from this.");
        muse.textLines.Add("I am the harbinger of despair. You will soon learn true sorrow if you continue.");
        muse.textLines.Add("We are well past the point of no return. I will rend your soul from your puny body.");
        muse.textLines.Add("You are just delaying the inevitable. I will enjoy your downfall thoroughly.");
        muse.textLines.Add("It seems that tragedy has befallen me instead. Laugh now, little girl. The end comes for all.");
        muse.textLines.Add("All this, for a little trinket. How tragic. Get it out of my sight already.");
        muse.textLines.Add("I hope it will all be worth it, but something tells me it won’t.");

        //Sets Attack and Ability Timers

        attackTimer = attackTimerMax * 2;
        swordTimer = swordTimerMax/2;
        skullTimer = skullTimerMax/2;
        teleportTimer = teleportTimerMax;

        eye1.SetActive(false);
        eye2.SetActive(false);

        
        wingSpeed = muse.baseSpeed * 1.75f;
        leftPupil.sprite = whitePupil;
        rightPupil.sprite = whitePupil;

        lightning = GameObject.Find("Flash").GetComponent<Animator>();

        cameraObject = FindObjectOfType<CameraScript>();

        leftWallSpawn.transform.position = new Vector2(-cameraObject.screenWidth, leftWallSpawn.transform.position.y);
        rightWallSpawn.transform.position = new Vector2(cameraObject.screenWidth, rightWallSpawn.transform.position.y);

        rigPieces = GetComponentsInChildren<SpriteRenderer>();
        mask.enabled = true;

    }

    // Update is called once per frame
    void Update()
    {

        //Activates Darkness Mechanics and Manipulates Spotlight Sprite Masks

        if (darknessActive == true)
        {
            darkness.color = darknessColor;
            if (darknessColor.a < .95f)
            {
                darknessColor.a += .02f;
            }

            spotlightTimer -= Time.deltaTime;

            if (spotlightTimer <= 0)
            {
                spotlightTarget1 = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                spotlightTarget2 = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                spotlightTarget3 = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                spotlightTimer = spotlightTimerMax;
            }
            spotlight1.SetActive(true); 
            spotlight1.transform.position = Vector2.MoveTowards(spotlight1.transform.position, spotlightTarget1, .02f);

            spotlight2.SetActive(true);
            spotlight2.transform.position = Vector2.MoveTowards(spotlight2.transform.position, spotlightTarget2, .02f);

            spotlight3.SetActive(true);
            spotlight3.transform.position = Vector2.MoveTowards(spotlight3.transform.position, spotlightTarget3, .02f);
        }
        else
        {
            darkness.color = darknessColor;
            if (darknessColor.a >= 0)
            {
                darknessColor.a -= .02f;
            }
            spotlight1.SetActive(false);
            spotlight2.SetActive(false);
            spotlight3.SetActive(false);
        }

        if (muse.active == true){

            //Cosmetic Adjustments for Mid Fight Shift

            if(muse.phase <= 4 || muse.health <= 0 && muse.phase < muse.maxPhase){
                mask.enabled = true;
                leftPupil.sprite = whitePupil;
                rightPupil.sprite = whitePupil;
            }else{
                mask.enabled = false;
                leftPupil.sprite = normalPupil;
                rightPupil.sprite = normalPupil;
            }

            //Detects the Player's Position and Performs Melee Attacks Accordingly

            if(inAttackBox == true){
                attackTimer -= Time.deltaTime;
            }

            magpiePosition = muse.magpie.transform.position - muse.transform.position;

            if(attackTimer <= 0){
                if(magpiePosition.x < 0 && Mathf.Abs(magpiePosition.x) > Mathf.Abs(magpiePosition.y)){
                    float randomNumber = Random.Range(0f,1f);
                    if(randomNumber <= .33f){
                        muse.anim.SetTrigger("Double Slash");
                    }else{
                        muse.anim.SetTrigger("Attack Left");
                    }
                }
                else if(magpiePosition.x > 0 && Mathf.Abs(magpiePosition.x) > Mathf.Abs(magpiePosition.y)){
                    float randomNumber = Random.Range(0f,1f);
                    if(randomNumber <= .33f){
                        muse.anim.SetTrigger("Double Slash");
                    }else{
                        muse.anim.SetTrigger("Attack Right");
                    }
                }
                else if(magpiePosition.y > 0 && Mathf.Abs(magpiePosition.y) > Mathf.Abs(magpiePosition.x)){
                    muse.anim.SetTrigger("Attack Up");
                }
                else if(magpiePosition.y < 0 && Mathf.Abs(magpiePosition.y) > Mathf.Abs(magpiePosition.x)){
                    muse.anim.SetTrigger("Attack Down");
                }
                attackTimer = attackTimerMax;
            }

            //Spawns Sword Obstacles, Swords Spawn in Either a Circular or Straight Pattern

            if(swordsActive == true){
                swordTimer -= Time.deltaTime;

                if(swordTimer <= 0){
                    if(swordWallsActive == false){
                        if (muse.phase < 4)
                        {
                            GameObject newWheel = Instantiate(swordWheelFour, new Vector2(transform.position.x, transform.position.y + .5f), Quaternion.identity);
                            newWheel.transform.parent = gameObject.transform;
                            newWheel.transform.Rotate(0, 0, Random.Range(0f, 360f));
                        }
                        if (muse.phase >= 4)
                        {
                            GameObject newWheel = Instantiate(swordWheelFive, new Vector2(transform.position.x, transform.position.y + .5f), Quaternion.identity);
                            newWheel.transform.parent = gameObject.transform;
                            newWheel.transform.Rotate(0, 0, Random.Range(0f, 360f));
                        }

                        swordTimer = swordTimerMax;
                    }

                    else{
                        float randomNumber = Random.Range(0f, 1f);
                        if(randomNumber <= .5f){
                            if (muse.phase < 4)
                            {
                                GameObject newWheel = Instantiate(swordWheelFour, new Vector2(transform.position.x, transform.position.y + .5f), Quaternion.identity);
                                newWheel.transform.parent = gameObject.transform;
                                newWheel.transform.Rotate(0, 0, Random.Range(0f, 360f));
                            }
                            if (muse.phase >= 4)
                            {
                                GameObject newWheel = Instantiate(swordWheelFive, new Vector2(transform.position.x, transform.position.y + .5f), Quaternion.identity);
                                newWheel.transform.parent = gameObject.transform;
                                newWheel.transform.Rotate(0, 0, Random.Range(0f, 360f));
                            }
                        }

                        else{
                            float wallNumber = Random.Range(0f, 1f);
                            if(wallNumber <= 1 && wallNumber >= .75f){
                                Instantiate(swordWall, topWallSpawn.transform.position, Quaternion.Euler(0,0,0));
                            }else if(wallNumber <= .74f && wallNumber >= .5f){
                                Instantiate(swordWall, leftWallSpawn.transform.position, Quaternion.Euler(0,0,90));
                            }else if(wallNumber <= .49f && wallNumber >= .25f){
                                Instantiate(swordWall, bottomWallSpawn.transform.position, Quaternion.Euler(0,0,180));
                            }else if(wallNumber <= .24f && wallNumber >= 0){
                                Instantiate(swordWall, rightWallSpawn.transform.position, Quaternion.Euler(0,0,-90));
                            }
                        }
                        swordTimer = swordTimerMax;
                    }
                }
            }

            //Spawns Skull Enemies

            if(skullsActive == true){
                skullTimer -= Time.deltaTime;
                if(skullTimer <= 0){
                    Instantiate(skull, new Vector2(Random.Range(-4.5f, 4.5f), Random.Range(-4.5f, 4.5f)), Quaternion.identity);
                    skullTimer = skullTimerMax;
                }
            }

            //Wing Activation (Boss Moves Faster)

            if(wingsAnim.enabled == false && muse.phase == 5 && muse.textBoxOut == false && wingsOut == false){
                muse.anim.SetTrigger("Grow Wings");
                wingsOut = true;
                muse.speed = wingSpeed;
                muse.positionTimerMax -= .5f;
            }
            if(teleportActive == true){
                teleportTimer -= Time.deltaTime;
                if(teleportTimer <= 0){
                    teleportTimer = teleportTimerMax;
                    muse.anim.SetTrigger("Teleport");
                }
            }

            
            
    	}

        //Cosmetic Character and Environment Changes

        if (rainActive == true)
        {
            rainSound.enabled = true;
            rainTimer -= Time.deltaTime;
            lightningTimer -= Time.deltaTime;
            if (rainTimer <= 0)
            {
                for (int i = 0; i < 14; i++)
                {
                    Instantiate(rain, new Vector2(Random.Range(-5f, 8f), Random.Range(-5f, 8f)), rain.transform.rotation);
                }
                rainTimer = rainTimerMax;
            }

            if(lightningTimer <= 0)
            {
                lightning.SetTrigger("Flash");
                lightningSound.Play();
                lightningTimer = lightningTimerMax;
            }
        }

        if (muse.phase > 4){
            mask.enabled = false;
            leftMask.enabled = false;
            rightMask.enabled = false;
            leftPupil.sprite = normalPupil;
            rightPupil.sprite = normalPupil;
        }
        
        if(muse.active == false){
            muse.wandering = true;
            muse.idle = false;
        }

        //Individual Phases

        if(muse.phase == 1){
            swordsActive = true;
            swordWallsActive = false;
        }

        if(muse.phase == 2 && muse.phaseChange == false){
            muse.wandering = true;
            muse.idle = false;
            swordsActive = true;
            swordWallsActive = true;
            swordTimerMax = 13;
        }

        if(muse.phase == 3 && muse.phaseChange == false && muse.textBoxOut == false){
            
            muse.idle = false;
            muse.wandering = true;
            swordTimerMax = 18;
            if(oneTearOut == false){
                eye1.SetActive(true);
                oneTearOut = true;
            }
            attackTimerMax = 6;
        }

        else if(muse.phase == 4 && muse.phaseChange == false){
            
            muse.idle = false;
            muse.wandering = true;
            skullsActive = true;
            swordsActive = false;
            swordTimerMax = 12;   
        }

        else if(muse.phase == 5 && muse.phaseChange == false && muse.textBoxOut == false){
            rainActive = true;
            muse.idle = false;
            muse.wandering = true;
            muse.speed = wingSpeed;
            teleportActive = true; 
            if(twoTearOut == false){
                eye2.SetActive(true);
                twoTearOut = true;
            } 
            skullsActive = false;
            swordsActive = true;
        }

        else if(muse.phase == 6 && muse.phaseChange == false){
            muse.idle = false;
            muse.wandering = true;
            darknessActive = true;
            skullsActive = true;
            eye1.SetActive(false);
            eye2.SetActive(false);
            attackTimerMax = 5;
        }

        else if(muse.phase == 7 && muse.phaseChange == false){

            muse.idle = false;
            muse.wandering = true;
            darknessActive = true;
            eye1.SetActive(true);
            eye2.SetActive(true);
        }

        else if(muse.phase == 8 && muse.phaseChange == false){

            muse.idle = false;
            muse.wandering = true;   
            darknessActive = true;         
        }

        else if(muse.phase == 0){
        	muse.idle = true;
        	muse.wandering = false;
        	muse.specificPath = false;
        }

        if(muse.health <= 0 && muse.phase >= muse.maxPhase){
        	muse.idle = true;
        	muse.active = false;
            darknessActive = false;
            skullsActive = false;
            swordsActive = false;
            eye1.SetActive(false);
            eye2.SetActive(false);
            darknessActive = false;
        }

       //Climax Dialogue

        if(muse.lineNumber == 4 && Input.GetMouseButtonDown(0) && muse.textBoxOut == true || muse.lineNumber == 4 && Input.GetButtonDown("Confirm") && muse.textBoxOut == true)
        {
            muse.stopText = true;
            if(started == false){
                muse.anim.SetTrigger("Start");
                started = true;
                //muse.music.Play();
                //muse.music.volume = 0;
            }
        }

        if(muse.lineNumber == 4 && muse.phase == 3){
            muse.stopText = true;
            muse.ActivateTextbox();
            muse.lineNumber = 5;
            muse.NextLine();   
        }

        if(muse.lineNumber == 5 && muse.phase == 5){
            muse.stopText = true;
            muse.ActivateTextbox();
            muse.lineNumber = 6;
            muse.NextLine();   
        }

        //Ending Dialogue

        if(muse.lineNumber == 6 && muse.phase == muse.maxPhase){
            muse.stopText = true;
            muse.ActivateTextbox();
            muse.lineNumber = 7;
            muse.NextLine();   
        }

        if(muse.lineNumber == 7 && muse.health <= 0 && muse.phase == muse.maxPhase && deathWaitTimer <= 0)
        {
            deathWaitTimer = deathWaitMax;
        }

        if (muse.lineNumber >= 9)
        {
            muse.medallionAnim.SetTrigger("Earn Medallion");
        }

        if (deathWaitTimer >= 0){
            deathWaitTimer -= Time.deltaTime;
        }

        if(deathWaitTimer <= 0 && muse.health <= 0 && muse.phase == muse.maxPhase && muse.textBoxOut == false && muse.lineNumber == 7){
            muse.ActivateTextbox();
            muse.lineNumber = 8;
            muse.NextLine();   
        }

        if(muse.lineNumber == 10){
            muse.stopText = true;
            if(muse.textBoxOut == false){
                muse.endScreen.gameObject.SetActive(true);
                muse.endScreen.enabled = true;
                muse.endScreen.SetTrigger("Activate");
                muse.endText.text = "MELPOMENE HAS BEEN DEFEATED";
            }
        }
    }

    public void PlayAgain(){
    	muse.blackOut.sceneToLoad = "Melpomene";
        muse.blackOut.anim.SetTrigger("Fade Out");
    }

    //Player Detection

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            inAttackBox = true;
        }
    }

    void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            inAttackBox = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            inAttackBox = false;
        }
    }

    //Animation Events

    void GrowWings(){
        wingsAnim.gameObject.SetActive(true);
        wingsAnim.enabled = true;
    }

    void LeftAttack(){
        leftSlash.SetTrigger("Slash");
        swordSound.Play();
    }

    void RightAttack(){
        rightSlash.SetTrigger("Slash");
        swordSound.Play();
    }

    void UpAttack(){
        upSlash.SetTrigger("Slash");
        swordSound.Play();
    }

    void DownAttack(){
        downSlash.SetTrigger("Slash");
        swordSound.Play();
    }

    void Teleport(){
        transform.position = new Vector2(Random.Range(-4.5f, 4.5f), Random.Range(-4f, 4f));
        teleportSound.Play();
    }
    void StartFlash(){
        foreach(SpriteRenderer sprite in rigPieces){
            sprite.material.shader = muse.whiteSprite;
        }
    }
    void EndFlash(){
        foreach(SpriteRenderer sprite in rigPieces){
            sprite.material.shader = muse.spriteDefault;
        }
    }

    public void MaskCrack()
    {
        maskCracking.Play();
    }

    public void Entrancesound()
    {
        entranceSound.Play();
    }
}
