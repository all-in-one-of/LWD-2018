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
///     Counts the score ofthe BHP game
/// </summary>
public class BHPScoreCounter : MonoBehaviour
{
    /// <summary>
    ///     The amount of prisoners escaped
    /// </summary>
    private static int _escaped;

    /// <summary>
    ///     The amount of prisoners not escaped
    /// </summary>
    private static int _notEscaped;

    /// <summary>
    ///     True if the game has ended
    /// </summary>
    private static bool _end;

    /// <summary>
    ///     The final score
    /// </summary>
    private GameObject _endScore;

    /// <summary>
    ///     The text that displays the final score
    /// </summary>
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

    /// <summary>
    ///     Updates the score.
    /// </summary>
    /// <param name="escaped">if set to <c>true</c> [escaped].</param>
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