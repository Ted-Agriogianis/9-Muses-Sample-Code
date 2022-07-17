using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

//The MuseScript Controls Universal Elements for the Game's Main Bosses, the Muses, Such As Dialogue Formatting, Base Movement, and Phase Transistions

public class MuseScript : MonoBehaviour
{
    public float speed, baseSpeed, health, healthMax, hitTimer, positionTimerMax, yellowHealth;
    public float positionTimer, yellowTimer;

    public Vector3 newPosition;
    public Vector3 velocity;

    public List <GameObject> possiblePositions;
    public GameObject head, newPosObject, oldPosObject, burst, middlePosition;

    public Shader whiteSprite, spriteDefault;
    
    public SpriteRenderer[] headSprites;

    public Animator anim, topUI, endScreen, pauseScreen, museTitle;

    public Image healthBar, yellowBar;

    public Rigidbody2D rb;

    public bool active, wandering, idle, specificPath, textBoxOut, phaseChange, freeMove, finishedTalking;
    public bool startCou, stopText, pressed, musicOn;

    public CircleCollider2D hitBox;
      
    public MagpieScript magpie;

    public int phase, maxPositions, maxPhase, lineNumber, currentLevel;
    public int iconNumber, positionNumber;

    public TextMeshProUGUI endText, phaseText;

    public List<string> textLines;

	public DialogueScript textBox;

	public Coroutine coReference;

    public BlackoutScript blackOut;

    public Button pauseButton;

    public AudioSource music, startMusic;

    public List<Image> phaseIcons;

    private Joystick joystick;

    private string[] DestroyList;

    public List<AudioClip> voiceLines;

    public Animator medallionAnim;

    public float timeLimit, healthLimit;
    public bool beforeTime, beforeHealth;

    public bool paused;

    public EventSystem eventSystem;

    public float scoreMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = 0;
        positionTimer = positionTimerMax * 2;
        newPosition = new Vector2(Random.Range(-1.25f, 1.25f), Random.Range(-2.5f, 2.5f));
        health = healthMax;

        whiteSprite = Shader.Find("GUI/Text Shader");
        spriteDefault = Shader.Find("Sprites/Default");

        headSprites = head.GetComponentsInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        yellowHealth = healthMax;
        baseSpeed = speed;
        endScreen.enabled = false;
        

        music = Camera.main.transform.parent.GetComponent<AudioSource>();
        startMusic = Camera.main.GetComponent<AudioSource>();
        magpie = FindObjectOfType<MagpieScript>();
        hitBox = head.GetComponent<CircleCollider2D>();

        //Links UI Elements

        phaseText = GameObject.Find("Phase Number").GetComponent<TextMeshProUGUI>();
        healthBar = GameObject.Find("Muse Health Bar").GetComponent<Image>();
        yellowBar = GameObject.Find("Muse Yellow Bar").GetComponent<Image>();
        endScreen = GameObject.Find("End Screen").GetComponent<Animator>();
        pauseScreen = GameObject.Find("Pause Screen").GetComponent<Animator>();
        museTitle = GameObject.Find("Muse Title").GetComponent<Animator>();
        blackOut = GameObject.Find("Blackscreen").GetComponent<BlackoutScript>();
        textBox = GameObject.Find("Textbox").GetComponent<DialogueScript>();
        eventSystem = FindObjectOfType<EventSystem>();

        for (int i = 1; i <=maxPhase; i++)
        {
            phaseIcons.Add(GameObject.Find("Phase Icon (" + i + ")").GetComponent<Image>());
        }
        iconNumber = phaseIcons.Count - 1;

        
        SaveData.currentLevel = currentLevel;
        SaveData.UpdatePlayerPrefs();

        //Sets Position Markers For the Boss' Movement

