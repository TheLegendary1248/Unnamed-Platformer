using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Camera cam;
    public static CamFollow self;
    public static Transform CamTransform;
    public float CameraHeight;
    public float CameraDistanceFromPlayer;
    public float LerpAmount;
    private void Start()
    {
        CamTransform = transform;
        self = this;
    }
    void FixedUpdate()
    {
        CameraHeight = Player.verticalRot;
        CameraDistanceFromPlayer = Player.verticalRot * 2;
        Vector3 off = new Vector3(Mathf.Cos(Player.horizontalRot), CameraHeight / CameraDistanceFromPlayer, Mathf.Sin(Player.horizontalRot)) * CameraDistanceFromPlayer;
        Vector3 offset = Player.playerTransform.position + off;
        transform.position = Vector3.Lerp( transform.position, offset, LerpAmount);
        transform.LookAt(Player.playerTransform.position + new Vector3(0,3,0)) ;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.GetComponent<EnemyDrone>() is EnemyDrone E)
                {
                    E.Kill();
                }
            }
        }
    }
}
