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
    public void Test_SkipList_Insert_Patch_WithRepeatedKey_Sort(
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
}