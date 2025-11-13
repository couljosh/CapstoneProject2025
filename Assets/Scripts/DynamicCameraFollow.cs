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
            else
            {
                Debug.LogWarning($"{playerName} not found in scene.");
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

        Bounds bounds = GetPlayerBounds();
        float targetHeight = CalculateRequiredHeight(bounds);

        Vector3 targetPosition = new Vector3(
            bounds.center.x + horizontalOffset,
            targetHeight + verticalOffset,
            bounds.center.z
        );

        // --- INITIAL POSITION CLAMPING ---
        if (boundaryCollider != null)
        {
            Bounds mapBounds = boundaryCollider.bounds;
            targetPosition.x = Mathf.Clamp(targetPosition.x, mapBounds.min.x, mapBounds.max.x);
            targetPosition.z = Mathf.Clamp(targetPosition.z, mapBounds.min.z, mapBounds.max.z);
        }
        // --- END CLAMPING ---

        transform.position = targetPosition;
    }

    void LateUpdate()
    {
        if (players.Count == 0 || cam == null) return;

        Bounds bounds = GetPlayerBounds();
        float targetHeight = CalculateRequiredHeight(bounds);

        Vector3 desiredPosition = new Vector3(
            bounds.center.x + horizontalOffset,
            targetHeight + verticalOffset,
            bounds.center.z + horizontalOffset
        );

        if (boundaryCollider != null)
        {
            Bounds mapBounds = boundaryCollider.bounds;

            desiredPosition.x = Mathf.Clamp(
                desiredPosition.x,
                mapBounds.min.x,
                mapBounds.max.x
            );

            desiredPosition.z = Mathf.Clamp(
                desiredPosition.z,
                mapBounds.min.z,
                mapBounds.max.z
            );
        }


        float dampedX = Mathf.SmoothDamp(
            transform.position.x,
            desiredPosition.x,
            ref velocity.x,
            smoothTime
        );

        float dampedZ = Mathf.SmoothDamp(
            transform.position.z,
            desiredPosition.z,
            ref velocity.z,
            smoothTime
        );

        float dampedY = Mathf.SmoothDamp(
            transform.position.y,
            desiredPosition.y,
            ref currentHeightVelocity,
            smoothTime
        );

        //apply clamped position
        transform.position = new Vector3(
            dampedX,
            dampedY,
            dampedZ
        );
    }

    private Bounds GetPlayerBounds()
    {
        if (players.Count == 0) return new Bounds(Vector3.zero, Vector3.zero);

        Bounds bounds = new Bounds(players[0].position, Vector3.zero);

        for (int i = 1; i < players.Count; i++)
        {
            if (players[i] != null)
            {
                bounds.Encapsulate(players[i].position);
            }
        }
        return bounds;
    }

    private float CalculateRequiredHeight(Bounds bounds)
    {
        if (cam == null) return minHeight;

        float requiredWidth = bounds.extents.x * 2f + padding;
        float requiredDepth = bounds.extents.z * 2f + padding;

        float halfFovRad = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;

        float heightByWidth = (requiredWidth / 2f) / Mathf.Tan(halfFovRad);

        float aspect = cam.aspect;
        float heightByDepth = (requiredDepth / 2f / aspect) / Mathf.Tan(halfFovRad);

        float requiredHeight = Mathf.Max(heightByWidth, heightByDepth);

        return Mathf.Clamp(requiredHeight, minHeight, maxHeight);
    }

    public void RemovePlayer(Transform playerTransform)
    {
        if (players.Contains(playerTransform))
        {

            players.Remove(playerTransform);
        }
    }
}