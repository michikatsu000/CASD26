using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task_28
{
    internal class Program
    {
        public class MyIteratorException : Exception
        {
            public MyIteratorException() : base() { }
            public MyIteratorException(string msg) : base(msg) { }
            public MyIteratorException(string msg, Exception inner) : base(msg, inner) { }
        }
        // Исключение при попытке удалить элемент без предварительного вызова next()
        public class IllegalStateException : MyIteratorException
        {
            public IllegalStateException() : base("Недопустимое состояние итератора") { }
            public IllegalStateException(string message) : base(message) { }
        }
        // Исключение при отсутствии следующего элемента
        public class NoSuchElementException : MyIteratorException
        {
            public NoSuchElementException() : base("Нет такого элемента") { }
            public NoSuchElementException(string message) : base(message) { }
        }

        public interface MyIterator<T>
        {
            bool HasNext();
            T Next();
            void Remove();
        }

        public interface MyListIterator<T> : MyIterator<T>
        {
            bool HasPrevious();
            T Previous();
            int NextIndex();
            int PreviousIndex();
            void Set(T element);
            void Add(T element);
        }

        public delegate int PriorityQueueComparer<T>(T x, T y);
        public class MyPriorityQueue<T>
        {
            private T[] queue; // массив для хранения элементов
            private int size; // количество элементов
            private PriorityQueueComparer<T> comparator; // компаратор для сравнения

            // 1) Конструктор по умолчанию
            public MyPriorityQueue()
            {
                queue = new T[11];
                size = 0;
                comparator = DefaultComparer;
            }

            // 2) Конструктор с массивом
            public MyPriorityQueue(T[] a)
            {
                if (a == null) throw new ArgumentNullException("Массив не может быть null");

                queue = new T[a.Length];
                size = a.Length;
                comparator = DefaultComparer;

                Array.Copy(a, queue, a.Length);
                BuildHeap();
            }

            // 3) Конструктор с начальной ёмкостью
            public MyPriorityQueue(int initialCapacity)
            {
                if (initialCapacity < 1) throw new ArgumentException("Ёмкость должна быть положительной");

                queue = new T[initialCapacity];
                size = 0;
                comparator = DefaultComparer;
            }

            // 4) Конструктор с ёмкостью и компаратором
            public MyPriorityQueue(int initialCapacity, PriorityQueueComparer<T> comparer)
            {
                if (initialCapacity < 1) throw new ArgumentException("Ёмкость должна быть положительной");

                queue = new T[initialCapacity];
                size = 0;
                comparator = comparer ?? DefaultComparer;
            }

            // 5) Конструктор копирования
            public MyPriorityQueue(MyPriorityQueue<T> other)
            {
                if (other == null) throw new ArgumentNullException("Очередь не может быть null");

                queue = new T[other.queue.Length];
                size = other.size;
                comparator = other.comparator;

                Array.Copy(other.queue, queue, size);
            }

            // 6) Добавление элемента
            public void Add(T e)
            {
                if (size == queue.Length)
                {
                    ResizeArray();
                }

                queue[size] = e;
                size++;
                HeapifyUp(size - 1);
            }

            // 7) Добавление массива элементов
            public void AddAll(T[] a)
            {
                if (a == null) throw new ArgumentNullException("Массив не может быть null");

                foreach (T item in a)
                {
                    Add(item);
                }
            }

            // 8) Очистка очереди
            public void Clear()
            {
                Array.Clear(queue, 0, size);
                size = 0;
            }

            // 9) Проверка наличия элемента
            public bool Contains(object o)
            {
                if (o == null) return false;

                for (int i = 0; i < size; i++)
                {
                    if (queue[i].Equals((T)o))
                    {
                        return true;
                    }
                }
                return false;
            }

            // 10) Проверка наличия всех элементов массива
            public bool ContainsAll(T[] a)
            {
                if (a == null) throw new ArgumentNullException("Массив не может быть null");

                foreach (T item in a)
                {
                    if (!Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }

            // 11) Проверка пустоты очереди
            public bool IsEmpty()
            {
                return size == 0;
            }

            // 12) Удаление элемента
            public bool Remove(object o)
            {
                if (o == null) return false;

                for (int i = 0; i < size; i++)
                {
                    if (queue[i].Equals((T)o))
                    {
                        RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }

            // 13) Удаление всех элементов массива
            public bool RemoveAll(T[] a)
            {
                if (a == null) throw new ArgumentNullException("Массив не может быть null");

                bool modified = false;
                foreach (T item in a)
                {
                    if (Remove(item))
                    {
                        modified = true;
                    }
                }
                return modified;
            }

            // 14) Оставить только указанные элементы
            public bool RetainAll(T[] a)
            {
                if (a == null) throw new ArgumentNullException("Массив не может быть null");

                bool modified = false;
                List<T> toRemove = new List<T>();

                // Находим элементы для удаления
                for (int i = 0; i < size; i++)
                {
                    bool found = false;
                    foreach (T item in a)
                    {
                        if (queue[i].Equals(item))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        toRemove.Add(queue[i]);
                        modified = true;
                    }
                }

                // Удаляем элементы
                foreach (T item in toRemove)
                {
                    Remove(item);
                }

                return modified;
            }

            // 15) Получение размера
            public int Size()
            {
                return size;
            }

            // 16) Преобразование в массив
            public T[] ToArray()
            {
                T[] result = new T[size];
                Array.Copy(queue, result, size);
                return result;
            }

            // 17) Преобразование в массив с указанным массивом
            public T[] ToArray(T[] a)
            {
                if (a == null)
                {
                    return ToArray();
                }

                if (a.Length < size)
                {
                    return ToArray();
                }

                Array.Copy(queue, a, size);
                if (a.Length > size)
                {
                    a[size] = default(T);
                }

                return a;
            }

            // 18) Получение элемента из головы без удаления
            public T Element()
            {
                if (size == 0) throw new InvalidOperationException("Очередь пуста");
                return queue[0];
            }

            // 19) Попытка добавления элемента
            public bool Offer(T obj)
            {
                try
                {
                    Add(obj);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            // 20) Безопасное получение элемента из головы
            public T Peek()
            {
                if (size == 0) return default(T);
                return queue[0];
            }

            // 21) Безопасное удаление и получение элемента из головы
            public T Poll()
            {
                if (size == 0) return default(T);

                T result = queue[0];
                RemoveAt(0);
                return result;
            }

            // Вспомогательные методы

            // Увеличение массива при необходимости
            private void ResizeArray()
            {
                int newCapacity;
                if (queue.Length < 64)
                {
                    newCapacity = queue.Length * 2;
                }
                else
                {
                    newCapacity = queue.Length + (queue.Length / 2);
                }

                T[] newQueue = new T[newCapacity];
                Array.Copy(queue, newQueue, size);
                queue = newQueue;
            }

            // Просеивание вверх
            private void HeapifyUp(int index)
            {
                while (index > 0)
                {
                    int parentIndex = (index - 1) / 2;
                    if (comparator(queue[index], queue[parentIndex]) > 0)
                    {
                        Swap(index, parentIndex);
                        index = parentIndex;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Просеивание вниз
            private void HeapifyDown(int index)
            {
                while (true)
                {
                    int leftChild = 2 * index + 1;
                    int rightChild = 2 * index + 2;
                    int largest = index;

                    if (leftChild < size && comparator(queue[leftChild], queue[largest]) > 0)
                    {
                        largest = leftChild;
                    }

                    if (rightChild < size && comparator(queue[rightChild], queue[largest]) > 0)
                    {
                        largest = rightChild;
                    }

                    if (largest != index)
                    {
                        Swap(index, largest);
                        index = largest;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Построение кучи
            private void BuildHeap()
            {
                for (int i = size / 2 - 1; i >= 0; i--)
                {
                    HeapifyDown(i);
                }
            }

            // Удаление элемента по индексу
            private void RemoveAt(int index)
            {
                if (index < 0 || index >= size) return;

                queue[index] = queue[size - 1];
                size--;
                HeapifyDown(index);
            }

            // Обмен элементов
            private void Swap(int i, int j)
            {
                T temp = queue[i];
                queue[i] = queue[j];
                queue[j] = temp;
            }

            // Компаратор по умолчанию
            private int DefaultComparer(T x, T y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                return Comparer<T>.Default.Compare(x, y);
            }


            public void Print()
            {
                Console.Write("Очередь с приоритетами: ");
                for (int i = 0; i < size; i++)
                {
                    Console.Write(queue[i] + " ");
                }
                Console.WriteLine();
            }

            public MyIterator<T> Iterator()
            {

                return new MyItr(this);
            }
            private class MyItr : MyIterator<T>
            {
                private MyPriorityQueue<T> collection;
                private int cursor;
                private int lastReturned = -1;

                public MyItr(MyPriorityQueue<T> collection)
                {
                    this.collection = collection;
                    this.cursor = 0;
                }
                public bool HasNext()
                {
                    return cursor < collection.size;
                }
                public T Next()
                {
                    if (!HasNext()) throw new NoSuchElementException();
                    lastReturned = cursor;
                    return collection.queue[cursor++];
                }
                public void Remove()
                {
                    if (lastReturned == -1) throw new IllegalStateException("next() не был вызван до remove()");
                    collection.RemoveAt(lastReturned);
                    if (lastReturned < cursor) cursor--;
                    lastReturned = -1;
                }
            }
        }
        public class MyArrayDeque<T>
        {
            private T[] elements;
            private int head;
            private int tail;
            private int size;
            // 1) Конструктор по умолчанию
            public MyArrayDeque()
            {
                elements = new T[16];
                head = 0;
                tail = 0;
                size = 0;
            }
            // 2) Конструктор из массива
            public MyArrayDeque(T[] a)
            {
                elements = new T[a.Length + 16];
                Array.Copy(a, 0, elements, 0, a.Length);
                head = 0;
                tail = a.Length;
                size = a.Length;
            }
            // 3) Конструктор с указанием размера
            public MyArrayDeque(int numElements)
            {
                elements = new T[numElements];
                head = 0;
                tail = 0;
                size = 0;
            }
            private void EnsureCapacity()
            {
                if (size == elements.Length)
                {
                    T[] newElements = new T[elements.Length * 2];
                    for (int i = 0; i < size; i++)
                    {

                        newElements[i] = elements[head + i];
                    }
                    elements = newElements;
                    head = 0;
                    tail = size;
                }
            }
            //4
            public void Add(T e)
            {
                if (tail >= elements.Length)
                {
                    EnsureCapacity();
                }
                elements[tail] = e;
                tail++;
                size++;
            }

            //5
            public void AddAll(T[] a)
            {
                foreach (T item in a)
                {
                    Add(item);
                }
            }
            //6
            public void Clear()
            {
                elements = new T[16];
                head = 0;
                tail = 0;
                size = 0;
            }

            //7
            public bool Contains(object o)
            {
                for (int i = head; i < tail; i++)
                {
                    if (elements[i]?.Equals(o) == true)
                        return true;
                }
                return false;
            }
            //8
            public bool ContainsAll(T[] a)
            {
                foreach (T item in a)
                {
                    if (!Contains(item))
                        return false;
                }
                return true;
            }
            //9
            public bool IsEmpty()
            {
                return size == 0;
            }
            //10
            public bool Remove(object o)
            {
                for (int i = head; i < tail; i++)
                {
                    if (elements[i]?.Equals(o) == true)
                    {
                        // Сдвигаем все элементы после удаленного
                        for (int j = i; j < tail - 1; j++)
                        {
                            elements[j] = elements[j + 1];
                        }
                        tail--;
                        size--;
                        return true;
                    }
                }
                return false;
            }
            //11
            public void RemoveAll(T[] a)
            {
                foreach (T item in a)
                {
                    Remove(item);
                }
            }
            //12
            public void RetainAll(T[] a)
            {
                List<T> toKeep = new List<T>(a);
                T[] newElements = new T[elements.Length];
                int newTail = 0;

                for (int i = head; i < tail; i++)
                {
                    if (toKeep.Contains(elements[i]))
                    {
                        newElements[newTail] = elements[i];
                        newTail++;
                    }
                }

                elements = newElements;
                head = 0;
                tail = newTail;
                size = newTail;
            }
            //13
            public int Size()
            {
                return size;
            }
            //14
            public T[] ToArray()
            {
                T[] result = new T[size];
                for (int i = 0; i < size; i++)
                {
                    result[i] = elements[head + i];
                }
                return result;
            }
            //15
            public T[] ToArray(T[] a)
            {
                if (a == null || a.Length < size)
                    return ToArray();

                for (int i = 0; i < size; i++)
                {
                    a[i] = elements[head + i];
                }
                return a;
            }
            //16
            public T Element()
            {
                if (size == 0) throw new Exception("Queue is empty");
                return elements[head];
            }
            //17
            public bool Offer(T obj)
            {
                try
                {
                    Add(obj);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            //18
            public T Peek()
            {
                if (size == 0) return default(T);
                return elements[head];
            }
            //19
            public T Poll()
            {
                if (size == 0) return default(T);
                T result = elements[head];
                head++;
                size--;
                return result;
            }
            //20
            public void AddFirst(T obj)
            {
                EnsureCapacity();
                // Сдвигаем все элементы вправо
                if (head == 0)
                {

                    for (int i = tail; i > head; i--)
                    {
                        elements[i] = elements[i - 1];

                    }

                    tail++;

                }
                else { head--; }
                elements[head] = obj; size++;
            }
            // 21) Добавить в конец
            public void AddLast(T obj)
            {
                Add(obj);
            }
            //22
            public T GetFirst()
            {
                return Element();
            }
            //23
            public T GetLast()
            {
                if (size == 0) throw new Exception("Queue is empty");
                return elements[tail - 1];
            }
            //24
            public bool OfferFirst(T obj)
            {
                try
                {
                    AddFirst(obj);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            // 25) Попытаться добавить в конец
            public bool OfferLast(T obj)
            {
                return Offer(obj);
            }
            //26
            public T Pop()
            {
                return Poll();
            }
            // 27) Добавить в начало (как в стеке)
            public void Push(T obj)
            {
                AddFirst(obj);
            }
            //28
            public T PeeeekFirst()
            {
                return Peek();
            }
            //29
            public T PeekLast()
            {
                if (size == 0) return default(T);
                return elements[tail - 1];
            }
            //30
            public T PollFirst()
            {
                return Poll();
            }
            //31
            public T PollLast()
            {
                if (size == 0) return default(T);
                tail--;
                T result = elements[tail];
                size--;
                return result;
            }
            //32
            public T RemoveLast()
            {
                if (size == 0) throw new Exception("Queue is empty");
                return PollLast();
            }
            //33
            public T RemoveFirst()
            {
                if (size == 0) throw new Exception("Queue is empty");
                return Poll();
            }
            //34
            public T PeekFirst()
            {
                if (size == 0) throw new Exception("Deque is empty");
                return elements[head];
            }
            //35
            public bool RemoveLastOccurrence(object obj)
            {
                for (int i = tail - 1; i >= head; i--)
                {
                    if (elements[i]?.Equals(obj) == true)
                    {

                        for (int j = i; j < tail - 1; j++)
                        {
                            elements[j] = elements[j + 1];
                        }
                        tail--;
                        size--;
                        return true;
                    }
                }
                return false;
            }
            //36
            public bool RemoveFirstOccurrence(object obj)
            {
                return Remove(obj);
            }
            public MyIterator<T> Iterator()
            {
                return new MyItr(this);
            }
            private class MyItr : MyIterator<T>
            {
                private MyArrayDeque<T> collection;
                private int cursor;
                private int lastReturned = -1;

                public MyItr(MyArrayDeque<T> collection)
                {
                    this.collection = collection;
                    this.cursor = collection.head;
                }

                public bool HasNext()
                {
                    return cursor < collection.tail;
                }

                public T Next()
                {
                    if (!HasNext()) throw new NoSuchElementException();
                    lastReturned = cursor;
                    return collection.elements[cursor++];
                }

                public void Remove()
                {
                    if (lastReturned == -1) throw new IllegalStateException("next() не был вызван до remove()");
                    for (int j = lastReturned; j < collection.tail - 1; j++)
                        collection.elements[j] = collection.elements[j + 1];
                    collection.tail--;
                    collection.size--;
                    if (lastReturned < cursor) cursor--;
                    lastReturned = -1;
                }
            }
        }

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
            public void Clear()
            {
                map.Clear();
            }

            public MyIterator<T> Iterator()
            {
                return new MyItr(this);
            }

            private class MyItr : MyIterator<T>
            {
                private MyHashSet<T> collection;
                private List<T> elements;
                private int cursor;
                private int lastReturned = -1;

                public MyItr(MyHashSet<T> collection)
                {
                    this.collection = collection;
                    this.elements = collection.KeySet();
                    this.cursor = 0;
                }

                public bool HasNext() => cursor < elements.Count;

                public T Next()
                {
                    if (!HasNext()) throw new NoSuchElementException();
                    lastReturned = cursor;
                    return elements[cursor++];
                }

                public void Remove()
                {
                    if (lastReturned == -1) throw new IllegalStateException("next() не был вызван до remove()");
                    collection.Remove(elements[lastReturned]);
                    elements.RemoveAt(lastReturned);
                    if (lastReturned < cursor) cursor--;
                    lastReturned = -1;
                }
            }
        }


            public class MyTreeSet<T> : IEnumerable<T>
            {
                private enum NodeColor { Red, Black }

                private class Node
                {
                    public T Key;
                    public NodeColor Color;
                    public Node Left, Right, Parent;

                    public Node(T key)
                    {
                        Key = key;
                        Color = NodeColor.Red;
                    }
                }

                private readonly Node _nil;
                private Node _root;
                private IComparer<T> _comparer;
                private int _count;

                public int Count => _count;

                public MyTreeSet()
                {
                    _comparer = Comparer<T>.Default;
                    _nil = new Node(default);
                    _nil.Color = NodeColor.Black;
                    _nil.Left = _nil;
                    _nil.Right = _nil;
                    _nil.Parent = _nil;
                    _root = _nil;
                }

                public MyTreeSet(IComparer<T> comparer) : this()
                {
                    _comparer = comparer;
                }

                public MyTreeSet(IEnumerable<T> collection) : this()
                {
                    foreach (var item in collection)
                        Add(item);
                }

                public MyTreeSet(IEnumerable<T> collection, IComparer<T> comparer) : this(comparer)
                {
                    foreach (var item in collection)
                        Add(item);
                }

                public MyTreeSet(T[] array) : this()
                {
                    foreach (var item in array)
                        Add(item);
                }

                private void RotateLeft(Node x)
                {
                    Node y = x.Right;
                    x.Right = y.Left;
                    if (y.Left != _nil)
                        y.Left.Parent = x;
                    y.Parent = x.Parent;
                    if (x.Parent == _nil)
                        _root = y;
                    else if (x == x.Parent.Left)
                        x.Parent.Left = y;
                    else
                        x.Parent.Right = y;
                    y.Left = x;
                    x.Parent = y;
                }

                private void RotateRight(Node x)
                {
                    Node y = x.Left;
                    x.Left = y.Right;
                    if (y.Right != _nil)
                        y.Right.Parent = x;
                    y.Parent = x.Parent;
                    if (x.Parent == _nil)
                        _root = y;
                    else if (x == x.Parent.Right)
                        x.Parent.Right = y;
                    else
                        x.Parent.Left = y;
                    y.Right = x;
                    x.Parent = y;
                }
                public bool Add(T item)
                {
                    if (item == null)
                        throw new ArgumentNullException(nameof(item));
                    Node z = new Node(item);
                    z.Left = _nil;
                    z.Right = _nil;
                    Node y = _nil;
                    Node x = _root;
                    while (x != _nil)
                    {
                        y = x;
                        int cmp = _comparer.Compare(item, x.Key);
                        if (cmp == 0)
                            return false;
                        else if (cmp < 0)
                            x = x.Left;
                        else
                            x = x.Right;
                    }
                    z.Parent = y;
                    if (y == _nil)
                        _root = z;
                    else if (_comparer.Compare(item, y.Key) < 0)
                        y.Left = z;
                    else
                        y.Right = z;
                    _count++;
                    InsertFixup(z);
                    return true;
                }
                private void InsertFixup(Node z)
                {
                    while (z.Parent.Color == NodeColor.Red)
                    {
                        if (z.Parent == z.Parent.Parent.Left) //если родитель левый ребенок деда 
                        {
                            Node y = z.Parent.Parent.Right;
                            if (y.Color == NodeColor.Red)
                            {
                                z.Parent.Color = NodeColor.Black; //Перекрашиваем родителя 
                                y.Color = NodeColor.Black; // его брата 
                                z.Parent.Parent.Color = NodeColor.Red; // дед теперь красный 
                                z = z.Parent.Parent; // поднимаемся к деду 
                            }
                            else // дядя чёрни => повороты из за черной высоты
                            {
                                if (z == z.Parent.Right)
                                {
                                    z = z.Parent;
                                    RotateLeft(z);
                                } // после ротации z=Parent z.parent=z
                                z.Parent.Color = NodeColor.Black; //z
                                z.Parent.Parent.Color = NodeColor.Red; //дед красный
                                RotateRight(z.Parent.Parent); //правый поворот вокруг деда 
                            }
                        }
                        else
                        {
                            Node y = z.Parent.Parent.Left;
                            if (y.Color == NodeColor.Red)
                            {
                                z.Parent.Color = NodeColor.Black;
                                y.Color = NodeColor.Black;
                                z.Parent.Parent.Color = NodeColor.Red;
                                z = z.Parent.Parent;
                            }
                            else
                            {
                                if (z == z.Parent.Left)
                                {
                                    z = z.Parent;
                                    RotateRight(z);
                                }
                                z.Parent.Color = NodeColor.Black;
                                z.Parent.Parent.Color = NodeColor.Red;
                                RotateLeft(z.Parent.Parent);
                            }
                        }
                    }
                    _root.Color = NodeColor.Black;
                }
                private Node FindNode(T item)
                {
                    Node x = _root;
                    while (x != _nil)
                    {
                        int cmp = _comparer.Compare(item, x.Key);
                        if (cmp == 0)
                            return x;
                        else if (cmp < 0)
                            x = x.Left;
                        else
                            x = x.Right;
                    }
                    return _nil;
                }

                public bool Contains(T item)
                {
                    if (item == null)
                        throw new ArgumentNullException(nameof(item));
                    return FindNode(item) != _nil;
                }

                public bool Remove(T item)
                {
                    if (item == null)
                        throw new ArgumentNullException(nameof(item));
                    Node z = FindNode(item);
                    if (z == _nil)
                        return false;
                    Delete(z);
                    _count--;
                    return true;
                }
                private void Delete(Node z)
                {
                    Node y = z;
                    Node x;
                    NodeColor originalColor = y.Color; //цвет удаляемого узла
                    if (z.Left == _nil)
                    {
                        x = z.Right;
                        Transplant(z, z.Right);
                    }
                    else if (z.Right == _nil)
                    {
                        x = z.Left;
                        Transplant(z, z.Left);
                    }
                    else
                    {
                        y = TreeMinimum(z.Right);
                        originalColor = y.Color;
                        x = y.Right;
                        if (y.Parent == z)
                            x.Parent = y;
                        else
                        {
                            Transplant(y, y.Right);
                            y.Right = z.Right;
                            y.Right.Parent = y;
                        }
                        Transplant(z, y);
                        y.Left = z.Left;
                        y.Left.Parent = y;
                        y.Color = z.Color;
                    }
                    if (originalColor == NodeColor.Black)
                        DeleteFixup(x);
                }

                private void Transplant(Node u, Node v) //замена поддеревьев
                {
                    if (u.Parent == _nil)
                        _root = v;
                    else if (u == u.Parent.Left)
                        u.Parent.Left = v;
                    else
                        u.Parent.Right = v;
                    v.Parent = u.Parent;
                }
                private void DeleteFixup(Node x)
                {
                    while (x != _root && x.Color == NodeColor.Black)
                    {
                        if (x == x.Parent.Left) //x левый ребенок 
                        {
                            Node w = x.Parent.Right; //брат икса 
                            if (w.Color == NodeColor.Red) //брат 
                            {
                                w.Color = NodeColor.Black; //перекрашиваем 
                                x.Parent.Color = NodeColor.Red;//перекрашиваем родителя 
                                RotateLeft(x.Parent);
                                w = x.Parent.Right;
                            }
                            if (w.Left.Color == NodeColor.Black && w.Right.Color == NodeColor.Black)
                            {
                                w.Color = NodeColor.Red;
                                x = x.Parent;
                            }
                            else
                            {
                                if (w.Right.Color == NodeColor.Black)
                                {
                                    w.Left.Color = NodeColor.Black;
                                    w.Color = NodeColor.Red;
                                    RotateRight(w);
                                    w = x.Parent.Right;
                                }
                                w.Color = x.Parent.Color;
                                x.Parent.Color = NodeColor.Black;
                                w.Right.Color = NodeColor.Black;
                                RotateLeft(x.Parent);
                                x = _root;
                            }
                        }
                        else
                        {
                            Node w = x.Parent.Left;
                            if (w.Color == NodeColor.Red)
                            {
                                w.Color = NodeColor.Black;
                                x.Parent.Color = NodeColor.Red;
                                RotateRight(x.Parent);
                                w = x.Parent.Left;
                            }
                            if (w.Right.Color == NodeColor.Black && w.Left.Color == NodeColor.Black)
                            {
                                w.Color = NodeColor.Red;
                                x = x.Parent;
                            }
                            else
                            {
                                if (w.Left.Color == NodeColor.Black)
                                {
                                    w.Right.Color = NodeColor.Black;
                                    w.Color = NodeColor.Red;
                                    RotateLeft(w);
                                    w = x.Parent.Left;
                                }
                                w.Color = x.Parent.Color;
                                x.Parent.Color = NodeColor.Black;
                                w.Left.Color = NodeColor.Black;
                                RotateRight(x.Parent);
                                x = _root;
                            }
                        }
                    }
                    x.Color = NodeColor.Black;
                }
                private Node TreeMinimum(Node node)
                {
                    while (node.Left != _nil)
                        node = node.Left;
                    return node;
                }

                private Node TreeMaximum(Node node)
                {
                    while (node.Right != _nil)
                        node = node.Right;
                    return node;
                }
                public T First()
                {
                    if (_root == _nil)
                        throw new InvalidOperationException("Set is empty");
                    return TreeMinimum(_root).Key;
                }

                public T Last()
                {
                    if (_root == _nil)
                        throw new InvalidOperationException("Set is empty");
                    return TreeMaximum(_root).Key;
                }

                public T PollFirst()
                {
                    if (_root == _nil)
                        return default;
                    Node min = TreeMinimum(_root);
                    T key = min.Key;
                    Delete(min);
                    _count--;
                    return key;
                }

                public T PollLast()
                {
                    if (_root == _nil)
                        return default;
                    Node max = TreeMaximum(_root);
                    T key = max.Key;
                    Delete(max);
                    _count--;
                    return key;
                }

                public T Floor(T item)
                {
                    if (item == null)
                        throw new ArgumentNullException(nameof(item));
                    Node result = _nil;
                    Node x = _root;
                    while (x != _nil)
                    {
                        int cmp = _comparer.Compare(item, x.Key);
                        if (cmp == 0)
                            return x.Key;
                        else if (cmp > 0)
                        {
                            result = x;
                            x = x.Right;
                        }
                        else
                            x = x.Left;
                    }
                    if (result == _nil)
                        throw new InvalidOperationException("No floor element exists");
                    return result.Key;
                }

                public T Ceiling(T item)
                {
                    if (item == null)
                        throw new ArgumentNullException(nameof(item));
                    Node result = _nil;
                    Node x = _root;
                    while (x != _nil)
                    {
                        int cmp = _comparer.Compare(item, x.Key);
                        if (cmp == 0)
                            return x.Key;
                        else if (cmp < 0)
                        {
                            result = x;
                            x = x.Left;
                        }
                        else
                            x = x.Right;
                    }
                    if (result == _nil)
                        throw new InvalidOperationException("No ceiling element exists");
                    return result.Key;
                }

                public T Lower(T item)
                {
                    if (item == null)
                        throw new ArgumentNullException(nameof(item));
                    Node result = _nil;
                    Node x = _root;
                    while (x != _nil)
                    {
                        int cmp = _comparer.Compare(item, x.Key);
                        if (cmp > 0)
                        {
                            result = x;
                            x = x.Right;
                        }
                        else
                            x = x.Left;
                    }
                    if (result == _nil)
                        throw new InvalidOperationException("No lower element exists");
                    return result.Key;
                }

                public T Higher(T item)
                {
                    if (item == null)
                        throw new ArgumentNullException(nameof(item));
                    Node result = _nil;
                    Node x = _root;
                    while (x != _nil)
                    {
                        int cmp = _comparer.Compare(item, x.Key);
                        if (cmp < 0)
                        {
                            result = x;
                            x = x.Left;
                        }
                        else
                            x = x.Right;
                    }
                    if (result == _nil)
                        throw new InvalidOperationException("No higher element exists");
                    return result.Key;
                }

                public MyTreeSet<T> SubSet(T fromElement, bool fromInclusive, T toElement, bool toInclusive)
                {
                    if (fromElement == null || toElement == null)
                        throw new ArgumentNullException();
                    var result = new MyTreeSet<T>(_comparer);
                    foreach (var item in this)
                    {
                        int cmpFrom = _comparer.Compare(item, fromElement);
                        int cmpTo = _comparer.Compare(item, toElement);
                        bool ok = (fromInclusive ? cmpFrom >= 0 : cmpFrom > 0)
                               && (toInclusive ? cmpTo <= 0 : cmpTo < 0);
                        if (ok)
                            result.Add(item);
                    }
                    return result;
                }

                public MyTreeSet<T> SubSet(T fromElement, T toElement)
                {
                    return SubSet(fromElement, true, toElement, false);
                }

                public MyTreeSet<T> HeadSet(T toElement, bool inclusive)
                {
                    if (toElement == null)
                        throw new ArgumentNullException(nameof(toElement));
                    var result = new MyTreeSet<T>(_comparer);
                    foreach (var item in this)
                    {
                        int cmp = _comparer.Compare(item, toElement);
                        if (inclusive ? cmp <= 0 : cmp < 0)
                            result.Add(item);
                    }
                    return result;
                }

                public MyTreeSet<T> HeadSet(T toElement) => HeadSet(toElement, false);

                public MyTreeSet<T> TailSet(T fromElement, bool inclusive)
                {
                    if (fromElement == null)
                        throw new ArgumentNullException(nameof(fromElement));
                    var result = new MyTreeSet<T>(_comparer);
                    foreach (var item in this)
                    {
                        int cmp = _comparer.Compare(item, fromElement);
                        if (inclusive ? cmp >= 0 : cmp > 0)
                            result.Add(item);
                    }
                    return result;
                }

                public MyTreeSet<T> TailSet(T fromElement) => TailSet(fromElement, true);

                public MyTreeSet<T> DescendingSet()
                {
                    var reverseComparer = Comparer<T>.Create((a, b) => _comparer.Compare(b, a));
                    var result = new MyTreeSet<T>(reverseComparer);
                    foreach (var item in this)
                        result.Add(item);
                    return result;
                }

                public IEnumerator<T> DescendingIterator()
                {
                    return InOrderDescending(_root).GetEnumerator();
                }

                private IEnumerable<T> InOrderDescending(Node node)
                {
                    if (node == _nil)
                        yield break;
                    foreach (var item in InOrderDescending(node.Right))
                        yield return item;
                    yield return node.Key;
                    foreach (var item in InOrderDescending(node.Left))
                        yield return item;
                }

                public IEnumerator<T> GetEnumerator() => InOrder(_root).GetEnumerator();
                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                private IEnumerable<T> InOrder(Node node)
                {
                    if (node == _nil)
                        yield break;
                    foreach (var item in InOrder(node.Left))
                        yield return item;
                    yield return node.Key;
                    foreach (var item in InOrder(node.Right))
                        yield return item;
                }

                public void Clear()
                {
                    _root = _nil;
                    _count = 0;
                }

                public override string ToString()
                {
                    var sb = new StringBuilder("[");
                    bool first = true;
                    foreach (var item in this)
                    {
                        if (!first)
                            sb.Append(", ");
                        sb.Append(item);
                        first = false;
                    }
                    sb.Append("]");
                    return sb.ToString();
                }
                public MyIterator<T> Iterator()
                {
                    return new MyItr(this);
                }

                private class MyItr : MyIterator<T>
                {
                    private MyTreeSet<T> collection;
                    private List<T> elements;
                    private int cursor;
                    private int lastReturned = -1;

                    public MyItr(MyTreeSet<T> collection)
                    {
                        this.collection = collection;
                        this.elements = new List<T>();
                        foreach (var item in collection)
                            this.elements.Add(item);
                        this.cursor = 0;
                    }

                    public bool HasNext()
                    {
                        return cursor < elements.Count;
                    }

                    public T Next()
                    {
                        if (!HasNext()) throw new NoSuchElementException();
                        lastReturned = cursor;
                        return elements[cursor++];
                    }

                    public void Remove()
                    {
                        if (lastReturned == -1) throw new IllegalStateException("next() не был вызван до remove()");
                        collection.Remove(elements[lastReturned]);
                        elements.RemoveAt(lastReturned);
                        if (lastReturned < cursor) cursor--;
                        lastReturned = -1;
                    }
                }
            }



            public class MyArrayList<T>
            {
                private T[] elementsData;
                private int size;
                //1
                public MyArrayList()
                {
                    elementsData = new T[10];
                    size = 0;
                }
                //2
                public MyArrayList(T[] a)
                {
                    if (a == null)
                        throw new ArgumentNullException(nameof(a));
                    elementsData = new T[a.Length];
                    for (int i = 0; i < a.Length; i++)
                    {

                        elementsData[i] = a[i];
                    }
                    size = a.Length;
                }
                //3
                public MyArrayList(int capacity)
                {

                    if (capacity < 0) throw
                     new ArgumentOutOfRangeException(nameof(capacity));
                    elementsData = new T[capacity];
                    size = 0;
                }
                //Вспомогательный метод для увелечения ёмкости массива
                private void EnsureCapacity(int minCapacity)
                {
                    if (minCapacity > elementsData.Length)
                    {
                        int newCapacity = (int)(elementsData.Length * 1.5) + 1;
                        if (newCapacity < minCapacity)
                            newCapacity = minCapacity;
                        T[] newArray = new T[newCapacity];
                        for (int i = 0; i < size; i++)
                            newArray[i] = elementsData[i]; //Array.Copy(elementsData, newArray, size);
                        elementsData = newArray;
                    }
                }
                //4
                public void Add(T e)
                {

                    EnsureCapacity(size + 1);
                    elementsData[size++] = e;
                }
                //5
                public void AddAll(T[] a)
                {
                    if (a == null) return;
                    EnsureCapacity(size + a.Length);
                    for (int i = 0; i < a.Length; i++)
                    {
                        elementsData[size] = a[i];
                        size++;
                    }
                }
                //6
                public void Clear()
                {
                    elementsData = new T[10];
                    size = 0;
                }
                //7
                public bool Contains(object o)
                {
                    int ind = -1;
                    for (int i = 0; i < size; i++)
                    {
                        if (o == null && elementsData[i] == null)
                        {
                            ind = i;
                        }
                        if (o != null && o.Equals(elementsData[i]))
                        {

                            ind = i;
                        }
                    }
                    if (ind != -1)
                    {
                        return true;
                    }
                    else { return false; }
                }
                //8
                public bool ContainsAll(T[] a)
                {
                    if (a == null) return false;
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (!Contains(a[i]))
                            return false;

                    }
                    return true;
                }
                //9
                public bool IsEmpty()
                {
                    return size == 0;
                }
                //10
                public bool Remove(object o)
                {

                    for (int i = 0; i < size; i++)
                    {
                        if ((o == null && elementsData[i] == null) ||
                       (o != null && o.Equals(elementsData[i])))
                        {
                            for (int j = i; j < size - 1; j++)
                            {
                                elementsData[j] = elementsData[j + 1];
                            }
                            elementsData[size - 1] = default(T);
                            size--;
                            return true;
                        }
                    }
                    return false;
                }
                //11
                public void RemoveAll(T[] a)
                {
                    if (a == null) return;
                    for (int i = 0; i < a.Length; i++)
                        Remove(a[i]);
                }
                //12
                public void RetainAll(T[] a)
                {
                    if (a == null) return;
                    T[] keep = new T[a.Length];
                    int i = 0;
                    while (i < size)
                    {
                        if (!keep.Contains(elementsData[i]))
                            Remove(a[i]);
                        else i++;
                    }
                }
                //13
                public int Size()
                { return size; }
                //14
                public object[] ToArray()
                {
                    object[] result = new object[size];
                    for (int i = 0; i < size; i++)
                        result[i] = elementsData[i];
                    return result;
                }
                //15
                public T[] ToArray(T[] a)
                {
                    if (a == null || a.Length < size)
                        a = new T[size];

                    for (int i = 0; i < size; i++)
                        a[i] = elementsData[i];

                    return a;
                }
                //16
                public void Add(int index, T e)
                {
                    if (index < 0 || index > size)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    EnsureCapacity(size + 1);
                    for (int i = size - 1; i >= index; i--)
                        elementsData[i + 1] = elementsData[i];

                    elementsData[index] = e;
                    size++;
                }
                //17

                public void AddAll(int index, T[] a)
                {
                    if (index < 0 || index > size)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    if (a == null) return;

                    EnsureCapacity(size + a.Length);

                    // сдвигаем элементы вправо
                    for (int i = size - 1; i >= index; i--)
                        elementsData[i + a.Length] = elementsData[i];

                    // вставляем новые элементы
                    for (int i = 0; i < a.Length; i++)
                        elementsData[index + i] = a[i];

                    size += a.Length;
                }
                //18
                public T Get(int index)
                {
                    if (index < 0 || index >= size)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    return elementsData[index];
                }
                //19
                public int IndexOf(object o)
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (o == null && elementsData[i] == null)
                            return i;
                        if (o != null && o.Equals(elementsData[i]))
                            return i;
                    }
                    return -1;
                }
                //20
                public int LastIndexOf(object o)
                {
                    for (int i = size - 1; i >= 0; i--)
                    {
                        if (o == null && elementsData[i] == null)
                            return i;
                        if (o != null && o.Equals(elementsData[i]))
                            return i;
                    }
                    return -1;
                }
                //21
                public T Remove(int index)
                {
                    if (index < 0 || index >= size)
                        throw new ArgumentOutOfRangeException("index");

                    T removedElement = elementsData[index];


                    for (int i = index; i < size - 1; i++)
                    {
                        elementsData[i] = elementsData[i + 1];
                    }


                    elementsData[size - 1] = default(T);
                    size--;

                    return removedElement;
                }
                //22
                public void Set(int index, T e)
                {
                    if (index < 0 || index >= size)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    elementsData[index] = e;
                }
                //23
                public T[] SubList(int fromIndex, int toIndex)
                {
                    if (fromIndex < 0 || toIndex > size || fromIndex > toIndex)
                        throw new ArgumentOutOfRangeException("Некорректные границы диапазона");

                    int newLength = toIndex - fromIndex;
                    T[] newArray = new T[newLength];

                    for (int i = 0; i < newLength; i++)
                    {
                        newArray[i] = elementsData[fromIndex + i];
                    }

                    return newArray;
                }
                public MyListIterator<T> ListIterator()
                {
                    return new MyItr(this, 0);
                }

                public MyListIterator<T> ListIterator(int index)
                {
                    if (index < 0 || index > size)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    return new MyItr(this, index);
                }

                private class MyItr : MyListIterator<T>
                {
                    private MyArrayList<T> collection;
                    private int cursor;      // указатель на текущий элемент
                    private int lastReturned = -1;

                    public MyItr(MyArrayList<T> collection, int startIndex)
                    {
                        this.collection = collection;
                        this.cursor = startIndex;
                    }

                    public bool HasNext()
                    {
                        return cursor < collection.size;
                    }

                    public T Next()
                    {
                        if (!HasNext()) throw new NoSuchElementException();
                        lastReturned = cursor;
                        return collection.elementsData[cursor++];
                    }

                    public int NextIndex()
                    {
                        return cursor;
                    }

                    public bool HasPrevious()
                    {
                        return cursor > 0;
                    }

                    public T Previous()
                    {
                        if (!HasPrevious()) throw new NoSuchElementException();
                        lastReturned = --cursor;
                        return collection.elementsData[cursor];
                    }

                    public int PreviousIndex()
                    {
                        return cursor - 1;
                    }

                    public void Remove()
                    {
                        if (lastReturned == -1) throw new IllegalStateException("next() или previous() не был вызван до remove()");
                        collection.Remove(lastReturned);
                        if (lastReturned < cursor) cursor--;
                        lastReturned = -1;
                    }

                    public void Set(T element)
                    {
                        if (lastReturned == -1) throw new IllegalStateException("next() или previous() не был вызван до set()");
                        collection.Set(lastReturned, element);
                    }

                    public void Add(T element)
                    {
                        collection.Add(cursor, element);
                        cursor++;
                        lastReturned = -1;
                    }
                }

            }

            public class MyVector<T>
            {
                private T[] elementsData;
                private int count;
                private int CapacityIncrement;

                public MyVector(int capac, int capacityIncrement)
                {
                    if (capac < 0) throw new ArgumentException("Capacity cannot be negative");
                    this.elementsData = new T[capac];
                    this.count = 0;
                    this.CapacityIncrement = capacityIncrement;
                }

                public MyVector(int capac) : this(capac, 0) { }
                public MyVector() : this(10, 0) { }

                public MyVector(T[] a)
                {
                    if (a == null) throw new ArgumentNullException(nameof(a));
                    elementsData = new T[a.Length];
                    Array.Copy(a, elementsData, a.Length);
                    count = a.Length;
                    CapacityIncrement = 0;
                }

                private void EnsureCapacity(int minCapacity)
                {
                    if (minCapacity <= elementsData.Length) return;
                    int newCapacity = elementsData.Length;
                    if (CapacityIncrement > 0)
                        newCapacity += CapacityIncrement;
                    else
                        newCapacity *= 2;
                    if (newCapacity < minCapacity) newCapacity = minCapacity;
                    Array.Resize(ref elementsData, newCapacity);
                }

                private void CheckIndex(int index)
                {
                    if (index < 0 || index >= count)
                        throw new ArgumentOutOfRangeException(nameof(index));
                }

                public void Add(T e)
                {
                    EnsureCapacity(count + 1);
                    elementsData[count++] = e;
                }

                public void AddAll(T[] a)
                {
                    if (a == null) throw new ArgumentNullException(nameof(a));
                    EnsureCapacity(count + a.Length);
                    Array.Copy(a, 0, elementsData, count, a.Length);
                    count += a.Length;
                }

                public void Clear()
                {
                    Array.Clear(elementsData, 0, count);
                    count = 0;
                }

                public int IndexOf(object o)
                {
                    if (o == null)
                    {
                        for (int i = 0; i < count; i++)
                            if (elementsData[i] == null)
                                return i;
                    }
                    else
                    {
                        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                        for (int i = 0; i < count; i++)
                            if (comparer.Equals(elementsData[i], (T)o))
                                return i;
                    }
                    return -1;
                }

                public bool Contains(object o)
                {
                    return IndexOf(o) >= 0;
                }

                public bool ContainsAll(T[] a)
                {
                    if (a == null) throw new ArgumentNullException(nameof(a));
                    foreach (T item in a)
                        if (!Contains(item)) return false;
                    return true;
                }

                public bool IsEmpty() => count == 0;

                public T Remove(int index)
                {
                    CheckIndex(index);
                    T oldValue = elementsData[index];
                    int numMoved = count - index - 1;
                    if (numMoved > 0)
                        Array.Copy(elementsData, index + 1, elementsData, index, numMoved);
                    elementsData[--count] = default;
                    return oldValue;
                }

                public void RemoveAt(int index) => Remove(index);

                public bool Remove(object o)
                {
                    int index = IndexOf(o);
                    if (index < 0) return false;
                    RemoveAt(index);
                    return true;
                }

                public void RemoveAll(T[] a)
                {
                    if (a == null) throw new ArgumentNullException(nameof(a));
                    foreach (T item in a)
                        Remove(item);
                }

                public void RetainAll(T[] a)
                {
                    if (a == null) throw new ArgumentNullException(nameof(a));
                    HashSet<T> retainset = new HashSet<T>(a);
                    int newSize = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (retainset.Contains(elementsData[i]))
                            elementsData[newSize++] = elementsData[i];
                    }
                    for (int i = newSize; i < count; i++)
                        elementsData[i] = default;
                    count = newSize;
                }

                public int Size() => count;

                public T[] ToArray()
                {
                    T[] result = new T[count];
                    Array.Copy(elementsData, result, count);
                    return result;
                }

                public T[] ToArray(T[] a)
                {
                    if (a == null || a.Length < count)
                        return ToArray();
                    Array.Copy(elementsData, a, count);
                    if (a.Length > count)
                        a[count] = default;
                    return a;
                }

                public void Add(int index, T e)
                {
                    if (index < 0 || index > count)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    EnsureCapacity(count + 1);
                    Array.Copy(elementsData, index, elementsData, index + 1, count - index);
                    elementsData[index] = e;
                    count++;
                }

                public void AddAll(int index, T[] a)
                {
                    if (a == null) throw new ArgumentNullException(nameof(a));
                    if (index < 0 || index > count)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    EnsureCapacity(count + a.Length);
                    Array.Copy(elementsData, index, elementsData, index + a.Length, count - index);
                    Array.Copy(a, 0, elementsData, index, a.Length);
                    count += a.Length;
                }

                public T Get(int index)
                {
                    CheckIndex(index);
                    return elementsData[index];
                }

                public void Set(int index, T e)
                {
                    CheckIndex(index);
                    elementsData[index] = e;
                }

                public T FirstElement()
                {
                    if (IsEmpty()) throw new InvalidOperationException("Vector is empty");
                    return elementsData[0];
                }

                public T LastElement()
                {
                    if (IsEmpty()) throw new InvalidOperationException("Vector is empty");
                    return elementsData[count - 1];
                }

                public override string ToString()
                {
                    return "[" + string.Join(", ", ToArray()) + "]";
                }
                public MyListIterator<T> ListIterator()
                {
                    return new MyItr(this, 0);
                }

                public MyListIterator<T> ListIterator(int index)
                {
                    if (index < 0 || index > count)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    return new MyItr(this, index);
                }

                private class MyItr : MyListIterator<T>
                {
                    private MyVector<T> collection;
                    private int cursor;
                    private int lastReturned = -1;

                    public MyItr(MyVector<T> collection, int startIndex)
                    {
                        this.collection = collection;
                        this.cursor = startIndex;
                    }

                    public bool HasNext()
                    {
                        return cursor < collection.count;
                    }

                    public T Next()
                    {
                        if (!HasNext()) throw new NoSuchElementException();
                        lastReturned = cursor;
                        return collection.elementsData[cursor++];
                    }

                    public int NextIndex()
                    {
                        return cursor;
                    }

                    public bool HasPrevious()
                    {
                        return cursor > 0;
                    }

                    public T Previous()
                    {
                        if (!HasPrevious()) throw new NoSuchElementException();
                        lastReturned = --cursor;
                        return collection.elementsData[cursor];
                    }

                    public int PreviousIndex()
                    {
                        return cursor - 1;
                    }

                    public void Remove()
                    {
                        if (lastReturned == -1) throw new IllegalStateException("next() или previous() не был вызван до remove()");
                        collection.Remove(lastReturned);
                        if (lastReturned < cursor) cursor--;
                        lastReturned = -1;
                    }

                    public void Set(T element)
                    {
                        if (lastReturned == -1) throw new IllegalStateException("next() или previous() не был вызван до set()");
                        collection.Set(lastReturned, element);
                    }

                    public void Add(T element)
                    {
                        collection.Add(cursor, element);
                        cursor++;
                        lastReturned = -1;
                    }
                }

            }
            public class Node<T>
            {
                public T data { get; set; }
                public Node<T> Prev { get; set; }
                public Node<T> Next { get; set; }
                public Node(T data)
                {
                    this.data = data;
                    Prev = null;
                    Next = null;

                }
            }
            public class MyLinkedList<T>
            {
                private Node<T> first;
                private Node<T> last;
                private int size;
                // 1
                public MyLinkedList()
                {
                    first = null;
                    last = null;
                    size = 0;
                }
                //8
                public bool IsEmpty()
                {
                    return size == 0;
                }
                //28
                public void AddLast(T obj)
                {
                    Node<T> newNode = new Node<T>(obj);
                    if (IsEmpty())
                    {
                        first = newNode;
                        last = newNode;
                    }
                    else
                    {
                        last.Next = newNode;
                        newNode.Prev = last;
                        last = newNode;
                    }
                    size++;
                }
                //2
                public MyLinkedList(T[] a)
                {
                    first = null;
                    last = null;
                    size = 0;
                    if (a != null)
                    {
                        foreach (T item in a)
                        {
                            AddLast(item);

                        }

                    }
                }
                //3
                public void Add(T e)
                {
                    AddLast(e);
                }
                //4
                public void AddAll(T[] a)
                {
                    if (a == null) return;
                    foreach (T item in a)
                    {
                        AddLast(item);
                    }
                }
                //5
                public void Clear()
                {
                    first = null;
                    last = null;
                    size = 0;
                }
                //6
                public bool Contains(object o)
                {
                    if (o == null) return false;
                    Node<T> current = first;
                    while (current != null)
                    {
                        if (current.data != null && current.data.Equals(0)) return true;
                        current = current.Next;
                    }
                    return false;
                }
                //7 
                public bool ContainsAll(T[] a)
                {
                    if (a == null) return true;
                    foreach (T item in a)
                    {
                        if (!Contains(item))
                            return false;
                    }
                    return true;
                }
                //9
                public bool Remove(object o)
                {
                    if (o == null || IsEmpty()) return false;
                    Node<T> current = first;
                    while (current != null)
                    {

                        if (current.data != null && current.data.Equals(o))
                        {
                            RemoveNode(current);
                            return true;
                        }
                        current = current.Next;
                    }
                    return false;
                }
                private void RemoveNode(Node<T> node)
                {
                    if (node.Prev != null)
                        node.Prev.Next = node.Next;
                    else first = node.Next;
                    if (node.Next != null) node.Next.Prev = node.Prev;
                    else last = node.Prev;
                    size--;
                }
                //10
                public void RemoveAll(T[] a)
                {
                    if (a == null || IsEmpty()) return;
                    foreach (T item in a)
                    {
                        Remove(item);
                    }
                }
                //11 
                public void RetainAll(T[] a)
                {
                    if (a == null) { Clear(); return; }
                    List<T> toKeep = new List<T>(a);
                    Node<T> current = first;
                    while (current != null)
                    {
                        Node<T> next = current.Next;
                        if (!toKeep.Contains(current.data))
                        {
                            RemoveNode(current);
                        }
                        current = next;
                    }
                }
                //12 
                public int Size() { return size; }
                //13
                public T[] ToArray()
                {
                    T[] result = new T[size];
                    Node<T> current = first;
                    int i = 0;
                    while (current != null)
                    {
                        result[i++] = current.data;
                        current = current.Next;
                    }
                    return result;
                }
                // 14) Преобразование в массив с указанием массива
                public T[] ToArray(T[] a)
                {
                    if (a == null) return ToArray();
                    if (a.Length < size) return ToArray();

                    Node<T> current = first;
                    int index = 0;

                    while (current != null)
                    {
                        a[index++] = current.data;
                        current = current.Next;
                    }

                    // Заполняем оставшиеся элементы значением по умолчанию
                    for (int i = size; i < a.Length; i++)
                    {
                        a[i] = default(T);
                    }

                    return a;
                }
                private Node<T> GetNodeAt(int index)
                {
                    if (index < 0 || index >= size)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    Node<T> current = first;
                    for (int i = 0; i < index; i++)
                    {
                        current = current.Next;
                    }
                    return current;
                }



                //27
                public void AddFirst(T obj)
                {
                    Node<T> newnode = new Node<T>(obj);
                    if (IsEmpty())
                    {
                        first = newnode;
                        last = newnode;
                    }
                    else
                    {
                        newnode.Next = first;
                        first.Prev = newnode;
                        first = newnode;
                    }
                    size++;
                }


                //15
                public void Add(int index, T e)
                {
                    if (index < 0 || index > size)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    if (index == 0)
                    {
                        AddFirst(e);
                    }
                    else if (index == size)
                    {
                        AddLast(e);
                    }
                    else
                    {
                        Node<T> current = GetNodeAt(index);
                        Node<T> newNode = new Node<T>(e);

                        newNode.Prev = current.Prev;
                        newNode.Next = current;
                        current.Prev.Next = newNode;
                        current.Prev = newNode;

                        size++;
                    }
                }
                //16
                public void AddAll(int index, T[] a)
                {
                    if (index < 0 || index > size)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    if (a == null) return;

                    for (int i = 0; i < a.Length; i++)
                    {
                        Add(index + i, a[i]);
                    }
                }
                //17
                public T Get(int index)
                {
                    if (index < 0 || index >= size)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    return GetNodeAt(index).data;
                }
                //18
                public int IndexOf(object o)
                {
                    if (o == null) return -1;

                    Node<T> current = first;
                    int index = 0;

                    while (current != null)
                    {
                        if (current.data != null && current.data.Equals(o))
                            return index;
                        current = current.Next;
                        index++;
                    }

                    return -1;
                }
                //19
                public int LastIndexOf(object o)
                {
                    if (o == null) return -1;

                    Node<T> current = last;
                    int index = size - 1;

                    while (current != null)
                    {
                        if (current.data != null && current.data.Equals(o))
                            return index;
                        current = current.Prev;
                        index--;
                    }

                    return -1;
                }
                //20
                public T Remove(int index)
                {
                    if (index < 0 || index >= size)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    Node<T> nodeToRemove = GetNodeAt(index);
                    T data = nodeToRemove.data;
                    RemoveNode(nodeToRemove);
                    return data;
                }
                //21
                public T Set(int index, T e)
                {
                    if (index < 0 || index >= size)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    Node<T> node = GetNodeAt(index);
                    T oldData = node.data;
                    node.data = e;
                    return oldData;
                }
                //22
                public MyLinkedList<T> SubList(int fromIndex, int toIndex)
                {
                    if (fromIndex < 0 || toIndex > size || fromIndex > toIndex)
                        throw new ArgumentOutOfRangeException();

                    MyLinkedList<T> subList = new MyLinkedList<T>();
                    Node<T> current = GetNodeAt(fromIndex);

                    for (int i = fromIndex; i < toIndex; i++)
                    {
                        subList.AddLast(current.data);
                        current = current.Next;
                    }

                    return subList;
                }
                //23
                public T Element()
                {
                    if (IsEmpty())
                        throw new InvalidOperationException("List is empty");

                    return first.data;
                }
                //24
                public bool Offer(T obj)
                {
                    try
                    {
                        AddLast(obj);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                //25
                public T Peek()
                {
                    if (IsEmpty()) return default(T);
                    return first.data;
                }
                //40
                public T RemoveFirst()
                {
                    if (IsEmpty())
                        throw new InvalidOperationException("List is empty");
                    T data = first.data;
                    RemoveNode(first);
                    return data;
                }
                //26
                public T Poll()
                {
                    if (IsEmpty()) return default(T);
                    T data = first.data;
                    RemoveFirst();
                    return data;
                }
                //29
                public T GetFirst()
                {
                    if (IsEmpty())
                        throw new InvalidOperationException("List is empty");

                    return first.data;
                }
                //30
                public T GetLast()
                {
                    if (IsEmpty())
                        throw new InvalidOperationException("List is empty");

                    return last.data;
                }
                //31
                public bool OfferFirst(T obj)
                {
                    try
                    {
                        AddFirst(obj);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                //32
                public bool OfferLast(T obj)
                {
                    return Offer(obj);
                }
                //33
                public T Pop()
                {
                    return RemoveFirst();
                }
                //34
                public void Push(T obj)
                {
                    AddFirst(obj);
                }
                //35
                public T PeekFirst()
                {
                    return Peek();
                }
                //36
                public T PeekLast()
                {
                    if (IsEmpty()) return default(T);
                    return last.data;
                }
                //37
                public T PollFirst()
                {
                    return Poll();
                }
                //39
                public T RemoveLast()
                {
                    if (IsEmpty())
                        throw new InvalidOperationException("List is empty");

                    T data = last.data;
                    RemoveNode(last);
                    return data;
                }

                //38
                public T PollLast()
                {
                    if (IsEmpty()) return default(T);

                    T data = last.data;
                    RemoveLast();
                    return data;
                }
                //41
                public bool RemoveLastOccurrence(object obj)
                {
                    if (obj == null || IsEmpty()) return false;

                    Node<T> current = last;
                    while (current != null)
                    {
                        if (current.data != null && current.data.Equals(obj))
                        {
                            RemoveNode(current);
                            return true;
                        }
                        current = current.Prev;
                    }
                    return false;
                }
                //42
                public bool RemoveFirstOccurrence(object obj)
                {
                    return Remove(obj);
                }
                public void Print()
                {
                    Node<T> current = first;
                    while (current != null)
                    {
                        Console.WriteLine(current.data);
                        current = current.Next;
                    }
                }
            private class MyItr : MyListIterator<T>
            {
                private MyLinkedList<T> collection;
                private int cursor;
                private int lastReturned = -1;
                private Node<T> lastReturnedNode = null;

                public MyItr(MyLinkedList<T> collection, int startIndex)
                {
                    this.collection = collection;
                    this.cursor = startIndex;
                }

                public bool HasNext()
                {
                    return cursor < collection.size;
                }

                public T Next()
                {
                    if (!HasNext()) throw new NoSuchElementException();
                    Node<T> node = collection.GetNodeAt(cursor);
                    lastReturned = cursor;
                    lastReturnedNode = node;
                    cursor++;
                    return node.data;
                }

                public int NextIndex()
                {
                    return cursor;
                }

                public bool HasPrevious()
                {
                    return cursor > 0;
                }

                public T Previous()
                {
                    if (!HasPrevious()) throw new NoSuchElementException();
                    cursor--;
                    Node<T> node = collection.GetNodeAt(cursor);
                    lastReturned = cursor;
                    lastReturnedNode = node;
                    return node.data;
                }

                public int PreviousIndex()
                {
                    return cursor - 1;
                }

                public void Remove()
                {
                    if (lastReturned == -1) throw new IllegalStateException("next() или previous() не был вызван до remove()");
                    collection.Remove(lastReturned);
                    if (lastReturned < cursor) cursor--;
                    lastReturned = -1;
                    lastReturnedNode = null;
                }

                public void Set(T element)
                {
                    if (lastReturned == -1) throw new IllegalStateException("next() или previous() не был вызван до set()");
                    collection.Set(lastReturned, element);
                }

                public void Add(T element)
                {
                    collection.Add(cursor, element);
                    cursor++;
                    lastReturned = -1;
                    lastReturnedNode = null;
                }
            }
            }
            
        }
    }





      


