using System;
using System.Collections.Generic;

namespace Task_25
{
    internal class Program
    {
        public class MyTreeMap<K, V>
        {
            private class Node
            {
                public K key;
                public V value;
                public Node Left;
                public Node Right;
                public Node(K key, V value)
                {
                    this.key = key;
                    this.value = value;
                }
            }
            private IComparer<K> comparator;
            private Node root;
            private int size;
            //1
            public MyTreeMap()
            {
                comparator = Comparer<K>.Default;
                root = null;
                size = 0;
            }
            //2
            public MyTreeMap(IComparer<K> comp)
            {
                comparator = comp ?? Comparer<K>.Default;
                root = null;
                size = 0;
            }

            private int Compare(K k1, K k2)
            {

                return comparator.Compare(k1, k2);
            }
            //3
            public void Clear()
            {
                root = null;
                size = 0;
            }

            private Node GetNode(Node node, K key)
            {
                if (node == null) return null;
                int cmp = Compare(key, node.key);
                if (cmp == 0) return node;
                if (cmp < 0) return GetNode(node.Left, key);
                return GetNode(node.Right, key);
            }
            //4
            public bool ContainsKey(K key)
            {
                return GetNode(root, key) != null;
            }
            //5 
            public bool ContainsValue(V value)
            {

                return ContainsValue(root, value);
            }
            private bool ContainsValue(Node node, V value)
            {
                if (node == null) return false;
                if (EqualityComparer<V>.Default.Equals(node.value, value))
                    return true;
                return ContainsValue(node.Left, value) ||
                        ContainsValue(node.Right, value);

            }



            private void InOrder(Node node, List<KeyValuePair<K, V>> list)
            {
                if (node == null) return;
                InOrder(node.Left, list);
                list.Add(new KeyValuePair<K, V>(node.key, node.value));
                InOrder(node.Right, list);


            }
            //6 
            public List<KeyValuePair<K, V>> EntrySet()
            {
                var list = new List<KeyValuePair<K, V>>();
                InOrder(root, list);
                return list;

            }
            //7 
            public V Get(K key)
            {
                var node = GetNode(root, key);
                return node == null ? default(V) : node.value;
            }
            //8

