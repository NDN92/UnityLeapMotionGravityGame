using UnityEngine;
using System.Collections;
using Leap;

public class StartMenu : MonoBehaviour
{
    public Game game;
    public GameObject bg;
    public GameObject text_gameTitle;
    public TextObj text_scoresHeader;
    public TextObj text_score1;
    public TextObj text_score2;
    public TextObj text_score3;
    public TextObj text_yourScoreHeader;
    public TextObj text_yourScore;
    public GameObject leftHandModel;
    public GameObject leftHandModel_Mesh;
    public GameObject rightHandModel;
    public GameObject rightHandModel_Mesh;
    public TextObj text_countdown;

    private Vector3 bg_StartPos;
    private Vector3 text_gameTitle_StartPos;
    private Vector3 text_scoresHeader_StartPos;
    private Vector3 text_score1_StartPos;
    private Vector3 text_score2_StartPos;
    private Vector3 text_score3_StartPos;
    private Vector3 text_yourScoreHeader_StartPos;
    private Vector3 text_yourScore_StartPos;
    private Vector3 leftHandModel_StartPos;
    private Vector3 rightHandModel_StartPos;
    private Vector3 text_countdown_StartPos;

    private float invisibleZ;

    private bool handAnimation;
    private bool leftHandAnimation;
    private bool rightHandAnimation;
    private bool handAnimation_Wait;
    private float handAnimation_WaitTime;
    private float twoHandsTime;

    private bool _isGameOver;
    private bool restartAllowed;
    private float gameOver_waitTime;

    // Use this for initialization
    void Start()
    {        
        bg_StartPos = bg.transform.position;
        text_gameTitle_StartPos = text_gameTitle.transform.position;
        text_scoresHeader_StartPos = text_scoresHeader.transform.position;
        text_score1_StartPos = text_score1.transform.position;
        text_score2_StartPos = text_score2.transform.position;
        text_score3_StartPos = text_score3.transform.position;
        text_yourScoreHeader_StartPos = text_yourScoreHeader.transform.position;
        text_yourScore_StartPos = text_yourScore.transform.position;
        leftHandModel_StartPos = leftHandModel.transform.position;
        rightHandModel_StartPos = rightHandModel.transform.position;
        text_countdown_StartPos = text_countdown.transform.position;

        invisibleZ = -11;
        text_yourScoreHeader.transform.position = new Vector3(text_yourScoreHeader.transform.position.x, text_yourScoreHeader.transform.position.y, invisibleZ);
        text_yourScore.transform.position = new Vector3(text_yourScore.transform.position.x, text_yourScore.transform.position.y, invisibleZ);

        handAnimation = true;
        leftHandAnimation = true;
        rightHandAnimation = true;
        handAnimation_Wait = false;
        handAnimation_WaitTime = 0f;

        twoHandsTime = 0f;

        _isGameOver = false;
        restartAllowed = false;
        gameOver_waitTime = 0f;

        updateScores("00:00");
        //resetScores();        
    }

