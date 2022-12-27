using System;
using System.Collections;
using UnityEngine;

public class ShapeShifter : MonoBehaviour
{
    enum MoveMode
    {
        Linear,
        Smooth
    }
    [Serializable]
    struct ShifterStruct
    {
        /// <summary>
        /// Vector
        /// </summary>
        public Vector3 vector;
        /// <summary>
        /// Time it takes to reach desire
        /// </summary>
        public float MoveTime;
        /// <summary>
        /// Time before executing
        /// </summary>
        public float WaitTime;
        public float TotalTime { get { return WaitTime + MoveTime; } } 
    }

    [SerializeField]
    ShifterStruct[] Positions;
    public bool positionSmoothTransitions; float positionsTimestamp; int posPointer = 0;
    Vector3 PosLast; Vector3 PosCurrent;
    [SerializeField]
    ShifterStruct[] Rotations;
    public bool rotationSmoothTransition; float rotationsTimestamp; int rotsPointer = 0; public bool ConstantRotation = false;
    public Vector3 rotationBase;
    [SerializeField]
    ShifterStruct[] Sizes;
    public bool sizeSmoothTransition; float sizesTimestamp; int sizePointer = 0;
    public Mesh mesh;
    public Rigidbody rb;
    private void Start()
    {
        positionsTimestamp = Time.time;
        rotationsTimestamp = Time.time;
        sizesTimestamp = Time.time;
        
        if (Positions.Length >= 2)
        {
            PosLast = Positions[0].vector ;
            PosCurrent = Positions[1].vector;
            StartCoroutine(PositionLooper());
        }
        if (Rotations.Length >= 2)
        {
            StartCoroutine(RotationLooper());
            if (true) //Switch to constantrotation once fixed
            {
                Vector3 v = new Vector3();
                for (int i = 0; i < 3; i++)
                {
                    v[i] = Rotations[0].vector[i] - Mathf.DeltaAngle(Rotations[Rotations.Length - 1].vector[i], Rotations[0].vector[i]);
                }
                rotationBase = v;
            }
        }
        if (Sizes.Length >= 2)
        {
            StartCoroutine(SizeLooper());
        }
    }
    /// <summary>
    /// Returns a smooth transition between floor of value and ceiling of value. Unclamped
    /// </summary>
    public static float UnitSlerp(float value)
    {
        value *= 2;
        //(sin(2x * PI + PI) + 2x * PI) / 2PI
        return (Mathf.Sin((value * Mathf.PI) + Mathf.PI) + (value * Mathf.PI))/(Mathf.PI * 2);
    }
    private void FixedUpdate()
    {
        if(Positions.Length >= 2)
        {
            rb.MovePosition(Vector3.Lerp(PosLast, PosCurrent, UnitSlerp((Time.time - positionsTimestamp) / Positions[posPointer].MoveTime)));
            //transform.position = Vector3.Lerp(Positions[posPointer].vector, Positions[posPointer + 1 >= Positions.Length ? 0 : posPointer + 1].vector, positionSmoothTransitions ? UnitSlerp((Time.time - positionsTimestamp) / Positions[posPointer].MoveTime) : (Time.time - positionsTimestamp)/Positions[posPointer].MoveTime);
        }
        if (Rotations.Length >= 2)
        {
            transform.eulerAngles = Vector3.Lerp(ConstantRotation & rotsPointer == Rotations.Length - 1 ? rotationBase : Rotations[rotsPointer].vector, Rotations[rotsPointer + 1 >= Rotations.Length ? 0 : rotsPointer + 1].vector, (Time.time - rotationsTimestamp) / Rotations[rotsPointer].MoveTime);
        }
        if (Sizes.Length >= 2)
        {
            transform.localScale = Vector3.Lerp(Sizes[sizePointer].vector, Sizes[sizePointer + 1 >= Sizes.Length ? 0 : sizePointer + 1].vector, (Time.time - sizesTimestamp) / Sizes[sizePointer].MoveTime);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (!(Positions.Length >= 2) & mesh)
            return;
        for (int i = 0; i < Positions.Length; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireMesh(mesh, Positions[i].vector, transform.rotation, transform.localScale);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Positions[i].vector, Positions[i + 1 >= Positions.Length ? 0 : i + 1].vector);
        }
    }

