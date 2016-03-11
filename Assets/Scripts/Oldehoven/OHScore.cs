using UnityEngine;
using UnityEngine.UI;

public class OHScore : MonoBehaviour
{
    private Text _scoreText;
    private GameObject _endScreen;
    private int _toeristen;
    private float _timeSurvived;
    private bool _end = false;

    private void Start()
    {
        _endScreen = GameObject.Find("EndSceen");
        _endScreen.SetActive(false);
        _scoreText = GameObject.Find("Score").GetComponent<Text>();
        _scoreText.text = "Time " + _timeSurvived;
    }

    public void Update()
    {
        _timeSurvived += Time.deltaTime;
        if (!_end) _scoreText.text = "Time " + Mathf.Floor(_timeSurvived);
    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        _toeristen = GameObject.FindGameObjectsWithTag("Toerist").Length - 1;
        print(_toeristen);
        if (_toeristen == 0)
        {
            _end = true;
            _endScreen.SetActive(true);
            _endScreen.GetComponentInChildren<Text>().text = "Time " + Mathf.Floor(_timeSurvived);
        }
    }
}