            public bool IsEmpty()
            {
                return size == 0;

            }
            //9
            public List<K> KeySet()
            {
                var list = new List<K>();
                foreach (var entry in EntrySet())
                    list.Add(entry.Key);
                return list;
            }
            private Node Put(Node node, K key, V value)
            {
                if (node == null)
                { size++; return new Node(key, value); }
                int cmp = Compare(key, node.key);
                if (cmp == 0) node.value = value;
                else if (cmp < 0) node.Left = Put(node.Left, key, value);
                else node.Right = Put(node.Right, key, value);
                return node;
            }
            //10 
            public void Put(K key, V value)
            {

                root = Put(root, key, value);
            }
            //11
            public void Remove(K key)
            {

                root = Remove(root, key);
            }
            private Node Max(Node node)
            {
                while (node.Right != null) node = node.Right;
                return node;


            }
            private Node Min(Node node)
            { while (node.Left != null) node = node.Left; return node; }
            private Node Remove(Node node, K key)
            {
                if (node == null) return null;
                int cmp = Compare(key, node.key);
                if (cmp < 0) node.Left = Remove(node.Left, key);
                else if (cmp > 0) node.Right = Remove(node.Right, key);
                else
                {
                    size--;
                    if (node.Left == null) return node.Right;
                    if (node.Right == null) return node.Left;
                    Node min = Min(node.Right);
                    node.key = min.key;
                    node.value = min.value;
                    node.Right = Remove(node.Right, min.key);
                }
                return node;
            }
            //12
            public int Size() { return size; }
            //13
            public K FirstKey()
            {
                if (IsEmpty()) throw new InvalidOperationException("Map is empty");
                return Min(root).key;
            }
            //14
            public K LastKey()
            {
                if (IsEmpty()) throw new InvalidOperationException("Map is empty");
                return Max(root).key;
            }
            //15
            public MyTreeMap<K, V> HeadMap(K end)
            {
                var result = new MyTreeMap<K, V>(comparator);
                foreach (var e in EntrySet())
                    if (Compare(e.Key, end) < 0) result.Put(e.Key, e.Value);
                return result;
            }
            //16
            public MyTreeMap<K, V> TailMap(K start)
            {
                var result = new MyTreeMap<K, V>(comparator);
                foreach (var e in EntrySet())
                    if (Compare(e.Key, start) > 0)
                        result.Put(e.Key, e.Value);
                return result;
            }
            //17
            public MyTreeMap<K, V> SubMap(K start, K end)
            {
                var result = new MyTreeMap<K, V>(comparator);
                foreach (var e in EntrySet())
                    if (Compare(e.Key, start) >= 0 &&
                        Compare(e.Key, end) < 0)
                        result.Put(e.Key, e.Value);
                return result;
            }
            //18
            public KeyValuePair<K, V>? LowerEntry(K key)
            {
                KeyValuePair<K, V>? result = null;

                foreach (var e in EntrySet())
                {
                    if (Compare(e.Key, key) < 0)
                        result = e;
                    else
                        break;
                }

                return result;
            }
            //19 
            public KeyValuePair<K, V>? FloorEntry(K key)
            {
                KeyValuePair<K, V>? result = null;

                foreach (var e in EntrySet())
                {
                    if (Compare(e.Key, key) <= 0)
                        result = e;
                    else
                        break;
                }

                return result;
            }
            //20
            public KeyValuePair<K, V>? HigherEntry(K key)
            {
                foreach (var e in EntrySet())
                {
                    if (Compare(e.Key, key) > 0)
                        return e;
                }

                return null;
            }
            //21
            public KeyValuePair<K, V>? CeilingEntry(K key)
            {
                foreach (var e in EntrySet())
                {
                    if (Compare(e.Key, key) >= 0)
                        return e;
                }

                return null;
            }
            //22
            public K LowerKey(K key)
            {
                var e = LowerEntry(key);
                return e.HasValue ? e.Value.Key : default(K);
            }
            //23
            public K FloorKey(K key)
            {
                var e = FloorEntry(key);
                return e.HasValue ? e.Value.Key : default(K);
            }
            //24
            public K HigherKey(K key)
            {
                var e = HigherEntry(key);
                return e.HasValue ? e.Value.Key : default(K);
            }
            //25
            public K CeilingKey(K key)
            {
                var e = CeilingEntry(key);
                return e.HasValue ? e.Value.Key : default(K);
            }
            //26
            public KeyValuePair<K, V>? PollFirstEntry()
            {
                if (IsEmpty()) return null;

                var minNode = Min(root);
                var result = new KeyValuePair<K, V>(minNode.key, minNode.value);

                Remove(minNode.key);
                return result;
            }
            //27
            public KeyValuePair<K, V>? PollLastEntry()
            {
                if (IsEmpty()) return null;

                var maxNode = Max(root);
                var result = new KeyValuePair<K, V>(maxNode.key, maxNode.value);

                Remove(maxNode.key);
                return result;
            }
            //28
            public KeyValuePair<K, V>? FirstEntry()
            {
                if (IsEmpty()) return null;

                var minNode = Min(root);
                return new KeyValuePair<K, V>(minNode.key, minNode.value);
            }
            //29 
            public KeyValuePair<K, V>? LastEntry()
            {
                if (IsEmpty()) return null;

                var maxNode = Max(root);
                return new KeyValuePair<K, V>(maxNode.key, maxNode.value);
            }
        }
            public class MyHashSet<T>
           {

