using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PhysicState
    {
        Grounded,
        InAir,
        GettingUpLedge,
        Wallrunning
    }
    public static Player self;
    public static Transform playerTransform;
    public float speed;
    public float camRotSpeed;
    public float jumpForce;
    public Rigidbody rb;
    public Transform body;
    public float maxVelocity;
    static Vector3 spawn;
    public bool hasJumped;
    public static float horizontalRot = 0;
    public static float verticalRot = 5;
    void Start()
    {
        self = this;
        playerTransform = transform;
        horizontalRot = transform.eulerAngles.y;
        spawn = transform.position;
    }
    private void FixedUpdate()
    {       
        RaycastHit hit;
        bool objectUnderneath = Physics.Raycast(transform.position, Vector3.down, out hit, (transform.localScale.y / 2f) + 0.05f);
        hasJumped = objectUnderneath ? false : hasJumped;
        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if(velocity.sqrMagnitude > maxVelocity * maxVelocity & !hasJumped)
        {
            velocity = velocity.normalized * maxVelocity;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }
    }
    private void Update()
    {
        Vector3 camNormal = new Vector3(Mathf.Cos(horizontalRot), 0, Mathf.Sin(horizontalRot));
        Vector3 fwd = camNormal * -Input.GetAxisRaw("Vertical");
        Vector3 strafe = new Vector3(-camNormal.z, 0, camNormal.x) * Input.GetAxisRaw("Horizontal");
        Vector3 added = fwd + strafe;
        added.Normalize();
        rb.AddForce(added * speed * Time.deltaTime * ((Vector3.Dot(rb.velocity.normalized,added)/4) + 1.5f) * (!hasJumped ? 1 : 0.5f), ForceMode.Impulse); // Add in the dot product of velocity and direction to add emphasize on smooth turns
        //rb.AddForce(new Vector3(Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal")) * speed * Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.Space) & !hasJumped)
        {
            rb.velocity = new Vector3(rb.velocity.x * 1.2f, jumpForce, rb.velocity.z * 1.2f);
            hasJumped = true;
            StartCoroutine(AnimJump());
        }
        horizontalRot -= Input.GetAxis("Mouse X") * Time.deltaTime * camRotSpeed;
        verticalRot -= Input.GetAxis("Mouse Y") * Time.deltaTime * camRotSpeed * 2;
        verticalRot = Mathf.Clamp(verticalRot, 1, 9);
        if (Input.GetKeyDown(KeyCode.R))
        {
            Kill();
        }
        body.eulerAngles = new Vector3(body.eulerAngles.x, Mathf.LerpAngle(body.eulerAngles.y,Mathf.Atan2(rb.velocity.x,rb.velocity.z) * Mathf.Rad2Deg, 0.1f), body.eulerAngles.z);
    }
  
    IEnumerator AnimJump()
    {
        float timestamp = Time.time + .5f;
        do
        {
            body.localScale = new Vector3(Mathf.Lerp(1f, 0.2f, Mathf.PingPong(timestamp - Time.time, 0.25f)), Mathf.Lerp(1.8f, 5f, Mathf.PingPong(timestamp - Time.time, 0.25f)),Mathf.Lerp(1f, 0.2f, Mathf.PingPong(timestamp - Time.time, 0.25f)));
            yield return new WaitForFixedUpdate();
        } while (timestamp > Time.time);
        body.localScale = new Vector3(1, 1.8f, 1);
    }
    IEnumerator AnimLanding()
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (true);
    }
    public void Kill() => StartCoroutine(IKill());
    IEnumerator IKill()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.detectCollisions = false;
        gameObject.layer = LayerMask.NameToLayer("Shadow Realm");
        yield return new WaitForSeconds(1f);
        transform.position = spawn;
        gameObject.layer = LayerMask.NameToLayer("Default");
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.None;
        rb.detectCollisions = true;
    }
}
interface IHealth
{
    float health { get; }
    void Damage();
}
interface IEnemy
{
    
}