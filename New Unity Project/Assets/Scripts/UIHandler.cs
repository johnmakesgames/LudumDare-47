using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.ComponentModel.Design.Serialization;

public class UIHandler : MonoBehaviour
{
    public Image BlueAlive;
    public Image GreenAlive;
    public Image RedAlive;
    public Image YellowAlive;
    public Image PinkAlive;
    public Image OrangeAlive;
    public Image PurpleAlive;
    public Sprite WhitenoiseImage;
    public Image canvasScreen;
    public GameObject[] buttonsToSetActiveOnStart;


    string SceneToMoveTo = "";
    float moveToSceneTimer;
    bool starting = true;

    void Start()
    {
        SetImageAlpha(BlueAlive, 0);
        SetImageAlpha(GreenAlive, 0);
        SetImageAlpha(RedAlive, 0);
        SetImageAlpha(YellowAlive, 0);
        SetImageAlpha(PinkAlive, 0);
        SetImageAlpha(OrangeAlive, 0);
        SetImageAlpha(PurpleAlive, 0);
        SetImageAlpha(canvasScreen, 1);

        foreach(var button in buttonsToSetActiveOnStart)
        {
            button.SetActive(false);
        }
    }

    void IncreaseAlphaOnImage(Image image, float increase)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + increase);
    }

    void SetImageAlpha(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    void Update()
    {
        if (starting)
        {
            if (canvasScreen.color.a > 0)
            {
                IncreaseAlphaOnImage(canvasScreen, Time.deltaTime * -1);

                IncreaseAlphaOnImage(BlueAlive, Time.deltaTime);
                IncreaseAlphaOnImage(GreenAlive, Time.deltaTime);
                IncreaseAlphaOnImage(RedAlive, Time.deltaTime);
                IncreaseAlphaOnImage(YellowAlive, Time.deltaTime);
                IncreaseAlphaOnImage(PinkAlive, Time.deltaTime);
                IncreaseAlphaOnImage(OrangeAlive, Time.deltaTime);
                IncreaseAlphaOnImage(PurpleAlive, Time.deltaTime);
            }
            else
            {
                starting = false;
                foreach (var button in buttonsToSetActiveOnStart)
                {
                    button.SetActive(true);
                }
            }
        }


        if (SceneToMoveTo != "")
        {
            moveToSceneTimer += Time.deltaTime;

            if (moveToSceneTimer > 2)
            {
                SceneManager.LoadScene(SceneToMoveTo);
            }

            foreach (var button in buttonsToSetActiveOnStart)
            {
                button.SetActive(false);
            }

            IncreaseAlphaOnImage(canvasScreen, Time.deltaTime);

            IncreaseAlphaOnImage(BlueAlive, Time.deltaTime * -1);
            IncreaseAlphaOnImage(GreenAlive, Time.deltaTime * -1);
            IncreaseAlphaOnImage(RedAlive, Time.deltaTime * -1);
            IncreaseAlphaOnImage(YellowAlive, Time.deltaTime * -1);
            IncreaseAlphaOnImage(PinkAlive, Time.deltaTime * -1);
            IncreaseAlphaOnImage(OrangeAlive, Time.deltaTime * -1);
            IncreaseAlphaOnImage(PurpleAlive, Time.deltaTime * -1);
        }
    }

    public void AccuseNotKiller()
    {
        SceneToMoveTo = SceneManager.GetActiveScene().name;
    }

    public void AccuseKiller()
    {
        SceneToMoveTo = "MainMenu";
    }

    public void SignalDeath(string deadNPCName)
    {
        switch (deadNPCName)
        {
           case "Blue":
                SetImageWhiteNoise(BlueAlive);
                break;
            case "Green":
                SetImageWhiteNoise(GreenAlive);
                break;
            case "Red":
                SetImageWhiteNoise(RedAlive);
                break;
            case "Yellow":
                SetImageWhiteNoise(YellowAlive);
                break;
            case "Pink":
                SetImageWhiteNoise(PinkAlive);
                break;
            case "Orange":
                SetImageWhiteNoise(OrangeAlive);
                break;
            case "Purple":
                SetImageWhiteNoise(PurpleAlive);
                break;
            default:
                break;
        }
    }

    private void SetImageWhiteNoise(Image imageToSet)
    {
        imageToSet.color = Color.white;
        imageToSet.sprite = WhitenoiseImage;
    }


}
