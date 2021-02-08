using UnityEngine;

public class MathHelper : MonoBehaviour
{
    public static Vector3 VSmoothInterp(Vector3 from, Vector3 to, float maxSpeed, float acceleration, float deltaTime, ref Vector3 currentVelocity)
    {
        float speed = currentVelocity.magnitude;
        speed = Mathf.Clamp(speed + acceleration * deltaTime, speed, maxSpeed);

        float distance = Mathf.Clamp(speed, speed, (to - from).magnitude);

        return from + (to-from).normalized * distance;
    }
}