        for (int i = 0; i <= 8; i++)
        {
            possiblePositions[i] = GameObject.Find("Possible Position (" + i + ")");
        }
        middlePosition = possiblePositions[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Sets Idle Position That the Boss Will Return To Between Phases

        if(idle){
            transform.position = Vector3.Lerp(transform.position, middlePosition.transform.position, .1f);
        }



        if(active){

            //Movement Code for the Bosses. The Boss Will Select Random Positions and Move to Them Over Time

            if(wandering == true){
                positionTimer -= Time.deltaTime;

                if(positionTimer <= 0){
                    if(newPosObject != null){
                        oldPosObject = newPosObject;
                    }
                    newPosObject = possiblePositions[Random.Range(0, possiblePositions.Count)];
                    if(newPosObject == oldPosObject){
                        positionTimer = 0;
                        newPosObject = possiblePositions[Random.Range(0, possiblePositions.Count)];
                    }
                    newPosition = newPosObject.transform.position;
                    velocity = newPosition - transform.position;
                    positionTimer = positionTimerMax;
                }

                rb.AddForce(velocity * speed);

             //This Movement System Introduces Less Randomness to the Movement

             }else if(specificPath){

                positionTimer -= Time.deltaTime;

                if(positionTimer <= 0){
                    positionNumber = possiblePositions.IndexOf(newPosObject) + Random.Range(-1,1);
                    if(positionNumber == possiblePositions.IndexOf(newPosObject)){
                        positionNumber = possiblePositions.IndexOf(newPosObject) + Random.Range(-1,1);
                    }
                    if(positionNumber > possiblePositions.Count - 1){
                        positionNumber = 0;
                    }else if(positionNumber < 0){
                        positionNumber = possiblePositions.Count - 1;
                    }
                    newPosObject = possiblePositions[positionNumber];
                    newPosition = newPosObject.transform.position;
                    positionTimer = positionTimerMax;
                }

            velocity = newPosition - transform.position;
            rb.AddForce(velocity * speed);
        }

        //Activates the Failure Condition

        if(magpie.health <= 0)
        {
            anim.SetTrigger("Victory");
            active = false;
            hitBox.enabled = false;
            foreach(SpriteRenderer sprite in headSprites)
            {
                sprite.material.shader = spriteDefault;
            }
        }
    }

        //Activates Damage Reprieve After Hit

        if(hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;

            if(hitTimer <= .6f)
            {
                foreach(SpriteRenderer sprite in headSprites)
                {
                    sprite.material.shader = spriteDefault;
                }
                anim.ResetTrigger("Hit Right");
                anim.ResetTrigger("Hit Left");
                anim.ResetTrigger("Hit Up");
                anim.ResetTrigger("Hit Down");
            }
        }        

        //Updates the Health Bar and the Yellow Bar Indicating How Much Health Was Lost

        if(yellowTimer > 0){
            yellowTimer -= Time.deltaTime;
        }
        if(yellowTimer <= 0 && (yellowHealth / healthMax) > (health / healthMax)){
            yellowHealth -= 1;
            yellowBar.fillAmount = yellowHealth / healthMax;
        }

        if(health >= healthMax){
            healthBar.fillAmount += .05f;
        }
    }

