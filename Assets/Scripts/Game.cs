using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject obj;
    public Text Text;

    private readonly Dictionary<Vector2, string> _locations = new Dictionary<Vector2, string>
    {
        {new Vector2(53.189800f, 5.783395f), "Friesland College"},
        {new Vector2(53.196654f, 5.792925f), "Station Leeuwarden"},
        {new Vector2(53.199829f, 5.800070f), "De Blokhuispoort"},
        {new Vector2(53.199627f, 5.794518f), "Fries Museum"}
    };

    public void Start()
    {
        foreach (var location in _locations)
        {
            var o = Instantiate(Resources.Load("Place"), NormalizeLatLong(location.Key), Quaternion.identity) as GameObject;
            o.GetComponent<TextMesh>().text = location.Value;
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
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Text.text = "Unable to determine device location";
            Text.color = Color.red;
        }

    }

    public void Poll()
    {
        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Text.text = "Unable to determine device location";
            Text.color = Color.red;
        }
        else
        {
            Closest(Input.location.lastData.latitude, Input.location.lastData.longitude);
            obj.transform.position = NormalizeLatLong(new Vector2(Input.location.lastData.latitude,
                Input.location.lastData.longitude));
        }
    }

    private void Closest(float latitude, float longitude)
    {
        var tMin = "";
        var minDist = Mathf.Infinity;
        var curLoc = new Vector2(latitude, longitude);
        foreach (var location in _locations)
        {
            float dist = Vector2.Distance(location.Key, curLoc);
            if (dist < minDist)
            {
                tMin = location.Value;
                minDist = dist;
            }
        }
        Text.text = "The closest location is: " + tMin;
    }

    private Vector3 NormalizeLatLong(Vector2 position)
    {
        //53.201233, 5.799913
        position.x -= 53.201233f;
        position.y -= 5.799913f;
        position *= 1000;
        return position;
    }
}