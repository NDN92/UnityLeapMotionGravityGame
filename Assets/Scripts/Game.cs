using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class Game : MonoBehaviour {

    public Vector3 rangeLeapMin;
    public Vector3 rangeLeapMax;
    public Vector3 rangeLevelMin;
    public Vector3 rangeLevelMax;

    private Controller leap_controller;
    private Frame leap_frame;
    private List<Hand> leap_hands;    

    private Vector3 leap_leftHandPos;
    private Vector3 leap_rightHandPos;
    private Vector3 leftHandPos = new Vector3(0f, 0f, 0f);
    private Vector3 rightHandPos = new Vector3(0f, 0f, 0f);


    public TextObj lifesText;
    public TextObj timeText;    
    public TextObj levelText;    
    public Player player;
    public Ball ball;
    public GameObject enemies;
    public Weapons weapons;
    public GameObject gameBoundaries;

    public MicInput micInput;
    public GameObject audioBarL;
    public GameObject audioBarR;
    private Color audioBarL_Color;
    private Color audioBarR_Color;
    private float audioBarL_InitScaleY;
    private float audioBarR_InitScaleY;

    public ScreenDarken screenDarken;

    public StartMenu startMenu;

    private int lifesCount;
    private float survivedTime;
    private float currTimePeriod;
    private int timePeriodCount;

    private bool _isStarted;
    private bool _isStartingProcedure;



    private bool shootPossible;

    // Use this for initialization
    void Start () {
        leap_controller = new Controller();
        leap_controller.Device += OnDevice;
        leap_controller.DeviceFailure += OnDeviceFailure;

        _isStarted = false;
        _isStartingProcedure = false;
        screenDarken.setOpacity(0f);

        lifesCount = 3;
        survivedTime = 0f;
        currTimePeriod = 0f;
        timePeriodCount = 1;

        audioBarL_Color = audioBarL.GetComponent<Renderer>().material.color;
        audioBarR_Color = audioBarR.GetComponent<Renderer>().material.color;
        audioBarL.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
        audioBarR.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
        audioBarL_InitScaleY = audioBarL.transform.localScale.y;
        audioBarR_InitScaleY = audioBarR.transform.localScale.y;

        shootPossible = true;
    }

    public void OnDevice(object sender, DeviceEventArgs args)
    {
        Debug.Log("Leap is connected!");
    }

    private void OnDeviceFailure(object sender, DeviceFailureEventArgs args)
    {
        Debug.Log("Leap fails to connect!");
    }

    private void OnApplicationQuit()
    {
        screenDarken.setOpacity(0f);
        audioBarL.GetComponent<Renderer>().material.color = audioBarL_Color;
        audioBarR.GetComponent<Renderer>().material.color = audioBarR_Color;
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("Mic Loudness" + micInput.getLoudness());
	}

    private void FixedUpdate()
    {
        //checkMicrophone();

        if(!leap_controller.IsConnected)
        {
            return;
        }

        leap_frame = leap_controller.Frame();
        leap_hands = leap_frame.Hands;
        //Debug.Log(leap_controller);
        //Debug.Log("Hand Count: " + leap_hands.Count);

        //Debug.Log(leap_leftHandPos.z);
        //Debug.Log(leap_rightHandPos.z);

        if (!_isStarted && leap_hands.Count == 0)
        {
            startMenu.noHandDetected();
        } else if (!_isStarted && leap_hands.Count == 1)
        {
            startMenu.oneHandDetected(leap_hands[0]);
        } else if (!_isStarted && leap_hands.Count == 2)
        {
            startMenu.twoHandsDetected(Time.deltaTime);
        }

        if(_isStartingProcedure)
        {
            updateHandPositions();
            player._update();
        }

        if (_isStarted) {

            if(checkLeftSensorArea())
            {
                return;
            }

            updateHandPositions();
            player._update();
            checkGestures();
            checkLeavingSensorArea();
            checkMicrophone();

            survivedTime += Time.deltaTime;
            currTimePeriod += Time.deltaTime;

            if(survivedTime < 10f)
            {
                if (currTimePeriod > 2f)
                {
                    doLevel_01();

                    timePeriodCount++;
                    currTimePeriod = 0;
                }
            }
            else if(survivedTime < 20f)
            {
                if (currTimePeriod > 1.3f)
                {
                    doLevel_01();

                    timePeriodCount++;
                    currTimePeriod = 0;
                }
            }
            else if (survivedTime < 30f)
            {
                if (currTimePeriod > 0.8f)
                {
                    doLevel_01();

                    timePeriodCount++;
                    currTimePeriod = 0;
                }
            }
            else
            {
                levelText.updateText("Level 02");
            }



            timeText.updateText_Time(survivedTime);
        }
    }

    private void updateHandPositions()
    {
        if (leap_hands[0].PalmPosition.x < leap_hands[1].PalmPosition.x)
        {
            leap_leftHandPos = new Vector3(leap_hands[0].PalmPosition.x, leap_hands[0].PalmPosition.y, leap_hands[0].PalmPosition.z);
            leap_rightHandPos = new Vector3(leap_hands[1].PalmPosition.x, leap_hands[1].PalmPosition.y, leap_hands[1].PalmPosition.z);
            leftHandPos = convertLeapSpaceToWorldSpace(leap_leftHandPos);
            rightHandPos = convertLeapSpaceToWorldSpace(leap_rightHandPos);
        }
        else
        {
            leap_rightHandPos = new Vector3(leap_hands[0].PalmPosition.x, leap_hands[0].PalmPosition.y, leap_hands[0].PalmPosition.z);
            leap_leftHandPos = new Vector3(leap_hands[1].PalmPosition.x, leap_hands[1].PalmPosition.y, leap_hands[1].PalmPosition.z);
            rightHandPos = convertLeapSpaceToWorldSpace(leap_leftHandPos);
            leftHandPos = convertLeapSpaceToWorldSpace(leap_rightHandPos);
        }
    }
    public Vector3 convertToNewRange(Vector3 input, Vector3 inputRangeMin, Vector3 inputRangeMax, Vector3 newRangeMin, Vector3 newRangeMax)
    {
        Vector3 diffOld = new Vector3(inputRangeMax.x - inputRangeMin.x, inputRangeMax.y - inputRangeMin.y, inputRangeMax.z - inputRangeMin.z);
        Vector3 diffNew = new Vector3(newRangeMax.x - newRangeMin.x, newRangeMax.y - newRangeMin.y, newRangeMax.z - newRangeMin.z);
        float resultX = (((input.x - inputRangeMin.x) * diffNew.x) / diffOld.x) + newRangeMin.x;
        float resultY = (((input.y - inputRangeMin.y) * diffNew.y) / diffOld.y) + newRangeMin.y;
        float resultZ = (((input.z - inputRangeMin.z) * diffNew.z) / diffOld.z) + newRangeMin.z;

        return new Vector3(resultX, resultY, resultZ);
    }
    public Vector3 convertLeapSpaceToWorldSpace(Vector3 leap_input)
    {
        return convertToNewRange(leap_input, rangeLeapMin, rangeLeapMax, rangeLevelMin, rangeLevelMax);
    }

    private void checkLeavingSensorArea()
    {
        /*
        float xd = 0f;
        if(leap_leftHandPos.x < rangeLeapMin.x)
        {
            xd = rangeLeapMin.x - leap_leftHandPos.x;
        } if(leap_leftHandPos.x > rangeLeapMax.x)
        {
            xd = leap_leftHandPos.x - rangeLeapMax.x;
        }

        float yd = 0f;
        if (leap_leftHandPos.y < rangeLeapMin.y)
        {
            yd = rangeLeapMin.y - leap_leftHandPos.y;
        }
        if (leap_leftHandPos.y > rangeLeapMax.y)
        {
            yd = leap_leftHandPos.y - rangeLeapMax.y;
        }
        */

        float zdl = 0f;
        if (leap_leftHandPos.z < rangeLeapMin.z)
        {
            zdl = rangeLeapMin.z - leap_leftHandPos.z;
        }
        if (leap_leftHandPos.z > rangeLeapMax.z)
        {
            zdl = leap_leftHandPos.z - rangeLeapMax.z;
        }
        float zdr = 0f;
        if (leap_rightHandPos.z < rangeLeapMin.z)
        {
            zdl = rangeLeapMin.z - leap_rightHandPos.z;
        }
        if (leap_rightHandPos.z > rangeLeapMax.z)
        {
            zdl = leap_rightHandPos.z - rangeLeapMax.z;
        }


        //Debug.Log("X:" + xd + " Y:" + yd + " Z:" + zd);


        //float maxd = Mathf.Max(xd, yd, zd);
        float maxd = Mathf.Max(zdl, zdr);
        float opacity = maxd / 200f;

        //Debug.Log(opacity);

        if(opacity > 1f)
        {
            opacity = 1f;
        }
        screenDarken.setOpacity(opacity);
    }

    private bool checkLeftSensorArea()
    {
        if(leap_hands.Count < 2)
        {
            player.resetPosition();
            return true;
        }
        return false;
    }

    private void checkGestures()
    {
        bool leftHandClosed = false;
        bool leftHandClosedWithoutThumb = false;
        bool rightHandClosed = false;
        bool rightHandClosedWithoutThumb = false;

        bool thumbClosed;
        bool indexClosed;
        bool middleClosed;
        bool ringClosed;
        bool pinkyClosed;
        for(int i = 0; i < leap_hands.Count; i++)
        {
            thumbClosed = false;
            indexClosed = false;
            middleClosed = false;
            ringClosed = false;
            pinkyClosed = false;

            foreach (Finger finger in leap_hands[i].Fingers)
            {
                switch (finger.Type)
                {
                    case Finger.FingerType.TYPE_THUMB:
                        thumbClosed = !finger.IsExtended;
                        break;
                    case Finger.FingerType.TYPE_INDEX:
                        indexClosed = !finger.IsExtended;
                        break;
                    case Finger.FingerType.TYPE_MIDDLE:
                        middleClosed = !finger.IsExtended;
                        break;
                    case Finger.FingerType.TYPE_RING:
                        ringClosed = !finger.IsExtended;
                        break;
                    case Finger.FingerType.TYPE_PINKY:
                        pinkyClosed = !finger.IsExtended;
                        break;
                }
            }
            //Debug.Log(leap_hands[i].GrabAngle);

            if( (i == 0 && leap_hands.Count > i && leap_hands[i].PalmPosition.x < leap_hands[(i+1)].PalmPosition.x) || 
                (i == 1 && leap_hands[i].PalmPosition.x < leap_hands[(i - 1)].PalmPosition.x) )
            {
                leftHandClosedWithoutThumb = !thumbClosed && indexClosed && middleClosed && ringClosed && pinkyClosed && (leap_hands[i].GrabAngle > 3f);
                leftHandClosed = thumbClosed && indexClosed && middleClosed && ringClosed && pinkyClosed && (leap_hands[i].GrabAngle > 3f);
            } else
            {
                rightHandClosedWithoutThumb = !thumbClosed && indexClosed && middleClosed && ringClosed && pinkyClosed && (leap_hands[i].GrabAngle > 3f);
                rightHandClosed = thumbClosed && indexClosed && middleClosed && ringClosed && pinkyClosed && (leap_hands[i].GrabAngle > 3f);
            }            
        }

        /*
        Debug.Log("leftHandClosed: " + leftHandClosed + "; " 
            + "leftHandClosedWithoutThumb: " + leftHandClosedWithoutThumb + "; " 
            + "rightHandClosed: " + rightHandClosed + "; " 
            + "rightHandClosedWithoutThumb: " + rightHandClosedWithoutThumb);
        */

        if( shootPossible && (leftHandClosedWithoutThumb || rightHandClosedWithoutThumb) )
        {
            Bullet newBullet = Instantiate(weapons.getBullet()).GetComponent<Bullet>();
            Physics.IgnoreCollision(newBullet.GetComponent<Collider>(), player.GetComponent<Collider>());
            Physics.IgnoreCollision(newBullet.GetComponent<Collider>(), ball.GetComponent<Collider>());

            newBullet.transform.position = player.transform.position;
            newBullet.transform.rotation = player.transform.rotation;
            newBullet.rb.velocity = newBullet.transform.up.normalized * 30;           

            shootPossible = false;
        }
        
    }

    private void checkMicrophone()
    {
        float maxVelocity = 5f;
        float loudness = micInput.getLoudness() * 0.5f;
        float velocity = loudness + ball.rb.velocity.x;
        float scaleYL = 0f;
        float scaleYR = 0f;

        audioBarL.GetComponent<Renderer>().material.color = audioBarL_Color;
        audioBarR.GetComponent<Renderer>().material.color = audioBarR_Color;
        scaleYL = convertToNewRange(new Vector3(loudness, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(audioBarL_InitScaleY, 0, 0)).x;
        scaleYR = convertToNewRange(new Vector3(loudness, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(audioBarR_InitScaleY, 0, 0)).x;
        audioBarL.transform.localScale = new Vector3(audioBarL.transform.localScale.x, scaleYL, audioBarL.transform.localScale.z);
        audioBarR.transform.localScale = new Vector3(audioBarR.transform.localScale.x, scaleYR, audioBarR.transform.localScale.z);

        if (micInput.getLoudness() > 0.2f && velocity < maxVelocity)
        {
            ball.rb.velocity = new Vector3(velocity, ball.rb.velocity.y, ball.rb.velocity.z);      
        }

        //audioBarL.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
        //audioBarR.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
    }

    private void doLevel_01()
    {
        GameObject newCubeEnemy = Instantiate(enemies.transform.Find("Cube").gameObject);

        float rand_xPos = Random.Range((rangeLevelMin.x + 2f), (rangeLevelMax.x - 2f));
        Vector3 newCubeEnemy_Pos = new Vector3(rand_xPos, rangeLevelMax.y + 2f, newCubeEnemy.transform.position.z);
        newCubeEnemy.transform.position = newCubeEnemy_Pos;

        Rigidbody newCubeEnemy_Rb = newCubeEnemy.GetComponents<Rigidbody>()[0];
        newCubeEnemy_Rb.useGravity = true;
        newCubeEnemy_Rb.angularVelocity = new Vector3(0f, 0f, -1f);
        Physics.IgnoreCollision(newCubeEnemy.GetComponent<Collider>(), player.GetComponent<Collider>());
        Physics.IgnoreCollision(newCubeEnemy.GetComponent<Collider>(), gameBoundaries.transform.Find("Ceiling").gameObject.GetComponent<Collider>());
    }

    public void startGame()
    {
        _isStarted = true;
        _isStartingProcedure = false;
        ball.useGravity(true);
    }

    public void endGame()
    {
        startMenu.setIsGameOver(true);
        startMenu.updateScores(timeText.getText());

        _isStarted = false;
        _isStartingProcedure = false;
        ball.useGravity(false);
        ball.resetPosition();

        lifesCount = 3;
        survivedTime = 0f;
        currTimePeriod = 0f;
        timePeriodCount = 1;

        timeText.updateText_Time(survivedTime);
        updateLifes(lifesCount);

        screenDarken.setOpacity(0f);       
    }

    public bool isStarted()
    {
        return _isStarted;
    }
    public bool isStartingProcedure()
    {
        return _isStartingProcedure;
    }
    public void setIsStartingProcedure(bool isSTartingProcedure)
    {
        _isStartingProcedure = isSTartingProcedure;
    }
    public void setShootPossible(bool shootPossible)
    {
        this.shootPossible = shootPossible;
    }

    public void updateLifes(int lifes)
    {
        lifesCount = lifes;
        string lifesAsText = "";
        for(int i = 0; i < lifes; i++)
        {
            if(i == 0)
            {
                lifesAsText += "•";
            }
            else
            {
                lifesAsText += " •";
            }
        }
        lifesText.updateText(lifesAsText);

        if(lifes < 1)
        {
            endGame();
        }
    }
    public void lifesMinusOne()
    {
        updateLifes(--lifesCount);
    }

    public Vector3 getLeap_leftHandPos()
    {
        return leap_leftHandPos;
    }
    public Vector3 getLeap_rightHandPos()
    {
        return leap_rightHandPos;
    }
    public Vector3 getLeftHandPos()
    {
        return leftHandPos;
    }
    public Vector3 getRightHandPos()
    {
        return rightHandPos;
    }
}