    void Update(){

        //Pauses the Game

        if (Input.GetButtonDown("Pause"))
        {
            Debug.Log("Pause");
            if (paused == false)
            {
                Pause();
            }
            else if (paused == true)
            {
                UnPause();
            }
        }

        //Controls the Look and Sequence of the Text Box and Dialogue

        if (textBoxOut == true){
            pauseButton.enabled = false;

            if(Input.GetMouseButtonDown(0) || Input.GetButtonDown("Confirm")){
                if(textBox.letterCount >= textBox.text.Length){

                    if(stopText == false){
                        lineNumber += 1;
                        NextLine();
                    }else{
                        DeactivateTextbox();
                        active = true;
                    }

                }else{
                    StopCoroutine(coReference);
                    textBox.letterCount = textBox.text.Length;
                    textBox.dialogue.maxVisibleCharacters = textBox.text.Length;
                    textBox.dialogue.text = textBox.text;
                    textBox.anim.SetTrigger("Waited");
                }
            }
        }else
        {
            pauseButton.enabled = true;
        }

        //Activates Phase Transitions

        if(health <= 0 && phase < maxPhase){
             anim.SetTrigger("Change Phase");
             phaseIcons[iconNumber].GetComponent<Animator>().enabled = true;
             idle = true;
             wandering = false;
             phaseChange = true;
        }

        anim.SetInteger("Phase", phase);

        //Music Fade In and Out at Start and End of the Fight

        if (musicOn == true)
        {
            if (music.volume < 1 && textBoxOut == false)
            {
                music.volume += .01f;
            }

            if (startMusic.volume > 0)
            {
                startMusic.volume -= .01f;
            }
        }
        else
        {
            if (music.volume > 0)
            {
                music.volume -= .01f;
            }

            if (startMusic.volume < .3f)
            {
                startMusic.volume += .01f;
            }
        }

        if (music.pitch < 1 + (phase - 1) * .02f)
        {
            music.pitch += .0001f;
        }

        if(textBoxOut == true)
        {
            idle = true;
        }
    }

    //Removes Health and Plays Hit Animation on Hit, The Boss Reacts With Unique Animations Depending on the Angle of the Strike

