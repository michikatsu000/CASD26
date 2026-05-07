using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_24_
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
  

        //Про loadFactor
        //1 Если он маленький то много resize => много выделенной памяти но быстрый поиск мало коллизий
        //2 loadFactor =0.75 стандартный (золотая середина) приемлемое использование памяти и коливо колизий
        //3 loadFactor=0.95 экономия памяти но много коллизий
        public class MyHashMap<K, V>
        {
            //Внутренний класс для хранения ключ-значения
            private class Entry
            {
                public K Key { get; set; }
                public V Value { get; set; }
                public Entry Next { get; set; }
                public Entry(K key, V value)
                {
                    Key = key;
                    Value = value;
                    Next = null;
                }
            }
            private Entry[] table;
            private int size;
            private float loadFactor;
            private const int Default_Capacity = 16;
            private const float Default_Load_Factor = 0.75f;

            //1 Конструктор для создания пустого отображения 
            // с начальной ёмкостью 16 элементов и коэффициентом загрузки 0.75
            public MyHashMap() : this(Default_Capacity, Default_Load_Factor)
            {

            }
            //2
            public MyHashMap(int initialCapacity) : this(initialCapacity, Default_Load_Factor)
            {


            }
            //3
            public MyHashMap(int initialCapacity, float loadFactor)
            {
                if (initialCapacity <= 0) throw new ArgumentException("Ёмкость массива не может быть нулевой или отрицательной");
                if (loadFactor <= 0 || float.IsNaN(loadFactor))
                    throw new ArgumentException("Коэффициент загрузки должен быть положительным");

                this.table = new Entry[initialCapacity];
                this.size = 0;
                this.loadFactor = loadFactor;
            }
            //Вспомогательный метод для вычисления хеш-кода по объекту ключа
            private int GetBucketIndex(K key)
            {
                if (key == null) return 0;
                int hashCode = key.GetHashCode();
                return Math.Abs(hashCode % table.Length);
            }
            //Метод для увелечения размера таблицы при превышении процента загржуенности (loadFactor)
            private void ResizeIfNeeded()
            {
                if ((float)size / table.Length <= loadFactor)
                    return; //Если кол во реальных элементов к выделенной  памяти массива не превышает процент загрузки ничего не делаем
                int newCapacity = table.Length * 2;
                Entry[] newTable = new Entry[newCapacity];

                for (int i = 0; i < table.Length; i++)
                {
                    Entry current = table[i];
                    while (current != null)
                    {
                        Entry next = current.Next;
                        //Вычисляем новый индекс
                        int newIndex = current.Key == null ? 0 : Math.Abs(current.Key.GetHashCode() % newCapacity);
                        current.Next = newTable[newIndex]; //при коллизии указывает на то что уже записано next или на null если ничего не записано
                        newTable[newIndex] = current;  //становмтся новой головой списка в бакете
                        current = next;
                    }
                }
                table = newTable;
            }
            //resize может разрешать коллизии перераспределением элементов из старых бакетов в одиночные
            //математически следует из формулы хеш кода
            //где вариантов хеша становится больше относительно нового класса вычета
            //4
            public void Clear()
            {
                for (int i = 0; i < table.Length; i++)
                {
                    table[i] = null;
                }
                size = 0;
            }
            //5
            public bool ContainsKey(object key)
            {
                if (key == null)
                {
                    int index = 0;
                    Entry current1 = table[index];
                    while (current1 != null)
                    {
                        if (current1.Key == null)
                            return true;
                        current1 = current1.Next;
                    }
                    return false;
                }
                int bucketIndex = GetBucketIndex((K)key);
                Entry current2 = table[bucketIndex];
                while (current2 != null)
                {
                    if (current2.Key != null && current2.Key.Equals(key))
                        return true;
                    current2 = current2.Next;
                }
                return false;
            }
            //6
            public bool ContainsValue(object value)
            {
                for (int i = 0; i < table.Length; i++)
                {
                    Entry current = table[i];
                    while (current != null)
                    {
                        if (value == null)
                        {
                            if (current.Value == null) return true;
                        }
                        else if (current.Value != null && current.Value.Equals(value))
                        { return true; }
                        current = current.Next;
                    }
                }
                return false;
            }
            //7 возврат множества который хранит элементы пар ключ значение 
            public HashSet<KeyValuePair<K, V>> EntrySet()
            {
                var set = new HashSet<KeyValuePair<K, V>>();
                for (int i = 0; i < table.Length; i++)
                {
                    Entry current = table[i];
                    while (current != null)
                    {
                        set.Add(new KeyValuePair<K, V>(current.Key, current.Value));

                        current = current.Next;
                    }
                }
                return set;
            }
            //8 возврат значения по ключу
            public V Get(object key)
            {
                if (key == null)
                {
                    int index = 0;
                    Entry current1 = table[index];
                    while (current1 != null)
                    {
                        if (current1.Key == null)
                            return current1.Value;
                        current1 = current1.Next;
                    }
                    return default(V);
                }
                int bucketIndex = GetBucketIndex((K)key);
                Entry current = table[bucketIndex];
                while (current != null)
                {
                    if (current != null && current.Key.Equals(key))
                        return current.Value;
                    current = current.Next;

                }
                return default(V);
            }
            //9
            public bool IsEmpty()
            {

                return size == 0;

            }
            public HashSet<K> KeySet()
            {
                var keySet = new HashSet<K>();
                for (int i = 0; i < table.Length; i++)
                {
                    Entry current = table[i];
                    while (current != null)
                    {
                        keySet.Add(current.Key);
                        current = current.Next;

                    }
                }
                return keySet;
            }
            //11
            public void Put(K key, V value)
            {
                ResizeIfNeeded();
                //Если этот ключ уже существует меняем значение по ключу
                int bukcetIndex = GetBucketIndex(key);
                Entry current = table[bukcetIndex];
                while (current != null)
                {
                    if (key == null)
                    {
                        if (current.Key == null)
                        {
                            current.Value = value;
                            return;
                        }
                    }
                    else if (current.Key != null && current.Key.Equals(key))
                    {
                        current.Value = value;
                        return;
                    }
                    current = current.Next;
                }

                //Иначе выполняем вставку в бакет как голова нового списка
                Entry newEntry = new Entry(key, value);
                newEntry.Next = table[bukcetIndex];
                table[bukcetIndex] = newEntry;
                size++;
            }
            //12
            public bool Remove(object key)
            {
                int bucketIndex;
                if (key == null)
                {
                    bucketIndex = 0;
                }
                else
                {

                    bucketIndex = GetBucketIndex((K)key);
                }
                Entry current = table[bucketIndex];
                Entry prev = null;
                while (current != null)
                {
                    if (key == null)
                    {
                        if (current.Key == null)
                        {
                            if (prev == null)
                            {
                                table[bucketIndex] = current.Next;
                            }
                            else
                            {
                                prev.Next = current.Next;
                            }
                            size--;
                            return true;
                        }
                    }
                    else if (current.Key != null && current.Key.Equals(key))
                    {

                        if (prev == null)
                        {
                            table[bucketIndex] = current.Next;
                        }
                        else
                        {
                            prev.Next = current.Next;
                        }
                        size--;
                        return true;

                    }
                    prev = current;
                    current = current.Next;
                }
                return false;
            }
            //13
            public int Size()
            {
                return size;
            }
        }
    
        internal class Program
       {
        const int RUNS = 20;

        static int[] sizes = { 100_000, 1_000_000 }; // можно расширить

        static void Main(string[] args)
        {
            foreach (int n in sizes)
            {
               

                BenchmarkPut(n);
                BenchmarkGet(n);
                BenchmarkRemove(n);
            }
        }
        static void BenchmarkPut(int n)
        {
            double hashTime = 0;
            double treeTime = 0;

            for (int r = 0; r < RUNS; r++)
            {
                var rand = new Random(r);

                var hash = new MyHashMap<int, int>();
                var tree = new MyTreeMap<int, int>();

                var sw = Stopwatch.StartNew();
                for (int i = 0; i < n; i++)
                    hash.Put(i, i);
                sw.Stop();
                hashTime += sw.Elapsed.TotalMilliseconds;

                sw.Restart();
                for (int i = 0; i < n; i++)
                    tree.Put(i, i);
                sw.Stop();
                treeTime += sw.Elapsed.TotalMilliseconds;
            }

            Console.WriteLine($"PUT:");
            Console.WriteLine($"  HashMap avg: {hashTime / RUNS} ms");
            Console.WriteLine($"  TreeMap avg: {treeTime / RUNS} ms");
        }

        // ================= GET =================
        static void BenchmarkGet(int n)
        {
            double hashTime = 0;
            double treeTime = 0;

            var hash = new MyHashMap<int, int>();
            var tree = new MyTreeMap<int, int>();

            for (int i = 0; i < n; i++)
            {
                hash.Put(i, i);
                tree.Put(i, i);
            }

            for (int r = 0; r < RUNS; r++)
            {
                var rand = new Random(r);

                var sw = Stopwatch.StartNew();
                for (int i = 0; i < n; i++)
                {
                    int key = rand.Next(n);
                    hash.Get(key);
                }
                sw.Stop();
                hashTime += sw.Elapsed.TotalMilliseconds;

                sw.Restart();
                for (int i = 0; i < n; i++)
                {
                    int key = rand.Next(n);
                    tree.Get(key);
                }
                sw.Stop();
                treeTime += sw.Elapsed.TotalMilliseconds;
            }

            Console.WriteLine($"GET:");
            Console.WriteLine($"  HashMap avg: {hashTime / RUNS} ms");
            Console.WriteLine($"  TreeMap avg: {treeTime / RUNS} ms");
        }

        // ================= REMOVE =================
        static void BenchmarkRemove(int n)
        {
            double hashTime = 0;
            double treeTime = 0;

            for (int r = 0; r < RUNS; r++)
            {
                var hash = new MyHashMap<int, int>();
                var tree = new MyTreeMap<int, int>();

                for (int i = 0; i < n; i++)
                {
                    hash.Put(i, i);
                    tree.Put(i, i);
                }

                var sw = Stopwatch.StartNew();
                for (int i = 0; i < n; i++)
                    hash.Remove(i);
                sw.Stop();
                hashTime += sw.Elapsed.TotalMilliseconds;

                sw.Restart();
                for (int i = 0; i < n; i++)
                    tree.Remove(i);
                sw.Stop();
                treeTime += sw.Elapsed.TotalMilliseconds;
            }

            Console.WriteLine($"REMOVE:");
            Console.WriteLine($"  HashMap avg: {hashTime / RUNS} ms");
            Console.WriteLine($"  TreeMap avg: {treeTime / RUNS} ms");
        }
    }
}

