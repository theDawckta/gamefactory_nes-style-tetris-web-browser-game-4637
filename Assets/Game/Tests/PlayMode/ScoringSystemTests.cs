using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ScoringSystemTests
{
    private GameObject _go;
    private ScoringSystem _scoring;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject();
        _scoring = _go.AddComponent<ScoringSystem>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator Awake_InitializesLevel1()
    {
        yield return null;
        Assert.AreEqual(1, _scoring.Level);
        Assert.AreEqual(0, _scoring.Score);
        Assert.AreEqual(0, _scoring.TotalLines);
    }

    [UnityTest]
    public IEnumerator AddLines_SingleAtLevel1_Awards80Points()
    {
        yield return null;
        _scoring.AddLines(1);
        Assert.AreEqual(80, _scoring.Score);
    }

    [UnityTest]
    public IEnumerator AddLines_DoubleAtLevel1_Awards200Points()
    {
        yield return null;
        _scoring.AddLines(2);
        Assert.AreEqual(200, _scoring.Score);
    }

    [UnityTest]
    public IEnumerator AddLines_TripleAtLevel1_Awards600Points()
    {
        yield return null;
        _scoring.AddLines(3);
        Assert.AreEqual(600, _scoring.Score);
    }

    [UnityTest]
    public IEnumerator AddLines_TetrisAtLevel1_Awards2400Points()
    {
        yield return null;
        _scoring.AddLines(4);
        Assert.AreEqual(2400, _scoring.Score);
    }

    [UnityTest]
    public IEnumerator AddLines_10SingleLines_IncrementsLevelTo2()
    {
        yield return null;
        for (int i = 0; i < 10; i++)
            _scoring.AddLines(1);
        Assert.AreEqual(2, _scoring.Level);
        Assert.AreEqual(10, _scoring.TotalLines);
    }

    [UnityTest]
    public IEnumerator AddLines_20Lines_IncrementsLevelTo3()
    {
        yield return null;
        for (int i = 0; i < 20; i++)
            _scoring.AddLines(1);
        Assert.AreEqual(3, _scoring.Level);
    }

    [UnityTest]
    public IEnumerator AddLines_ScoreUsesLevelBeforeLineIncrement()
    {
        yield return null;
        // Fill 9 lines so we are at level 1 still, then add 1 more.
        // The 10th line scores at level 1 (before level becomes 2).
        for (int i = 0; i < 9; i++)
            _scoring.AddLines(1);
        Assert.AreEqual(1, _scoring.Level);
        _scoring.AddLines(1);
        // All 10 singles scored at level 1: 80 * 10 = 800
        Assert.AreEqual(800, _scoring.Score);
        Assert.AreEqual(2, _scoring.Level);
    }

    [UnityTest]
    public IEnumerator Reset_ReturnsToInitialState()
    {
        yield return null;
        _scoring.AddLines(4);
        _scoring.Reset();
        Assert.AreEqual(0, _scoring.Score);
        Assert.AreEqual(1, _scoring.Level);
        Assert.AreEqual(0, _scoring.TotalLines);
    }

    [UnityTest]
    public IEnumerator OnStatsChanged_FiresWithCorrectValuesAfterAddLines()
    {
        yield return null;
        int capturedScore = -1, capturedLevel = -1, capturedTotalLines = -1;
        _scoring.OnStatsChanged += (s, l, tl) => { capturedScore = s; capturedLevel = l; capturedTotalLines = tl; };

        _scoring.AddLines(1);

        Assert.AreEqual(80, capturedScore);
        Assert.AreEqual(1, capturedLevel);
        Assert.AreEqual(1, capturedTotalLines);
    }

    [UnityTest]
    public IEnumerator OnStatsChanged_FiresOnEveryAddLines()
    {
        yield return null;
        int fireCount = 0;
        _scoring.OnStatsChanged += (s, l, tl) => fireCount++;

        _scoring.AddLines(1);
        _scoring.AddLines(2);
        _scoring.AddLines(4);

        Assert.AreEqual(3, fireCount);
    }

    [UnityTest]
    public IEnumerator AddLines_Zero_DoesNotChangeState()
    {
        yield return null;
        _scoring.AddLines(0);
        Assert.AreEqual(0, _scoring.Score);
        Assert.AreEqual(1, _scoring.Level);
        Assert.AreEqual(0, _scoring.TotalLines);
    }
}
