using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SkipList.Test;

[TestFixture]
public class TestInsert
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(5)]
    public void Test_SkipList_Insert_Single(
        int val)
    {
        var mockData = new SkipList<int, string>(32, 0.5);
        mockData.Insert(val, val.ToString());
        var meta = mockData.GetType();
        var headFiled = meta.GetField("_head", BindingFlags.NonPublic | BindingFlags.Instance);
        var head = headFiled!.GetValue(mockData) as SkipList<int, string>.SkipListNode;
        Assert.That(head!.NextNodes[0].Key, Is.EqualTo(val));
    }

    [TestCase(new[] { 1, 1 })]
    public void Test_SkipList_Insert_Patch_WithRepeatedKey_ThrowException(
        int[] keys)
    {
        var mockData = new SkipList<int, string>(32, 0.5);
        try
        {
            mockData.Insert(keys[0], keys[0].ToString());
            mockData.Insert(keys[1], keys[1].ToString());
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
            return;
        }

        Assert.That(mockData, Is.False, "插入相同的Key未抛异常");
    }

    [TestCase(new[] { 1, 3, 2, 5, 4 })]
    public void Test_SkipList_Insert_Patch_WithoutRepeatedKey_Sort(
        int[] keys)
    {
        var mockData = new SkipList<int, string>(32, 0.5);
        var meta = mockData.GetType();
        var headFiled = meta.GetField("_head", BindingFlags.NonPublic | BindingFlags.Instance);
        var head = headFiled!.GetValue(mockData) as SkipList<int, string>.SkipListNode;
        foreach (var key in keys)
        {
            mockData.Insert(key, key.ToString());
        }

        Assert.Multiple(() =>
                        {
                            Assert.That(mockData, Has.Count.EqualTo(5));
                            Assert.That(head!.NextNodes[0].Key, Is.EqualTo(1));
                            Assert.That(head!.NextNodes[0].NextNodes[0].Key, Is.EqualTo(2));
                            Assert.That(head!.NextNodes[0].NextNodes[0].NextNodes[0].Key, Is.EqualTo(3));
                            Assert.That(head!.NextNodes[0].NextNodes[0].NextNodes[0].NextNodes[0].Key, Is.EqualTo(4));
                            Assert.That(head!.NextNodes[0].NextNodes[0].NextNodes[0].NextNodes[0].NextNodes[0].Key, Is.EqualTo(5));
                        });
    }

    [TestCase(new[] { 1, 3, 2, 5, 4 })]
    public void Test_SkipList_Select_CheckModel(
        int[] keys)
    {
        var mockData = new SkipList<int, string>(10, 0.5);
        var meta = mockData.GetType();
        var headFiled = meta.GetField("_head", BindingFlags.NonPublic | BindingFlags.Instance);
        var head = headFiled!.GetValue(mockData) as SkipList<int, string>.SkipListNode;
        foreach (var key in keys)
        {
            mockData.Insert(key, key.ToString());
        }

        for (var i = mockData.Level - 1;
             i >= 0;
             i--)
        {
            var list = new List<string>();
            var cur = head.NextNodes[i];
            while (cur != null)
            {
                list.Add(cur.Value);
                cur = cur.NextNodes[i];
            }

            var res = list.Aggregate(String.Empty, (seed, str) => seed + " -> " + str);
            Console.WriteLine(res);
        }
 
    }

    [TestCase(new[] { 1, 3, 2, 5, 4 })]
    public void Test_SkipList_Select(
        int[] keys)
    {
        var mockData = new SkipList<int, string>(10, 0.5);
        var ret = mockData.SelectOrDefault(0);
        Assert.That(ret, Is.Null);
        mockData.Insert(0, "0");
        ret = mockData.SelectOrDefault(0);
        Assert.That(ret, Is.EqualTo("0"));
        mockData.Insert(1, "1");
        ret = mockData.SelectOrDefault(3);
        Assert.That(ret, Is.Null);
    }
}
