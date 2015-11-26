using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    private int _score;
    private Text Text;

    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            Text.text = _score + "";
        }
    }

    public void Start()
    {
        Text = GetComponent<Text>();
        Text.text = "0";
    }
}