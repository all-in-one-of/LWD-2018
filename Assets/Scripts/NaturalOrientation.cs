using UnityEngine;

public class NaturalOrientation : MonoBehaviour
{
    public static int OrientationUndefined = 0x00000000;
    public static int OrientationPortrait = 0x00000001;
    public static int OrientationLandscape = 0x00000002;

    public static int Rotation0 = 0x00000000;
    public static int Rotation180 = 0x00000002;
    public static int Rotation270 = 0x00000003;
    public static int Rotation90 = 0x00000001;

    public static int Portrait = 0;
    public static int PortraitUpsidedown = 1;
    public static int Landscape = 2;
    public static int LandscapeLeft = 3;

#if UNITY_ANDROID

    private AndroidJavaObject _mConfig;
    private AndroidJavaObject _mWindowManager;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    //adapted from http://stackoverflow.com/questions/4553650/how-to-check-device-natural-default-orientation-on-android-i-e-get-landscape/4555528#4555528
    public int GetDeviceDefaultOrientation()
    {
        if ((_mWindowManager == null) || (_mConfig == null))
        {
            using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").
                GetStatic<AndroidJavaObject>("currentActivity"))
            {
                _mWindowManager = activity.Call<AndroidJavaObject>("getSystemService", "window");
                _mConfig = activity.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getConfiguration");
            }
        }

        var lRotation = _mWindowManager.Call<AndroidJavaObject>("getDefaultDisplay").Call<int>("getRotation");
        var dOrientation = _mConfig.Get<int>("orientation");

        if ((((lRotation == Rotation0) || (lRotation == Rotation180)) && (dOrientation == OrientationLandscape)) ||
            (((lRotation == Rotation90) || (lRotation == Rotation270)) && (dOrientation == OrientationPortrait)))
        {
            return Landscape; //TABLET
        }

        return Portrait; //PHONE
    }

#endif
}