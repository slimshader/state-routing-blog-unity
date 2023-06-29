using UnityEngine;

public class Cube : MonoBehaviour
{
    public Material _material;
    private bool isIn;

    public void In() => isIn = true;

    void Update()
    {
        if (isIn)
        {
            _material.color = Color.red;
        }
    }
}
