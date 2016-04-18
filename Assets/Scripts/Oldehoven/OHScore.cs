// This file is part of Leeuwarden-2018
// 
// Copyright (c) 2016 sietze greydanus
// 
// Leeuwarden-2018 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3, as
// published by the Free Software Foundation.
// 
// Leeuwarden-2018 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Leeuwarden-2018. If not, see <http://www.gnu.org/licenses/>.
// 

using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     The script for counting the score
/// </summary>
public class OHScore : MonoBehaviour
{
    /// <summary>
    ///     True if the game has ended.
    /// </summary>
    private bool _end;

    /// <summary>
    ///     The end screen.
    /// </summary>
    private GameObject _endScreen;

    /// <summary>
    ///     The score text.
    /// </summary>
    private Text _scoreText;

    private float _timeSurvived;

    /// <summary>
    ///     The amount of toeristen left.
    /// </summary>
    private int _toeristen;

    private void Start()
    {
        _endScreen = GameObject.Find("EndSceen");
        _endScreen.SetActive(false);
        _scoreText = GameObject.Find("Score").GetComponent<Text>();
        _scoreText.text = "Time " + _timeSurvived;
        _toeristen = GameObject.FindGameObjectsWithTag("Toerist").Length;
    }

    public void Update()
    {
        if (!_end)
        {
            _timeSurvived += Time.deltaTime;
            _scoreText.text = "Time " + Mathf.Floor(_timeSurvived);
        }
    }

    public void toeristHasFallen()
    {
        _toeristen--;
        print(_toeristen);
        if (_toeristen <= 0)
        {
            _end = true;
            _endScreen.SetActive(true);
            _endScreen.GetComponentInChildren<Text>().text = "Time " + Mathf.Floor(_timeSurvived);
        }
    }
}