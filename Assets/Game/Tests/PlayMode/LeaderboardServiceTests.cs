using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LeaderboardServiceTests
{
    private GameObject _go;
    private LeaderboardService _service;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("LeaderboardService");
        _service = _go.AddComponent<LeaderboardService>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.DestroyImmediate(_go);
    }

    private void SetBaseUrl(string url)
    {
        FieldInfo field = typeof(LeaderboardService).GetField("_baseUrl",
            BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(_service, url);
    }

    [UnityTest]
    public IEnumerator Awake_SetsInstance()
    {
        yield return null;
        Assert.IsNotNull(LeaderboardService.Instance, "Instance should be set after Awake");
        Assert.AreEqual(_service, LeaderboardService.Instance, "Instance should be the component added in SetUp");
    }

    [UnityTest]
    public IEnumerator FetchScores_CallsOnError_WhenServerUnreachable()
    {
        yield return null;
        SetBaseUrl("http://localhost:19999");

        string errorMessage = null;
        bool done = false;
        _service.FetchScores(
            _ => done = true,
            err => { errorMessage = err; done = true; }
        );

        float startTime = Time.realtimeSinceStartup;
        yield return new WaitUntil(() => done || Time.realtimeSinceStartup - startTime > 10f);

        Assert.IsTrue(done, "A callback should have been invoked within timeout");
        Assert.IsNotNull(errorMessage, "onError should be invoked, not onSuccess");
        Assert.IsNotEmpty(errorMessage, "onError message should not be empty");
    }

    [UnityTest]
    public IEnumerator PostScore_CallsOnError_WhenServerUnreachable()
    {
        yield return null;
        SetBaseUrl("http://localhost:19999");

        string errorMessage = null;
        bool done = false;
        _service.PostScore("AAA", 12345,
            _ => done = true,
            err => { errorMessage = err; done = true; }
        );

        float startTime = Time.realtimeSinceStartup;
        yield return new WaitUntil(() => done || Time.realtimeSinceStartup - startTime > 10f);

        Assert.IsTrue(done, "A callback should have been invoked within timeout");
        Assert.IsNotNull(errorMessage, "onError should be invoked, not onSuccess");
        Assert.IsNotEmpty(errorMessage, "onError message should not be empty");
    }

    [UnityTest]
    public IEnumerator SecondInstance_IsDestroyedBySingleton()
    {
        yield return null;
        Assert.IsNotNull(LeaderboardService.Instance, "First instance should exist");

        var go2 = new GameObject("LeaderboardService2");
        go2.AddComponent<LeaderboardService>();

        yield return null;

        Assert.IsTrue(go2 == null, "Second LeaderboardService GameObject should be destroyed by singleton logic");
        Assert.AreEqual(_service, LeaderboardService.Instance, "Original instance should remain");
    }
}
