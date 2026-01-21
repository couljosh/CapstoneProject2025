using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

[Serializable]
public class TelemetryPayload
{
    public string eventName;
    public string platform;
    public string version;
    public float value;
    public string metadata;
    public string sessionID;
}

public class TelemetryManager : MonoBehaviour
{
    // YOUR SECURE SERVER URL
    private const string ServerUrl = "https://marwanmohamed.ca/api/telemetry";

    public static TelemetryManager Instance { get; private set; }

    // Store the unique session ID for this run of the game
    private string _currentSessionId;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- NEW CODE: Generate Session ID ---
        // Creates a unique string (e.g., "b8d04-...") every time the game starts
        _currentSessionId = Guid.NewGuid().ToString();
        Debug.Log($"[Telemetry] Session ID generated: {_currentSessionId}");
    }

    /// <summary>
    /// Sends a metric to your private server.
    /// </summary>
    /// <param name="eventName">What happened? (e.g. "level_complete", "death")</param>
    /// <param name="value">A number associated with it (e.g. 15.5 seconds, or 1 for a count)</param>
    /// <param name="meta">Optional extra info (e.g. "region:NA")</param>
    public void Log(string eventName, float value = 1.0f, string meta = "")
    {
        // --- NEW CODE START ---
        // If we are in the Unity Editor, STOP here. Do not send data.
        if (Application.isEditor)
        {
            Debug.Log($"[Telemetry Skipped] Editor Mode: {eventName}");
            return;
        }
        // --- NEW CODE END ---

        StartCoroutine(PostRequest(eventName, value, meta));
    }

    private IEnumerator PostRequest(string eventName, float value, string meta)
    {
        // Create the data package
        TelemetryPayload payload = new TelemetryPayload
        {
            eventName = eventName,
            platform = Application.platform.ToString(),
            version = Application.version,
            value = value,
            metadata = meta
        };

        // Convert to JSON
        string json = JsonUtility.ToJson(payload);

        // Prepare the web request
        using (UnityWebRequest request = new UnityWebRequest(ServerUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send and wait (does not freeze game)
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                // Only log errors in the Editor so players don't see them
#if UNITY_EDITOR
                Debug.LogWarning($"Telemetry Failed: {request.error}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"[Telemetry] Sent: {eventName}");
#endif
            }
        }
    }
}
