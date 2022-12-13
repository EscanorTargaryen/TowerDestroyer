using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text youwon;
    public TMP_Text youlost;
    public GameObject canvas;
    public static GameOverScreen instance;
    public static bool GameOver;

    private void Awake()
    {
        instance = this;
    }


    public void lost()
    {
        setup();
        youlost.gameObject.SetActive(true);
        youwon.gameObject.SetActive(false);
    }

    private void setup()
    {
        GameOver = true;
        canvas.SetActive(true);
    }

    public void won()
    {
        setup();
        youlost.gameObject.SetActive(false);
        youwon.gameObject.SetActive(true);
    }

    void Start()
    {
        canvas.SetActive(false);
    }


    void Update()
    {
    }
}