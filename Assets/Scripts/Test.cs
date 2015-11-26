using UnityEngine;

public class Test : MonoBehaviour
{
    public AnimationClip Clip;

    private void Start()
    {
        print(Clip.length);
    }
}