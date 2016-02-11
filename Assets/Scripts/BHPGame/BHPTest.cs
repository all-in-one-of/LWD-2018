using UnityEngine;

public class BHPTest : MonoBehaviour
{
    public AnimationClip Clip;

    private void Start()
    {
        print(Clip.length);
    }
}