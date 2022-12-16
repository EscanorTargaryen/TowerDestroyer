using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text youwon;

    public GameObject canvas;
    public static GameOverScreen instance;
    public static bool GameOver;

    private void Awake()
    {
        instance = this;
    }


    private void setup()
    {
        GameOver = true;
        canvas.SetActive(true);
    }

    public void won()
    {
        setup();

        youwon.gameObject.SetActive(true);
    }

    void Start()
    {
        canvas.SetActive(false);
    }
    
}