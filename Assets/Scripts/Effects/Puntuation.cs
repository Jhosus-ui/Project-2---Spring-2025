using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance;
    public TMP_Text scoreText;

    private int currentScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AgregarPuntos(int puntos)
    {
        currentScore += puntos;
        scoreText.text = $"{currentScore}";
    }
}