            private MyTreeMap<T, object> map;
            private static readonly object dummy = new object();
            //1
            public MyHashSet() : this(16, 0.75f)
            {

            }
            //2
            public MyHashSet(T[] a) : this()
            {
                if (a != null) AddAll(a);
            }
            //4
            public MyHashSet(int initialCapacity) : this(initialCapacity, 0.75f)
            {
            }
            //3 
            public MyHashSet(int initialCapacity, float loadFactor)
            {
                if (initialCapacity < 0)
                    throw new Exception("Ошибка");
                if (loadFactor <= 0 || float.IsNaN(loadFactor))
                    throw new Exception("Ошибка");
                map = new MyTreeMap<T, object>();
            }
            //5
            public bool Add(T e)
            {
                if (e == null) throw new Exception("Элемент не может иметь нулевой ссылки");

                if (Contains(e)) return false;
                map.Put(e, dummy);
                return true;
            }
            //6
            public void AddAll(T[] a)
            {
                if (a == null) throw new Exception("Массив не может быть null");
                foreach (T e in a)
                {
                    Add(e);
                }
            }
            //7
            public bool Contains(object o)
            {
                if (o == null) return false;
                try
                {
                    return map.ContainsKey((T)o);


                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }
            //9
            public bool ContainsAll(T[] a)
            {
                if (a == null)
                    throw new ArgumentNullException("Массив не может быть null");

                foreach (T e in a)
                {
                    if (!Contains(e))
                        return false;
                }
                return true;
            }
            //10
            public bool IsEmpty()
            {
                return map.IsEmpty();
            }
            //11
            public bool Remove(object o)
            {
                if (o == null)
                    return false;

                try
                {
                    if (!map.ContainsKey((T)o))
                        return false;

                    map.Remove((T)o);
                    return true;
                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }
            //12
            public void RemoveAll(T[] a)
            {
                if (a == null)
                    throw new ArgumentNullException("Массив не может быть null");

                foreach (T e in a)
                {
                    Remove(e);
                }
            }
            private List<T> KeySet()
            {
                return map.KeySet();
            }
            //13
            public void RetainAll(T[] a)
            {
                if (a == null)
                    throw new ArgumentNullException("Массив не может быть null");


                var retainSet = new HashSet<T>(a);


                List<T> allElements = KeySet();


                foreach (T e in allElements)
                {
                    if (!retainSet.Contains(e))
                    {
                        Remove(e);
                    }
                }
            }
            //14
            public int Size()
            {
                return map.Size();
            }
            // 15) Преобразование в массив объектов
            public object[] ToArray()
            {
                List<T> keys = KeySet();
                object[] result = new object[keys.Count];
                for (int i = 0; i < keys.Count; i++)
                {
                    result[i] = keys[i];
                }
                return result;
            }
            // 16) Преобразование в массив указанного типа
            public T[] ToArray(T[] a)
            {
                List<T> keys = KeySet();

                if (a == null)
                {
                    a = new T[keys.Count];
                }


                if (a.Length < keys.Count)
                {
                    a = new T[keys.Count];
                }


                for (int i = 0; i < keys.Count; i++)
                {
                    a[i] = keys[i];
                }


                if (a.Length > keys.Count)
                {
                    a[keys.Count] = default(T);
                }

                return a;
            }
            public override string ToString()
            {
                List<T> keys = KeySet();
                return "[" + string.Join(", ", keys) + "]";
            }
            public void Clear() {
                map.Clear();
            }
        }


        static void Main(string[] args)
        {
            MyHashSet<int> set1 = new MyHashSet<int>();
            set1.Add(1);
            set1.Add(2);
            set1.Add(3);
            Console.WriteLine($"set1: {set1}");

            int[] arr = { 3, 4, 5, 6 };
            MyHashSet<int> set2 = new MyHashSet<int>(arr);
            Console.WriteLine($"set2: {set2}");

            int[] newElements = { 7, 8, 9 };
            set2.AddAll(newElements);
            Console.WriteLine($"set2 после AddAll: {set2}");
            int[] checkElements = { 3, 5, 7 };

            Console.WriteLine(set2.ContainsAll(checkElements));

            set2.Remove(5);
            Console.WriteLine(set2);

            int[] toRetain = { 4, 8 };
            set2.RetainAll(toRetain);
            Console.WriteLine(set2);

            MyHashSet<string> set3 = new MyHashSet<string>();
            set3.Add("apple");
            set3.Add("banana");
            set3.Add("cherry");
        }
    }
}
