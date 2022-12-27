using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerItem : MonoBehaviour
{
    public static TimerItem activeTimer;

    public Transform pointsParent;
    public Mode mode;
    public float Time = 0;

    public Collider collider;
    public GameObject renderer;

    List<TimerPoint> points = new List<TimerPoint>();
    int pointsRecieved = 0;

    public enum Mode
    {
        AllAtOnce, OneAtATime
    }
    private void Awake()
    {
        if (mode == Mode.AllAtOnce)
        {
            points = new List<TimerPoint>(pointsParent.gameObject.GetComponentsInChildren<TimerPoint>());
            pointsParent.gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < pointsParent.childCount; i++)
            {
                GameObject g = pointsParent.GetChild(i).gameObject;
                points.Add(g.GetComponent<TimerPoint>());
                g.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") & !activeTimer)
        {
            renderer.SetActive(false);
            collider.enabled = false;
            activeTimer = this;
            if(mode == Mode.AllAtOnce)
            {
                pointsParent.gameObject.SetActive(true);
            }
            else
            {
                points[0].gameObject.SetActive(true);
            }
        }
        StartCoroutine(Timer());
    }
    public void AddPoint()
    {
        if (points.Count <= ++pointsRecieved)
            gameObject.SetActive(false);
        else if(mode == Mode.OneAtATime)
        {
            points[pointsRecieved].gameObject.SetActive(true);
        }
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(Time);
        activeTimer = null;
        foreach (TimerPoint i in points)
        {
            i.Default(mode == Mode.AllAtOnce);
        }
        pointsParent.gameObject.SetActive(mode != Mode.AllAtOnce);
        renderer.SetActive(true);
        collider.enabled = true;
        pointsRecieved = 0;
    }
}
