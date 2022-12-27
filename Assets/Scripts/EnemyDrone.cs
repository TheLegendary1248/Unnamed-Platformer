using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Vector3 _respawnPos = Vector3.zero;
    protected enum State
    {
        Sleep,
        Awake,
        Killed
    }
    public virtual void Kill() { }
}
public class EnemyDrone : Enemy, IFallable
{
    float _respawnTime = 5f;
    public float respawnTime => _respawnTime;
    public Vector3 respawnPos => _respawnPos;
    public Rigidbody rb;
    public float Lev = 1;
    public float For = 10;
    State EnemyState = State.Awake;
    private void FixedUpdate()
    {
        Vector3 dif = transform.position - Player.playerTransform.position;
        float AngDif = Mathf.Repeat(transform.eulerAngles.y, 360) - (Mathf.Atan2(dif.x, dif.z) * Mathf.Rad2Deg);
        float Dist = Vector3.Distance(transform.position, Player.playerTransform.position);
        if (EnemyState != State.Sleep)
        {


            rb.AddRelativeForce(((-Physics.gravity * Lev) + (Vector3.back * For)) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            velocity = velocity.normalized * 25;
            rb.velocity = new Vector3(velocity.x, Mathf.Clamp(rb.velocity.y,-20,20), velocity.z);
            Lev = Mathf.Clamp(((Player.playerTransform.position.y - transform.position.y) / 50) + 1.5f + Mathf.Clamp((Dist / 150),.2f,1f), 1.2f, 2.5f);
        }
        if (EnemyState == State.Awake)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - AngDif, transform.eulerAngles.z);
            transform.localEulerAngles = new Vector3(-Mathf.Clamp(Dist/3, 18, 27), transform.localEulerAngles.y, 0);
        }
        if (transform.position.y < Setting.DestructionLevel) StartCoroutine(Return());
        //rb.AddTorque(new Vector3(0,  AngDif * Time.fixedDeltaTime * Rot, 0));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (EnemyState == State.Killed)
        {
            EnemyState = State.Sleep;
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        Kill();
    }
    public override void Kill()
    {
        rb.freezeRotation = false;
        EnemyState = State.Killed;
        rb.AddTorque(Random.onUnitSphere * 2000, ForceMode.VelocityChange);
        rb.AddForce(Random.onUnitSphere * 50, ForceMode.VelocityChange);
    }
    public IEnumerator Return()
    {
        int layer = gameObject.layer;
        rb.detectCollisions = false;
        rb.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("Shadow Realm");
        transform.position = respawnPos;
        transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(respawnTime);
        rb.detectCollisions = true;
        rb.isKinematic = false;
        gameObject.layer = layer;
        EnemyState = State.Awake;
    }
    void Start()
    {
        _respawnPos = transform.position;       
    }
}
/// <summary>
/// Used with objects that can fall off the stage
/// </summary>
public interface IFallable
{
    float respawnTime { get; }
    Vector3 respawnPos { get; }
}
public interface ISleep
{

}
public static class Setting
{
    public static float DestructionLevel = -100f;
}