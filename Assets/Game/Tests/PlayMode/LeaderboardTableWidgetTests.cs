using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Tetris.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class LeaderboardTableWidgetTests
{
    private GameObject _widgetGo;
    private LeaderboardTableWidget _widget;

    [SetUp]
    public void SetUp()
    {
        _widgetGo = new GameObject("LeaderboardTableWidget");
        _widget = _widgetGo.AddComponent<LeaderboardTableWidget>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_widgetGo != null) Object.Destroy(_widgetGo);
    }

    private void InvokeBuildTable(VisualElement container)
    {
        typeof(LeaderboardTableWidget)
            .GetMethod("BuildTable", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(_widget, new object[] { container });
    }

    private Label[] GetLabelArray(string fieldName)
    {
        return (Label[])typeof(LeaderboardTableWidget)
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(_widget);
    }

    private static ScoreEntry[] MakeFiveEntries()
    {
        return new ScoreEntry[]
        {
            new ScoreEntry { rank = 1, initials = "AAA", score = 100000 },
            new ScoreEntry { rank = 2, initials = "BBB", score = 80000 },
            new ScoreEntry { rank = 3, initials = "CCC", score = 60000 },
            new ScoreEntry { rank = 4, initials = "DDD", score = 40000 },
            new ScoreEntry { rank = 5, initials = "EEE", score = 20000 },
        };
    }

    private static ScoreEntry[] MakeFivePlaceholders()
    {
        return new ScoreEntry[]
        {
            new ScoreEntry { rank = 1, initials = "", score = 0 },
            new ScoreEntry { rank = 2, initials = "", score = 0 },
            new ScoreEntry { rank = 3, initials = "", score = 0 },
            new ScoreEntry { rank = 4, initials = "", score = 0 },
            new ScoreEntry { rank = 5, initials = "", score = 0 },
        };
    }

    [UnityTest]
    public IEnumerator Start_WithNullStartScreen_DoesNotThrow()
    {
        yield return null;
        Assert.IsNotNull(_widget);
    }

    [UnityTest]
    public IEnumerator PopulateTable_NullInput_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _widget.PopulateTable(null));
    }

    [UnityTest]
    public IEnumerator PopulateTable_BeforeBuildTable_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _widget.PopulateTable(MakeFiveEntries()));
    }

    [UnityTest]
    public IEnumerator BuildTable_CreatesFiveNonNullRankLabels()
    {
        yield return null;
        var container = new VisualElement();
        InvokeBuildTable(container);

        Label[] rankLabels = GetLabelArray("_rankLabels");
        Assert.AreEqual(5, rankLabels.Length);
        for (int i = 0; i < 5; i++)
            Assert.IsNotNull(rankLabels[i], "rankLabel[" + i + "] should not be null");
    }

    [UnityTest]
    public IEnumerator BuildTable_CreatesFiveNonNullInitialsLabels()
    {
        yield return null;
        var container = new VisualElement();
        InvokeBuildTable(container);

        Label[] initialsLabels = GetLabelArray("_initialsLabels");
        for (int i = 0; i < 5; i++)
            Assert.IsNotNull(initialsLabels[i], "initialsLabel[" + i + "] should not be null");
    }

    [UnityTest]
    public IEnumerator BuildTable_CreatesFiveNonNullScoreLabels()
    {
        yield return null;
        var container = new VisualElement();
        InvokeBuildTable(container);

        Label[] scoreLabels = GetLabelArray("_scoreLabels");
        for (int i = 0; i < 5; i++)
            Assert.IsNotNull(scoreLabels[i], "scoreLabel[" + i + "] should not be null");
    }

    [UnityTest]
    public IEnumerator PopulateTable_SetsRankFromEntry()
    {
        yield return null;
        var container = new VisualElement();
        InvokeBuildTable(container);
        _widget.PopulateTable(MakeFiveEntries());

        Label[] rankLabels = GetLabelArray("_rankLabels");
        Assert.AreEqual("1", rankLabels[0].text);
        Assert.AreEqual("3", rankLabels[2].text);
        Assert.AreEqual("5", rankLabels[4].text);
    }

    [UnityTest]
    public IEnumerator PopulateTable_SetsInitialsFromEntry()
    {
        yield return null;
        var container = new VisualElement();
        InvokeBuildTable(container);
        _widget.PopulateTable(MakeFiveEntries());

        Label[] initialsLabels = GetLabelArray("_initialsLabels");
        Assert.AreEqual("AAA", initialsLabels[0].text);
        Assert.AreEqual("EEE", initialsLabels[4].text);
    }

    [UnityTest]
    public IEnumerator PopulateTable_SetsScoreFromEntry()
    {
        yield return null;
        var container = new VisualElement();
        InvokeBuildTable(container);
        _widget.PopulateTable(MakeFiveEntries());

        Label[] scoreLabels = GetLabelArray("_scoreLabels");
        Assert.AreEqual("100000", scoreLabels[0].text);
        Assert.AreEqual("20000", scoreLabels[4].text);
    }

    [UnityTest]
    public IEnumerator PopulateTable_EmptyInitials_ShowsDashesForInitials()
    {
        yield return null;
        var container = new VisualElement();
        InvokeBuildTable(container);
        _widget.PopulateTable(MakeFivePlaceholders());

        Label[] initialsLabels = GetLabelArray("_initialsLabels");
        for (int i = 0; i < 5; i++)
            Assert.AreEqual("---", initialsLabels[i].text, "row " + i + " initials should show ---");
    }

    [UnityTest]
    public IEnumerator PopulateTable_EmptyInitials_ShowsDashesForScore()
    {
        yield return null;
        var container = new VisualElement();
        InvokeBuildTable(container);
        _widget.PopulateTable(MakeFivePlaceholders());

        Label[] scoreLabels = GetLabelArray("_scoreLabels");
        for (int i = 0; i < 5; i++)
            Assert.AreEqual("-------", scoreLabels[i].text, "row " + i + " score should show -------");
    }

    [UnityTest]
    public IEnumerator PopulateTable_MixedEntries_CorrectlyMapsByIndex()
    {
        yield return null;
        var container = new VisualElement();
        InvokeBuildTable(container);

        var scores = new ScoreEntry[]
        {
            new ScoreEntry { rank = 1, initials = "ZZZ", score = 999999 },
            new ScoreEntry { rank = 2, initials = "", score = 0 },
            new ScoreEntry { rank = 3, initials = "AAA", score = 1 },
            new ScoreEntry { rank = 4, initials = "", score = 0 },
            new ScoreEntry { rank = 5, initials = "XYZ", score = 500 },
        };
        _widget.PopulateTable(scores);

        Label[] initialsLabels = GetLabelArray("_initialsLabels");
        Label[] scoreLabels = GetLabelArray("_scoreLabels");

        Assert.AreEqual("ZZZ", initialsLabels[0].text);
        Assert.AreEqual("---", initialsLabels[1].text);
        Assert.AreEqual("AAA", initialsLabels[2].text);
        Assert.AreEqual("---", initialsLabels[3].text);
        Assert.AreEqual("XYZ", initialsLabels[4].text);

        Assert.AreEqual("999999", scoreLabels[0].text);
        Assert.AreEqual("-------", scoreLabels[1].text);
        Assert.AreEqual("1", scoreLabels[2].text);
        Assert.AreEqual("-------", scoreLabels[3].text);
        Assert.AreEqual("500", scoreLabels[4].text);
    }

    [UnityTest]
    public IEnumerator OnDestroy_WithNullStartScreen_DoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => Object.Destroy(_widgetGo));
        _widgetGo = null;
        yield return null;
    }
}
