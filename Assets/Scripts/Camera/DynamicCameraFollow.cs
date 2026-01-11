using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCameraFollow : MonoBehaviour
{
    public List<Transform> players = new List<Transform>();

    public float smoothTime = 0.5f;
    public float minHeight = 15f;
    public float maxHeight = 40f;
    public float padding = 5f;

    private Vector3 velocity = Vector3.zero;
    private float currentHeightVelocity;
    private Camera cam;

    public float verticalOffset = 0f;
    public float horizontalOffset = -3f;

    [Header("Boundary")]
    public BoxCollider boundaryCollider;

    [Header("Player Framing")]
    public float playerBufferRadius = 1.5f; //sphere around each player

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 1; i <= 4; i++)
        {
            string playerName = $"Player {i}(Clone)";
            GameObject found = GameObject.Find(playerName);
            if (found != null)
            {
                AddPlayer(found.transform);
            }
        }
    }

    public void AddPlayer(Transform playerTransform)
    {
        if (!players.Contains(playerTransform))
        {
            players.Add(playerTransform);
        }

        if (players.Count > 0 && !enabled)
        {
            enabled = true;
            SnapToCenter();
        }
    }

    private void SnapToCenter()
    {
        if (players.Count == 0 || cam == null) return;

        Bounds zoomBounds = GetPlayerBounds(true);
        float targetHeight = CalculateRequiredHeight(zoomBounds);

        Vector3 targetPosition = new Vector3(
            zoomBounds.center.x + horizontalOffset,
            targetHeight + verticalOffset,
            zoomBounds.center.z //center on z initially (middle of the screen)
        );

        if (boundaryCollider != null)
        {
            Bounds mapBounds = boundaryCollider.bounds;
            targetPosition.x = Mathf.Clamp(targetPosition.x, mapBounds.min.x, mapBounds.max.x);
            targetPosition.z = Mathf.Clamp(targetPosition.z, mapBounds.min.z, mapBounds.max.z);
        }

        transform.position = targetPosition;
    }

    void LateUpdate()
    {
        if (players.Count == 0 || cam == null) return;

        //get bounds area
        Bounds zoomBounds = GetPlayerBounds(true);

        float targetHeight = CalculateRequiredHeight(zoomBounds);

        //offset postions
        Vector3 desiredPosition = new Vector3(
            zoomBounds.center.x + horizontalOffset,
            targetHeight,
            zoomBounds.center.z + verticalOffset
        );

        //boundaries clamp
        if (boundaryCollider != null)
        {
            Bounds mapBounds = boundaryCollider.bounds;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, mapBounds.min.x, mapBounds.max.x);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, mapBounds.min.z, mapBounds.max.z);
            //Debug.Log("testing");
        }

        // 4. Smooth Damp X, Y, and Z separately as in your original
        float dampedX = Mathf.SmoothDamp(transform.position.x, desiredPosition.x, ref velocity.x, smoothTime);
        float dampedZ = Mathf.SmoothDamp(transform.position.z, desiredPosition.z, ref velocity.z, smoothTime);
        float dampedY = Mathf.SmoothDamp(transform.position.y, desiredPosition.y, ref currentHeightVelocity, smoothTime);

        transform.position = new Vector3(dampedX, dampedY, dampedZ);
    }

    private Bounds GetPlayerBounds(bool includeBuffer)
    {
        if (players.Count == 0) return new Bounds(Vector3.zero, Vector3.zero);

        Bounds bounds = new Bounds(players[0].position, Vector3.zero);

        foreach (Transform t in players)
        {
            if (t == null) continue;

            if (includeBuffer)
            {
                Vector3 pos = t.position;
                //expand box distance around player
                bounds.Encapsulate(new Vector3(pos.x + playerBufferRadius, pos.y, pos.z + playerBufferRadius));
                bounds.Encapsulate(new Vector3(pos.x - playerBufferRadius, pos.y, pos.z - playerBufferRadius));
                bounds.Encapsulate(new Vector3(pos.x + playerBufferRadius, pos.y, pos.z - playerBufferRadius));
                bounds.Encapsulate(new Vector3(pos.x - playerBufferRadius, pos.y, pos.z + playerBufferRadius));
            }
            else
            {
                bounds.Encapsulate(t.position);
            }
        }
        return bounds;
    }

    private float CalculateRequiredHeight(Bounds bounds)
    {
        if (cam == null) return minHeight;

        float halfFovRad = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;

        float heightByWidth = (requiredWidth / 2f) / Mathf.Tan(halfFovRad);
        float heightByWidth = ((bounds.size.x + padding) * 0.5f) / (Mathf.Tan(halfFovRad) * cam.aspect);

        //height to fit Z (vertical depth)
        float heightByDepth = ((bounds.size.z + padding) * 0.5f) / Mathf.Tan(halfFovRad);

        //use max of both to ensure none are cut off
        float requiredHeight = Mathf.Max(heightByWidth, heightByDepth);

        return Mathf.Clamp(requiredHeight, minHeight, maxHeight);
    }
    //Debug.Log("test");

    public void RemovePlayer(Transform playerTransform)
    {
        if (players.Contains(playerTransform))
        {
            players.Remove(playerTransform);
        }
    }

    private void OnDrawGizmos()
    {
        if (players.Count > 0)
        {
            Gizmos.color = Color.yellow;
            Bounds b = GetPlayerBounds(true);
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}