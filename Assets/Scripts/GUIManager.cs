using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    public Text centerMessage;
    public Text BalanceText;
    //private bool showRestart = false;

    public GameObject IntroObject;
    public GameObject ActionsObject;
    public GameObject RestartButton;

    public static GUIManager Instance;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (Input.touchCount > 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    print(hit.transform.tag);
                    //SceneManager.LoadScene(1);
                    if(!hit.transform.tag.Equals("Untagged"))
                        GameManager.Instance.LoadMainStage(hit.transform.tag);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    print(hit.transform.tag);
                    //SceneManager.LoadScene(1);
                    if (!hit.transform.tag.Equals("Untagged"))
                        GameManager.Instance.LoadMainStage(hit.transform.tag);
                }
            }
        }
    }


    public void UpdateBalance()
    {
        BalanceText.text = "Balance: " + GameManager.Instance.PlayerBalance + "$";
    }

    public void ShowPlayerLostMessage()
    {
        RestartButton.SetActive(true);
        ActionsObject.SetActive(false);
        centerMessage.text = "PLAYER LOST";
    }

    public void ShowPlayerWonMessage()
    {
        RestartButton.SetActive(true);
        ActionsObject.SetActive(false);
        centerMessage.text = "PLAYER WIN!!!";
    }

    public void ShowTie()
    {
        RestartButton.SetActive(true);
        ActionsObject.SetActive(false);
        centerMessage.text = "TIE";
    }



    public void Hit()
    {
        GameManager.Instance.Hit();
    }

    public void Stand()
    {
        GameManager.Instance.Stand();
    }

    public void Double()
    {
        GameManager.Instance.Double();
    }

    public void Surrender()
    {
        GameManager.Instance.Surrender();
    }

    public void Restart()
    {
        RestartButton.SetActive(false);
        ActionsObject.SetActive(true);
        GameManager.Instance.Restart();
        centerMessage.text = "";
    }

    public void ShowActionButtons()
    {
        ActionsObject.SetActive(true);
    }

    public void ARTargetFound()
    {
        IntroObject.SetActive(false);
        ActionsObject.SetActive(true);
        BalanceText.gameObject.SetActive(true);
    }

    public void ARTargetLost()
    {
        IntroObject.SetActive(true);
        ActionsObject.SetActive(false);
    }


    /*
    void OnGUI()
    {
        if (!showRestart)
        {
            if (GUI.Button((new Rect(0, 0, 200, 100)), "HIT"))
            {
                GameManager.Instance.Hit();
            }
            if (GUI.Button((new Rect(200, 0, 200, 100)), "Stand"))
            {
                GameManager.Instance.Stand();
            }
            if (GUI.Button((new Rect(400, 0, 200, 100)), "Double"))
            {
                GameManager.Instance.Double();
            }
            if (GUI.Button((new Rect(600, 0, 200, 100)), "Surrender"))
            {
                GameManager.Instance.Surrender();
            }
        }
        else
        {
            if (GUI.Button((new Rect(600, 0, 200, 100)), "Bet 10$"))
            {
                showRestart = false;
                GameManager.Instance.Restart();
                centerMessage.text = "";
            }
        }
        
    }*/
}
