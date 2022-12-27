using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TimerPoint : MonoBehaviour
{
    public float yPos;
    public void Default(bool isAllAtOnce)
    {
        gameObject.SetActive(isAllAtOnce);
    }
    private void Awake()
    {
        yPos = transform.position.y;
    }
    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, yPos - (Mathf.Sin(Time.time) * .5f), transform.position.z);
        transform.Rotate(0, Time.fixedDeltaTime * 100, 0, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            TimerItem.activeTimer.AddPoint();
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }
}

