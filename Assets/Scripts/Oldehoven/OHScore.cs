using UnityEngine;
using UnityEngine.UI;

public class OHScore : MonoBehaviour
{
    private Text _text;
    public int Score;

    private void Start()
    {
        _text = GameObject.Find("Score").GetComponent<Text>();
        _text.text = "Score " + Score;
    }

    public void UpdateScore(int s)
    {
        Score += s;
        _text.text = "Score " + Score;
    }
}