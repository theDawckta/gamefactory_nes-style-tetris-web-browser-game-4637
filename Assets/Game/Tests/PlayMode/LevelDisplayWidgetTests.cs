using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Tetris.UI;
using UnityEngine;
using UnityEngine.TestTools;

public class LevelDisplayWidgetTests
{
    private GameObject _widgetGo;
    private LevelDisplayWidget _widget;
    private GameObject _scoringGo;
    private ScoringSystem _scoring;

    [SetUp]
    public void SetUp()
    {
        _scoringGo = new GameObject("ScoringSystem");
        _scoring = _scoringGo.AddComponent<ScoringSystem>();

        _widgetGo = new GameObject("LevelDisplayWidget");
        _widget = _widgetGo.AddComponent<LevelDisplayWidget>();

        typeof(LevelDisplayWidget)
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
    public IEnumerator UpdateLevel_WithNullLabel_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _widget.UpdateLevel(5));
    }

    [UnityTest]
    public IEnumerator UpdateLevel_WithNullLabel_MultipleValues_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() =>
        {
            _widget.UpdateLevel(1);
            _widget.UpdateLevel(2);
            _widget.UpdateLevel(10);
        });
    }

    [UnityTest]
    public IEnumerator OnStatsChanged_SubscriptionActive_HandlesEventWithoutException()
    {
        yield return null;
        Assert.DoesNotThrow(() => _scoring.AddLines(10));
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
