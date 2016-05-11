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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Oldehoven
{
    /// <summary>
    ///     The script for counting the score
    /// </summary>
    public class Score : MonoBehaviour
    {
        /// <summary>
        ///     True if the game has ended.
        /// </summary>
        private bool _countScore = true;

        /// <summary>
        ///     The end screen.
        /// </summary>
        private GameObject _scoreScreen;

        /// <summary>
        ///     The score text.
        /// </summary>
        private Text _textObj;

        public int Level;

        private int _lives = 3;

        private float _timeLeft = 30;

        /// <summary>
        ///     The amount of touristen left.
        /// </summary>
        private int _touristsLeft;

        private TouristAi[] _touristAis;

        private GameObject _oldehoven;

        private void Start()
        {
            _scoreScreen = GameObject.Find("EndSceen");
            _scoreScreen.SetActive(false);
            _textObj = GameObject.Find("Score").GetComponent<Text>();
            _textObj.text = "Time " + Mathf.Floor(_timeLeft) + "\nLives Left " + _lives;
            var touristen = GameObject.FindGameObjectsWithTag("Toerist");
            _touristsLeft = touristen.Length;
            _touristAis = touristen.Select(o => o.GetComponent<TouristAi>()).ToArray();
            _oldehoven = GameObject.Find("Oldehoven");
        }

        private void Update()
        {
            if (_countScore)
            {
                _timeLeft -= Time.deltaTime;
                _textObj.text = "Time " + Mathf.Floor(_timeLeft) + "\nLives Left " + _lives;
                if (_timeLeft <= 0) StartCoroutine(ResetLevel(true));
            }
        }

        public void TouristHasFallen()
        {
            _touristsLeft--;
            print(_touristsLeft);
            if (_touristsLeft <= 0) StartCoroutine(ResetLevel(false));
        }

        IEnumerator ResetLevel(bool levelup)
        {
            if (levelup)
            {
                Level++;
            }
            else
            {
                _lives --;
            }

            _countScore = false;
            _scoreScreen.SetActive(true);
            _touristsLeft = _touristAis.Length;

            _scoreScreen.GetComponentInChildren<Text>().text = "Current level " + Level + "\nLives Left " + _lives;

            yield return new WaitForSeconds(5);

            _oldehoven.transform.rotation=Quaternion.identity;

            if (_lives <= 0)
            {
                SceneManager.LoadScene(0);
                yield break;
            }

            foreach (var touristAi in _touristAis)
            {
                touristAi.Reset();
            }

            _countScore = true;
            _scoreScreen.SetActive(false);
            _timeLeft = 30;
        }
    }
}