    public void TakeDamage(float damage, Vector2 hitDirection){
        if(/*hitTimer <= 0 &&*/ textBoxOut == false){
            //Instantiate(burst, head.transform.position, Quaternion.identity);
            health -= damage;
            healthBar.fillAmount = health / healthMax;
            hitTimer = .75f;
            yellowTimer = 3;
            foreach(SpriteRenderer sprite in headSprites){
                sprite.material.shader = whiteSprite;
            }

            if(hitDirection.x >= .25 && Mathf.Abs(hitDirection.x) > Mathf.Abs(hitDirection.y)){
                anim.SetTrigger("Hit Right");
            }else if(hitDirection.x <= .25 && Mathf.Abs(hitDirection.x) > Mathf.Abs(hitDirection.y)){
                anim.SetTrigger("Hit Left");
            }else if(hitDirection.y >= .25 && Mathf.Abs(hitDirection.y) > Mathf.Abs(hitDirection.x)){
                anim.SetTrigger("Hit Up");
            }else if(hitDirection.y <= .25 && Mathf.Abs(hitDirection.y) > Mathf.Abs(hitDirection.x)){
                anim.SetTrigger("Hit Down");
            }else{
                anim.SetTrigger("Hit Up");
            }

            if(health <= 0 && phase == maxPhase){
                idle = true;
                anim.SetTrigger("Death");
                active = false;
                hitBox.enabled = false;
                phaseIcons[0].GetComponent<Animator>().enabled = true;
                phaseIcons[0].GetComponent<Animator>().SetTrigger("Break");
                foreach (SpriteRenderer sprite in headSprites){
                    sprite.material.shader = spriteDefault;
                }
                music.Stop();
                DestroyThings();
            }

            if(magpie.health <= 0){
                phase = 0;
            }
            topUI.SetTrigger("Shake");
        }
        
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Player Attack"){
            if (gameObject.GetComponent<HomingScript>() != null)
            {
                hitTimer = 0;
            }
            if (other.GetComponent<DamageScript>() != null){
                TakeDamage(other.gameObject.GetComponent<DamageScript>().damage, other.gameObject.transform.up);
                other.gameObject.GetComponent<DamageScript>().DestroySelf();
                magpie.effectSound.PlayOneShot(magpie.attackSounds[Random.Range(0, magpie.attackSounds.Length)]);
                if (gameObject.GetComponent<HomingScript>() != null)
                {
                    hitTimer = 0;
                }
            }
        }
    }

    void TurnActive(){
        active = true;
    }

    //Increases Phase When Health Depletes

    public void IncreasePhase(){
        phase += 1;
        anim.ResetTrigger("Change Phase");
        finishedTalking = false;
        phaseChange = false;
        health = healthMax;
        yellowHealth = healthMax;
        yellowBar.fillAmount = yellowHealth / healthMax;
        
        phaseText.text = "" + phase;
        phaseIcons[iconNumber].GetComponent<Animator>().SetTrigger("Break");
        iconNumber -= 1;
        hitBox.enabled = true;
    }

    //Turns the Textbox On And Off

    public void ActivateTextbox(){
        textBoxOut = true;
        magpie.canFire = false;
        foreach(SpriteRenderer sprite in headSprites){
                    sprite.material.shader = spriteDefault;
                }
    	coReference = StartCoroutine(textBox.Typewriter());
        textBox.fullAudio.Play();
        
        textBox.anim.SetTrigger("Activate");
    	
    	anim.SetBool("Talking", true);
    	magpie.active = false;
        magpie.speedStage = 1;
        active = false;
        music.volume = .3f;

        if (lineNumber == 0 && voiceLines.Count > 0)
        {
            textBox.voiceActing.clip = voiceLines[lineNumber];
            textBox.voiceActing.Play();
        }
    }

    public void DeactivateTextbox(){
        textBoxOut = false;
        magpie.active = true;
        magpie.speedStage = 1;
        textBox.dialogue.text = "";
        finishedTalking = true;
    	textBox.anim.ResetTrigger("Activate");
    	textBox.anim.SetTrigger("Deactivate");
        stopText = false;
    	anim.SetBool("Talking", false);
        active = true;
        pressed = false;
        idle = true;
        magpie.gracePeriod = magpie.gracePeriodMax;
        museTitle.enabled = true;

        if(active == true && music.isPlaying == false && health > 0)
        {
            musicOn = true;
            music.Play();
        }
        textBox.voiceActing.Stop();
    }

    //Progresses the Dialogue

    public void NextLine(){
        textBox.letterCount = 0;
        textBox.fullAudio.Play();

        if (voiceLines.Count > 0)
        {
            textBox.voiceActing.clip = voiceLines[lineNumber];
            textBox.voiceActing.Play();
        }

        textBox.dialogue.maxVisibleCharacters = 0;
        textBox.anim.ResetTrigger("Waited");
        textBox.anim.SetTrigger("New Line");
    	StopCoroutine(coReference);   			
        textBox.dialogue.text = "";
        textBox.text = "";

        if(lineNumber < textLines.Count){
            textBox.text = textLines[lineNumber];
        }

        coReference = StartCoroutine(textBox.Typewriter());
        pressed = false;

            
    }

    //Transitions to Other Scenes From Menu Buttons

    public void Quit(){
        blackOut.sceneToLoad = "Start Scene";
        blackOut.anim.SetTrigger("Fade Out");
        Time.timeScale = 1;
        pauseScreen.SetTrigger("Deactivate");
    }

    public void LevelSelect(){
        blackOut.sceneToLoad = "Level Select";
        blackOut.anim.SetTrigger("Fade Out");
        //Time.timeScale = 1;
        //pauseScreen.SetTrigger("Deactivate");
    }

    public void Pause(){
        eventSystem.SetSelectedGameObject(GameObject.Find("Continue Button"));
        paused = true;
        Time.timeScale = 0;
        pauseScreen.SetTrigger("Activate");
        pauseScreen.ResetTrigger("Deactivate");
        pauseScreen.gameObject.SetActive(true);
        joystick.enabled = false;
        pauseButton.enabled = false;

    }

    public void UnPause(){
        paused = false;
        Time.timeScale = 1;
        pauseScreen.ResetTrigger("Activate");
        pauseScreen.SetTrigger("Deactivate");
        eventSystem.SetSelectedGameObject(null);
        joystick.enabled = true;
        pauseButton.enabled = true;

    }

    //Clears Level of Enemies and Hazards When the Boss is Defeated

    public void DestroyThings()
    {
        DestroyableScript[] destroyables = FindObjectsOfType<DestroyableScript>();
        foreach(DestroyableScript item in destroyables)
        {
              Destroy(item);
        }
        
    }

}
