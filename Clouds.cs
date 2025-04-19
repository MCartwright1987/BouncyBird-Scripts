using UnityEngine;

public class Clouds : MonoBehaviour
{

    public float speed;
    void Update()
    {
        if (transform.position.x > 18.5)
        {
            transform.position = new Vector2(-18.5f,transform.position.y);
        }

        transform.Translate(Vector2.right * Time.deltaTime * speed);//0.2
    }
}
