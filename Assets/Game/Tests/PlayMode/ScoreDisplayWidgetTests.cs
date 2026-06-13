using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Tetris.UI;
using UnityEngine;
using UnityEngine.TestTools;

public class ScoreDisplayWidgetTests
{
    private GameObject _widgetGo;
    private ScoreDisplayWidget _widget;
    private GameObject _scoringGo;
    private ScoringSystem _scoring;

    [SetUp]
    public void SetUp()
    {
        _scoringGo = new GameObject("ScoringSystem");
        _scoring = _scoringGo.AddComponent<ScoringSystem>();

        _widgetGo = new GameObject("ScoreDisplayWidget");
        _widget = _widgetGo.AddComponent<ScoreDisplayWidget>();

        typeof(ScoreDisplayWidget)
            .GetField("_scoringSystem", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(_widget, _scoring);
    }

    [TearDown]
    public void TearDown()
    {
        if (_widgetGo != null) Object.Destroy(_widgetGo);
        if (_scoringGo != null) Object.Destroy(_scoringGo);
    }

    [UnityTest]
    public IEnumerator Start_WithNullGameScreen_DoesNotThrow()
    {
        yield return null;
        Assert.IsNotNull(_widget);
    }

    [UnityTest]
    public IEnumerator UpdateScore_WithNullLabel_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _widget.UpdateScore(0));
    }

    [UnityTest]
    public IEnumerator UpdateScore_WithNullLabel_MultipleValues_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() =>
        {
            _widget.UpdateScore(0);
            _widget.UpdateScore(100);
            _widget.UpdateScore(9999999);
        });
    }

    [UnityTest]
    public IEnumerator OnStatsChanged_SubscriptionActive_HandlesEventWithoutException()
    {
        yield return null;
        Assert.DoesNotThrow(() => _scoring.AddLines(1));
    }

    [UnityTest]
    public IEnumerator OnStatsChanged_FiresOnEveryAddLines_NoException()
    {
        yield return null;
        Assert.DoesNotThrow(() =>
        {
            _scoring.AddLines(1);
            _scoring.AddLines(2);
            _scoring.AddLines(4);
        });
    }

    [UnityTest]
    public IEnumerator OnDestroy_UnsubscribesFromScoringSystem()
    {
        yield return null;
        Object.Destroy(_widgetGo);
        _widgetGo = null;
        yield return null;
        Assert.DoesNotThrow(() => _scoring.AddLines(1));
    }
}