    IEnumerator PositionLooper()
    {
        yield return new WaitForSeconds(Positions[posPointer].TotalTime);
        PosLast = PosCurrent;
        posPointer = posPointer + 1 >= Positions.Length ? 0 : posPointer + 1;
        PosCurrent = Positions[posPointer].vector;
        positionsTimestamp = Time.time;
        StartCoroutine(PositionLooper());
    }
    IEnumerator RotationLooper()
    {
        yield return new WaitForSeconds(Rotations[rotsPointer].TotalTime);
        rotsPointer = rotsPointer + 1 >= Rotations.Length ? 0 : rotsPointer + 1;
        rotationsTimestamp = Time.time;
        StartCoroutine(RotationLooper());
    }
    IEnumerator SizeLooper()
    {
        yield return new WaitForSeconds(Sizes[sizePointer].TotalTime);
        sizePointer = sizePointer + 1 >= Sizes.Length ? 0 : sizePointer + 1;
        sizesTimestamp = Time.time;
        StartCoroutine(SizeLooper());
    }













    /*
    public Vector4[] checkPoints;
    public Vector4[] rotations;
    public Vector4[] sizes;

    float[] timestamps = new float[] { 0f, 0f, 0f};
    int[] pointer = new int[] { 0, 0, 0 };

    bool[] IsUsed = new bool[]{ false, false, false};
    private void Start()
    {
        if (checkPoints.Length > 1)
        {
            IsUsed[0] = true;
            float[] s = new float[checkPoints.Length];
            for (int i = 0; i < checkPoints.Length; i++)
            {
                s[i] = checkPoints[i].w;
            }
            StartCoroutine(Loop(0, s));
        }
        if (rotations.Length > 1)
        {
            IsUsed[1] = true;
            float[] s = new float[rotations.Length];
            for (int i = 0; i < rotations.Length; i++)
            {
                s[i] = rotations[i].w;
            }
            StartCoroutine(Loop(1, s));
        }
        if (sizes.Length > 1)
        {
            IsUsed[2] = true;
            float[] s = new float[sizes.Length];
            for (int i = 0; i < sizes.Length; i++)
            {
                s[i] = sizes[i].w;
            }
            StartCoroutine(Loop(2, s));
        }
    }
    private void FixedUpdate()
    {
        int p = 0;
        if (IsUsed[p])
        {
            transform.position = Vector3.Lerp(checkPoints[pointer[p] == 0 ? checkPoints.Length - 1 : pointer[p] - 1], checkPoints[pointer[p]], (Time.time - timestamps[p]) / checkPoints[pointer[p]].w);
        }
        p++;
        if (IsUsed[p])
        {
            transform.eulerAngles = Vector3.Lerp(rotations[pointer[p] == 0 ? rotations.Length - 1 : pointer[p] - 1], rotations[pointer[p]], (Time.time - timestamps[p]) / rotations[pointer[p]].w);
        }
        p++;
        if (IsUsed[p])
        {
            transform.localScale = Vector3.Lerp(sizes[pointer[p] == 0 ? sizes.Length - 1 : pointer[p] - 1], sizes[pointer[p]], (Time.time - timestamps[p]) / sizes[pointer[p]].w);
        }
    }
    IEnumerator Loop(int i, float[] times)
    {
        while (true)
        {
            timestamps[i] = Time.time;
            yield return new WaitForSeconds(times[i]);
            i = i++ % times.Length;
            pointer[i] = i;
        }
    }
    */
}
