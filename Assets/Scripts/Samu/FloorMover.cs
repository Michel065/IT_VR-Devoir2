using UnityEngine;

public class FloorMover : MonoBehaviour
{
    public Transform target; // un point vers lequel le plancher glissera
    public float speed = 2f;
    private bool move = false;

    void Update()
    {
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.position) < 0.01f)
                move = false;
        }
    }

    public void StartMoving()
    {
        move = true;
    }
}
