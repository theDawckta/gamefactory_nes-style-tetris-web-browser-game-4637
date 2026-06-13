using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ScoreEntry
{
    public int rank;
    public string initials;
    public int score;
}

[Serializable]
public class ScoresResponse
{
    public ScoreEntry[] scores;
}

public class LeaderboardService : MonoBehaviour
{
    [SerializeField] private string _baseUrl = "http://localhost:3000";

    public static LeaderboardService Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void FetchScores(Action<ScoreEntry[]> onSuccess, Action<string> onError)
    {
        StartCoroutine(FetchScoresRoutine(onSuccess, onError));
    }

    public void PostScore(string initials, int score, Action<ScoreEntry[]> onSuccess, Action<string> onError)
    {
        StartCoroutine(PostScoreRoutine(initials, score, onSuccess, onError));
    }

    private IEnumerator FetchScoresRoutine(Action<ScoreEntry[]> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(_baseUrl + "/scores"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
                yield break;
            }

            ScoresResponse response = JsonUtility.FromJson<ScoresResponse>(request.downloadHandler.text);
            onSuccess?.Invoke(response.scores);
        }
    }

    [Serializable]
    private class PostScoreRequest
    {
        public string initials;
        public int score;
    }

    private IEnumerator PostScoreRoutine(string initials, int score, Action<ScoreEntry[]> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(new PostScoreRequest { initials = initials, score = score });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(_baseUrl + "/scores", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
                yield break;
            }

            ScoresResponse response = JsonUtility.FromJson<ScoresResponse>(request.downloadHandler.text);
            onSuccess?.Invoke(response.scores);
        }
    }
}
