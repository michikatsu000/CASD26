using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Threading;
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
    //Вспомогательный метод для вычисления хешк-кода по объекту ключа
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
    class Program {
        static void Main() {
            var map = new MyHashMap<string, int>();

            map.Put("one", 1);
            map.Put("two", 2);
            map.Put("three", 3);

            Console.WriteLine(map.Get("one"));
            Console.WriteLine(map.Get("two"));
            Console.WriteLine(map.Get("three"));

        Console.WriteLine(map.ContainsValue(3));
        Console.WriteLine(map.ContainsValue(4));
        Console.WriteLine(map.ContainsKey("five"));
        
        map.Remove("three");

        Console.WriteLine("Все ключи:");
        var keys=map.KeySet();
        foreach (var key in keys) {
            Console.Write(" "+ key);
        }

        Console.WriteLine("Все пары ключ-значение:");

        var entries = map.EntrySet();
        foreach (var entry in entries)
        {
            Console.WriteLine($"  {entry.Key} -> {entry.Value}");
        }
        map.Clear();
        Console.WriteLine(map.IsEmpty());
    }
    }
    