    private void FixedUpdate()
    {
        if(handAnimation)
        {
            if(!handAnimation_Wait)
            {
                float targetPosY = -1.5f;
                Vector3 speed = new Vector3(0f, 0.05f, 0f);

                moveAnimation(leftHandModel, speed);
                moveAnimation(rightHandModel, speed);
                
                if (leftHandModel.transform.position.y > targetPosY || rightHandModel.transform.position.y > targetPosY)
                {
                    leftHandModel.transform.position = new Vector3(leftHandModel.transform.position.x, targetPosY, leftHandModel.transform.position.z);
                    rightHandModel.transform.position = new Vector3(rightHandModel.transform.position.x, targetPosY, rightHandModel.transform.position.z);
                    handAnimation_Wait = true;
                }
            }
            else
            {
                handAnimation_WaitTime += Time.deltaTime;
                float targetWaitTime = 0.5f;
                if(handAnimation_WaitTime > targetWaitTime)
                {
                    leftHandModel.transform.position = leftHandModel_StartPos;
                    rightHandModel.transform.position = rightHandModel_StartPos;
                    handAnimation_Wait = false;
                    handAnimation_WaitTime = 0f;
                }
            }

            if(!leftHandAnimation)
            {
                leftHandModel.transform.position = leftHandModel_StartPos;
            }       
            if(!rightHandAnimation)
            {
                rightHandModel.transform.position = rightHandModel_StartPos;
            }
            if(!leftHandAnimation && !rightHandAnimation)
            {
                handAnimation = false;
            }
        }

        if (_isGameOver)
        {
            if (bg.transform.position.z < bg_StartPos.z)
            {
                Vector3 offset = new Vector3(0f, 0f, 0.2f);
                moveAnimation(bg, offset);
                moveAnimation(text_gameTitle, offset);
                moveAnimation(text_scoresHeader.gameObject, offset);
                moveAnimation(text_score1.gameObject, offset);
                moveAnimation(text_score2.gameObject, offset);
                moveAnimation(text_score3.gameObject, offset);
                moveAnimation(leftHandModel, offset);
                moveAnimation(rightHandModel, offset);

                moveAnimation(text_yourScoreHeader.gameObject, offset);
                moveAnimation(text_yourScore.gameObject, offset);
            } else
            {
                bg.transform.position = bg_StartPos;
                text_gameTitle.transform.position = text_gameTitle_StartPos;
                text_scoresHeader.transform.position = text_scoresHeader_StartPos;
                text_score1.transform.position = text_score1_StartPos;
                text_score2.transform.position = text_score2_StartPos;
                text_score3.transform.position = text_score3_StartPos;
                leftHandModel.transform.position = leftHandModel_StartPos;
                rightHandModel.transform.position = rightHandModel_StartPos;

                text_yourScoreHeader.transform.position = text_yourScoreHeader_StartPos;
                text_yourScore.transform.position = text_yourScore_StartPos;

                if(restartAllowed)
                {
                    _isGameOver = false;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnApplicationQuit()
    {
        noHandDetected();
    }

    public void noHandDetected()
    {
        leftHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
        rightHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
        handAnimation = true;
        leftHandAnimation = true;
        rightHandAnimation = true;

        twoHandsTime = 0f;
        game.setIsStartingProcedure(false);

        if(_isGameOver)
        {
            restartAllowed = true;
        }
    }

    public void oneHandDetected(Hand hand)
    {
        if(hand.IsLeft)
        {
            leftHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
            rightHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
            leftHandAnimation = false;
        } else
        {
            rightHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
            leftHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
            rightHandAnimation = false;
        }

        twoHandsTime = 0f;
        game.setIsStartingProcedure(false);
    }

    public void twoHandsDetected(float deltaTime)
    {
        if(twoHandsTime < 0.000001f)
        {
            leftHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
            rightHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
            leftHandAnimation = false;
            rightHandAnimation = false;
        }
        
        if(!_isGameOver)
        {
            if (bg.transform.position.z > invisibleZ)
            {
                Vector3 offset = new Vector3(0f, 0f, -0.2f);
                moveAnimation(bg, offset);
                moveAnimation(text_gameTitle, offset);
                moveAnimation(text_scoresHeader.gameObject, offset);
                moveAnimation(text_score1.gameObject, offset);
                moveAnimation(text_score2.gameObject, offset);
                moveAnimation(text_score3.gameObject, offset);
                moveAnimation(leftHandModel, offset);
                moveAnimation(rightHandModel, offset);

                if(restartAllowed)
                {
                    moveAnimation(text_yourScoreHeader.gameObject, offset);
                    moveAnimation(text_yourScore.gameObject, offset);
                }

                if (!game.isStartingProcedure() && bg.transform.position.z < -8f)
                {
                    game.setIsStartingProcedure(true);
                }
            }
            else
            {
                twoHandsTime += deltaTime;

                if (twoHandsTime >= 5f)
                {
                    text_countdown.transform.position = new Vector3(text_countdown.transform.position.x, text_countdown.transform.position.y, invisibleZ);
                    restartAllowed = false;
                    game.startGame();
                }
                else if (twoHandsTime >= 4f)
                {
                    text_countdown.updateText("1");
                }
                else if (twoHandsTime >= 3f)
                {
                    text_countdown.updateText("2");
                }
                else if (twoHandsTime >= 2f)
                {
                    text_countdown.updateText("3");
                }
                else if (twoHandsTime >= 1f)
                {
                    text_countdown.updateText("4");
                }
            }
        }
        
    }

    private void moveAnimation(GameObject gobj, Vector3 offset)
    {
        gobj.transform.position = new Vector3(gobj.transform.position.x + offset.x, gobj.transform.position.y + offset.y, gobj.transform.position.z + offset.z);
    }

    public void updateScores(string time)
    {
        System.DateTime now = System.DateTime.Now;
        string newScore = time + " - " + now.Day + "." + now.Month + "." + now.Year + " " + now.Hour + ":" + now.Minute;

        text_yourScore.updateText(newScore);

        string score1 = PlayerPrefs.GetString("score1");
        string score2 = PlayerPrefs.GetString("score2");
        string score3 = PlayerPrefs.GetString("score3");

        //Debug.Log("Score1: " + score1 + " Score2: " + score2 + " Score3: " + score3);

        if(scoreGreaterThan(newScore, score1))
        {
            PlayerPrefs.SetString("score2", PlayerPrefs.GetString("score1"));
            PlayerPrefs.SetString("score3", PlayerPrefs.GetString("score2"));
            PlayerPrefs.SetString("score1", newScore);
        }
        else if(scoreGreaterThan(newScore, score2))
        {
            PlayerPrefs.SetString("score3", PlayerPrefs.GetString("score2"));
            PlayerPrefs.SetString("score2", newScore);
        }
        else if (scoreGreaterThan(newScore, score3))
        {
            PlayerPrefs.SetString("score3", newScore);
        }

        text_score1.updateText("1.\t\t" + PlayerPrefs.GetString("score1"));
        text_score2.updateText("2.\t\t" + PlayerPrefs.GetString("score2"));
        text_score3.updateText("3.\t\t" + PlayerPrefs.GetString("score3"));
    }

    private bool scoreGreaterThan(string newScore, string playerPrefsScore)
    {
        string scoreTime = playerPrefsScore.Substring((playerPrefsScore.IndexOf(':') - 2), 5);
        scoreTime = scoreTime.Remove(scoreTime.IndexOf(':'), 1);
        int scoreTime_Num = int.Parse(scoreTime);
        //Debug.Log(scoreTime);

        string newScoreTime = newScore.Substring((newScore.IndexOf(':') - 2), 5);
        newScoreTime = newScoreTime.Remove(newScoreTime.IndexOf(':'), 1);
        int newScoreTime_Num = int.Parse(newScoreTime);
        //Debug.Log(newScoreTime);

        return newScoreTime_Num > scoreTime_Num;
    }

    public void resetScores()
    {
        PlayerPrefs.SetString("score1", "00:00 - 00.00.000 00:00");
        PlayerPrefs.SetString("score2", "00:00 - 00.00.000 00:00");
        PlayerPrefs.SetString("score3", "00:00 - 00.00.000 00:00");
        updateScores("00:00");
    }

    public void setIsGameOver(bool isGameOver)
    {
        _isGameOver = isGameOver;

        handAnimation = true;
        leftHandAnimation = true;
        rightHandAnimation = true;
        handAnimation_Wait = false;
        handAnimation_WaitTime = 0f;

        twoHandsTime = 0f;

        text_countdown.transform.position = text_countdown_StartPos;
    }
}
