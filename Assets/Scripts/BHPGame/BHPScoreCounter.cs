using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Counts the score ofthe BHP game
/// </summary>
public class BHPScoreCounter : MonoBehaviour
{
    private static int _escaped;
    private static int _notEscaped;
    private static bool _end;
    private GameObject _endScore;
    private Text _endScoreText;

    private int _score;
    private Text _scoreText;
    private float _timer = 60;

    public void Start()
    {
        _endScore = GameObject.Find("EndScore");
        _endScoreText = _endScore.GetComponentInChildren<Text>();
        _endScore.SetActive(false);
        _scoreText = GetComponent<Text>();
        _scoreText.text = "0";
    }

    public void Update()
    {
        if (_timer > 0)
            _timer -= Time.deltaTime;

        _scoreText.text = "Score " + (_notEscaped - _escaped) + "\nTime left " + Mathf.Floor(_timer);

        if (_timer <= 0 && !_end)
        {
            _endScore.SetActive(true);
            _end = true;

            var tmp = "Prisoners not escaped " + _notEscaped;
            tmp += "\nPrisoners escaped " + _escaped;
            tmp += "\nEnd score " + (_notEscaped - _escaped);
            _endScoreText.text = tmp;
        }
    }

    public static void UpdateScore(bool escaped)
    {
        if (_end)
            return;
        if (escaped)
            _escaped++;
        else
            _notEscaped++;
    }
}