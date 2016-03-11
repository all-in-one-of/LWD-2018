using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// The scenes that belong to the locations
    /// </summary>
    private readonly Dictionary<string, string> _gameLocations = new Dictionary<string, string>
    {
        {"De Blokhuispoort", "BHPGame"},
        {"Oldehove", "Oldehove"}
    };

    /// <summary>
    /// The locations that belong to the coordinates
    /// </summary>
    private readonly Dictionary<Vector2, string> _locations = new Dictionary<Vector2, string>
    {
        {new Vector2(53.189800f, 5.783395f), "Friesland College"},
        {new Vector2(53.196654f, 5.792925f), "Station Leeuwarden"},
        {new Vector2(53.199829f, 5.800070f), "De Blokhuispoort"},
        {new Vector2(53.199627f, 5.794518f), "Fries Museum"},
        {new Vector2(53.204059f, 5.795633f), "Natuurmuseum Fryslân"},
        {new Vector2(53.204305f, 5.801764f), "Bonifatiuskerk"},
        {new Vector2(53.200166f, 5.789036f), "Stadsschouwburg De Harmonie"},
        {new Vector2(53.202957f, 5.789702f), "Oldehove"}
    };

    private string _closestLocation;

    private float _timer;
    public GameObject obj;
    public GameObject PlayGameButton;
    public Text Text;

    public void Start()
    {
        PlayGameButton.SetActive(false);

        foreach (var location in _locations)
        {
            var o =
                Instantiate(Resources.Load("Place"), ToUnityCoordinates(location.Key), Quaternion.identity) as
                    GameObject;
            o.GetComponent<TextMesh>().text = location.Value.Length < 7
                ? location.Value.Substring(0, 7)
                : location.Value;
        }
        Text.text = "Start";

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Text.text = "location is disabled!";
            Text.color = Color.red;
        }

        // Start service before querying location
        Input.location.Start();
    }

    public void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _timer = 2.5f;

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Text.text = "Unable to determine device location";
                Text.color = Color.red;
            }
            else
            {
                Closest(Input.location.lastData.latitude, Input.location.lastData.longitude);
                obj.transform.position = ToUnityCoordinates(new Vector2(Input.location.lastData.latitude,
                    Input.location.lastData.longitude));
            }
        }
    }

    /// <summary>
    /// Plays the selected game.
    /// </summary>
    public void PlayGame()
    {
        var g = _gameLocations[_closestLocation];
        print(g);

        if (g != null)
        {
            SceneManager.LoadScene(g);
        }
    }

    /// <summary>
    /// Gets the clostest location with the specified latitude & longitude.
    /// </summary>
    /// <param name="latitude">The latitude.</param>
    /// <param name="longitude">The longitude.</param>
    private void Closest(float latitude, float longitude)
    {
        var tMin = "";
        var minDist = Mathf.Infinity;
        var curLoc = new Vector2(latitude, longitude);

        foreach (var location in _locations)
        {
            var dist = Vector2.Distance(location.Key, curLoc);
            if (dist < minDist)
            {
                tMin = location.Value;
                minDist = dist;
            }
        }

        Text.text = "The closest location is: " + tMin;
        _closestLocation = tMin;
        string s;
        _gameLocations.TryGetValue(tMin, out s);

        if (s != null)
        {
            PlayGameButton.SetActive(true);
        }
        else
        {
            PlayGameButton.SetActive(false);
        }
    }

    /// <summary>
    /// Converts realworld to unity coordinates.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns> A new vector with unity coordinates</returns>
    private Vector3 ToUnityCoordinates(Vector2 position)
    {
        //53.201233, 5.799913
        position.x -= 53.201233f;
        position.y -= 5.799913f;
        position *= 1000;
        return new Vector2(position.y, position.x);
    }
}