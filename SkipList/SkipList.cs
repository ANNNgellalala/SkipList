using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SkipList;

public class SkipList<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : IComparable<TKey>
{
    public SkipList(
        int maxLevel,
        double probability,
        bool isReadOnly)
    {
        _random = new Random();
        MaxLevel = maxLevel;
        Count = 0;
        Probability = probability;
        IsReadOnly = isReadOnly;
        _head = new SkipListNode(MaxLevel);
        for (var i = 0;
             i < MaxLevel;
             i++)
        {
            _head.NextNodes[i] = _end!;
        }

        Level = 0;
        Keys = new List<TKey>();
        Values = new List<TValue>();
    }

    public int MaxLevel { get; }

    public int Level { get; set; }

    public void Add(
        KeyValuePair<TKey, TValue> item)
    {
        
    }

    public void Clear()
    {
        for (var i = 0;
             i < Level;
             i++)
        {
            _head.NextNodes[i] = _end!;
        }
        
        Level = 0;
    }

    public bool Contains(
        KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }

    public void CopyTo(
        KeyValuePair<TKey, TValue>[] array,
        int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array, nameof(array));

        if (arrayIndex + Count > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        var cur = _head.NextNodes[0];
        while (cur != _end)
        {
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(cur.Key, cur.Value);
            cur = cur.NextNodes[0];
        }
    }

    public bool Remove(
        KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public int Count { get; private set; }

    public bool IsReadOnly { get; }

    private double Probability { get; }

    private readonly Random _random;

    private readonly SkipListNode _head;

    private readonly SkipListNode? _end = null;

    private int GetRandomLevel()
    {
        var level = 1;
        while (_random.NextDouble() > Probability)
            level++;

        return Math.Min(level, MaxLevel);
    }

    private TValue Select(
        TKey key)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));

        var cur = _head;
        for (var i = Level - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) >= 0)
                cur = cur.NextNodes[i];
        }

        if (key.CompareTo(cur.Key) != 0)
            throw new InvalidOperationException($"{key} not exist");

        return cur.Value;
    }

    public void Add(
        TKey key,
        TValue value)
    {
        var cur = _head;
        for (var i = Level - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) >= 0)
                cur = cur.NextNodes[i];
        }

        if (key.CompareTo(cur.Key) == 0)
        {
            cur.Value = value;
            return;
        }

        var level = GetRandomLevel();
        var newNode = new SkipListNode(level, key, value);
        cur = _head;
        for (var i = level - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) > 0)
                cur = cur.NextNodes[i];

            newNode.NextNodes[i] = cur.NextNodes[i];
            cur.NextNodes[i] = newNode;
        }

        Level = Math.Max(Level, level);
        Count++;
    }

    public bool ContainsKey(
        TKey key)
    {
        var cur = _head;
        for (var i = Level - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) >= 0)
                cur = cur.NextNodes[i];
        }

        return cur != _head && key.CompareTo(cur.Key) == 0;
    }

    public bool Remove(
        TKey key)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));

        var cur = _head;
        var previousNodesOfDeletedNode = new List<SkipListNode>();
        var indexes = new List<int>();
        for (var i = Level - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) > 0)
                cur = cur.NextNodes[i];

            if (key.CompareTo(cur.NextNodes[i].Key) != 0)
                continue;

            indexes.Add(i);
            previousNodesOfDeletedNode.Add(cur);
        }

        if (previousNodesOfDeletedNode.Count == 0)
            return false;
        
        for (var i = 0;
             i < previousNodesOfDeletedNode.Count;
             i++)
        {
            var index = indexes[i];
            previousNodesOfDeletedNode[i].NextNodes[index] = previousNodesOfDeletedNode[i].NextNodes[index].NextNodes[index];
        }

        return true;
    }

    public bool TryGetValue(
        TKey key,
        out TValue value)
    {
        var cur = _head;
        for (var i = Level - 1;
             i >= 0;
             i--)
        {
            while (cur.NextNodes[i] != _end && key.CompareTo(cur.NextNodes[i].Key) >= 0)
                cur = cur.NextNodes[i];
        }

        if (cur != _head && key.CompareTo(cur.Key) == 0)
        {
            value = cur.Value;
            return true;
        }

        value = default;
        return false;
    }

    public TValue this[
        TKey key] { get => Select(key); set => Add(key, value); }

    public ICollection<TKey> Keys { get; }

    public ICollection<TValue> Values { get; }

    public class SkipListNode
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

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
