﻿using System.Diagnostics.CodeAnalysis;

namespace SkipList;

public class SkipList<TKey, TValue>
    where TKey : IComparable<TKey>
{
    public SkipList(
        int maxLevel,
        double probability)
    {
        _random = new Random();
        MaxLevel = maxLevel;
        Count = 0;
        Probability = probability;
        _head = new SkipListNode(MaxLevel);
        ValidLevel = 0;
    }

    public int MaxLevel { get; init; }

    public int ValidLevel { get; set; }

    public int Count { get; internal set; }

    public double Probability { get; init; }

    private readonly Random _random;

    private readonly SkipListNode _head;

    private readonly SkipListNode? _end = null;

    private int GetRandomLevel()
    {
        var level = 0;
        while (_random.NextDouble() > Probability)
            level++;

        return level;
    }

    public bool Contains(
        TKey key)
    {
        var cur = _head;
        for (var i = ValidLevel - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) >= 0)
                cur = cur.NextNodes[i];
        }

        return key.CompareTo(cur.Key) == 0;
    }

    public TValue Select(
        TKey key)
    {
        var cur = _head;
        for (var i = ValidLevel - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) >= 0)
                cur = cur.NextNodes[i];
        }

        if (key.CompareTo(cur.Key) != 0)
        {
            ThrowHelper.ThrowKeyNotFoundException();
        }

        return cur.Value!;
    }

    public TValue? SelectOrDefault(
        TKey key)
    {
        var cur = _head;
        for (var i = ValidLevel - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) >= 0)
                cur = cur.NextNodes[i];
        }

        return key.CompareTo(cur.Key) == 0 ? cur.Value : default;
    }

    public void Insert(
        TKey key,
        TValue value)
    {
        if (Contains(key))
            return;

        var level = GetRandomLevel();
        var newNode = new SkipListNode(level, key, value);
        var cur = _head;
        for (var i = level - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) > 0)
                cur = cur.NextNodes[i];

            newNode.NextNodes[i] = cur.NextNodes[i];
            cur.NextNodes[i] = newNode;
        }

        ValidLevel = Math.Max(ValidLevel, level);
    }

    public void Update(
        TKey key,
        TValue newValue)
    {
        var cur = _head;
        for (var i = ValidLevel - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) >= 0)
                cur = cur.NextNodes[i];
        }

        if (key.CompareTo(cur.Key) != 0)
            return;

        cur.Value = newValue;
    }

    public void Delete(
        TKey key)
    {
        var cur = _head;
        var previousNodesOfDeleteNode = new SkipListNode[ValidLevel];
        for (var i = ValidLevel - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) > 0)
                cur = cur.NextNodes[i];

            previousNodesOfDeleteNode[i] = cur;
        }

        for (var i = ValidLevel - 1;
             i >= 0;
             i--)
        {
            if (previousNodesOfDeleteNode[i].NextNodes[i] != _end && key.CompareTo(previousNodesOfDeleteNode[i].NextNodes[i].Key) == 0)
                previousNodesOfDeleteNode[i].NextNodes[i] = previousNodesOfDeleteNode[i].NextNodes[i].NextNodes[i];

            if (previousNodesOfDeleteNode[i] == _head && previousNodesOfDeleteNode[i].NextNodes[i] == _end)
                ValidLevel = i;
        }
    }

    public TValue this[
        TKey key] { get => Select(key); set => Update(key, value); }

    internal class SkipListNode
    {
        public SkipListNode(
            int level,
            TKey key,
            TValue value)
        {
            Key = key;
            Value = value;
            NextNodes = new SkipListNode[level];
        }

        internal SkipListNode(
            int level) : this(level, default, default)
        {
        }

        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public SkipListNode[] NextNodes { get; set; }
    }
}
