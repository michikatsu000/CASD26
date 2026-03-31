using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    internal class Program
    {
        static int[,] TransitiveClosureBFS(List<int>[] graph) {
            int n=graph.Length;
            int[,] rechability = new int[n, n];
            for (int source = 0; source < n; source++) {
                bool[] visited = new bool[n];
             Queue<int> queue=new Queue<int>();
              queue.Enqueue(source);
                visited[source] = true;
                while (queue.Count > 0) {
                  int current=queue.Dequeue();
                    //Помечаем достижимость из source в current
                    rechability[source, current] = 1;
                    foreach (int neighbor in graph[current])
                    {
                        if (!visited[neighbor])
                        {
                            visited[neighbor] = true;
                            queue.Enqueue(neighbor);
                        }
                    }
               }
            }
        return rechability;
        }
        class WeightEdge {
        public int Vertex { get; set; }
        public int Weight { get; set; }
            public WeightEdge(int vertex, int weight) {
                Vertex = vertex;
                Weight = weight;
            }

        }
        static int[] Dijkstra(List<WeightEdge>[] weightedGraph, int startVertex) {
        int n=weightedGraph.Length;
            int[] distances = new int[n];
            for (int i = 0; i < n; i++)
            {
                distances[i]=int.MaxValue;
            }
            distances[startVertex] = 0;
            bool[] visited=new bool [n];

            var priorityQ = new SortedSet<(int distances, int vertex)>();
            priorityQ.Add((0, startVertex));
            while (priorityQ.Count > 0) {
                var current = priorityQ.Min;
                priorityQ.Remove(current);
                int currentVerteex=current.vertex;
                if (visited[currentVerteex]) continue;
                visited[currentVerteex]=true;
                foreach (var edge in weightedGraph[currentVerteex]) {
                    int neighbor = edge.Vertex;
                    int weight = edge.Weight;
                    if (!visited[neighbor] && distances[currentVerteex] != int.MaxValue)
                    {
                        int newDistance = distances[currentVerteex] + weight;
                        if (newDistance < distances[neighbor]) {
                            priorityQ.Remove((distances[neighbor], neighbor));
                            distances[neighbor]=newDistance;
                            priorityQ.Add((distances[neighbor], neighbor));
                        }    
                    }
                }
            }
            return distances;
        }
        class Edge {
        public int To { get; set; }
            public int ReverseIndex { get; set; }
        public long Capacity { get; set; }
            public Edge(int to, int reverseIndex, long capacity) {
                To = to;
                ReverseIndex=reverseIndex;
                Capacity = capacity;
            }
        }
        class DinicalAlgorithm {
            private int verticesCount;
            private List<Edge>[] graph;
            private int[] level;
            private int[] pointer;
            public DinicalAlgorithm(int n)
            {
                verticesCount = n;
                graph = new List<Edge>[verticesCount];
                level = new int[verticesCount];
                pointer = new int[verticesCount];
                for (int i = 0; i < verticesCount; i++) {
                    graph[i] = new List<Edge>();   
                }
            }
            public void AddEdge(int from, int to, long capacity)
            {

                Edge forwardEdge = new Edge(to, graph[to].Count, capacity);
                Edge backwardEdge = new Edge(from, graph[from].Count, 0);
                graph[from].Add(forwardEdge);
                graph[to].Add(backwardEdge);
            }
            private bool BFS(int source, int sink) {
                for (int i = 0; i < verticesCount; i++) {
                    level[i] = -1;
                }
            Queue<int> queue=new Queue<int>();
            queue.Enqueue(source);
                level[source] = 0;
                while (queue.Count > 0) {
                int current=queue.Dequeue();
                    foreach (Edge edge in graph[current]) {
                        if (edge.Capacity > 0 && level[edge.To] < 0)
                        {
                            level[edge.To] = level[current] + 1;
                            queue.Enqueue(edge.To);
                        }
                    }
                }
                return level[sink] >= 0;

             }
            private long DFS(int current, int sink, long flow) {
                if (current == sink) //дошли до стока дно рекурсии
                    return flow;
                for (; pointer[current] < graph[current].Count; pointer[current]++) {
                Edge edge= graph[current][pointer[current]];
                    if (edge.Capacity > 0 && level[current] + 1 == level[edge.To]) {
                        long pushed = DFS(edge.To, sink, Math.Min(flow, edge.Capacity));
                        if (pushed > 0) {
                        edge.Capacity-= pushed;
                            graph[edge.To][edge.ReverseIndex].Capacity += pushed;
                            return pushed;
                        }
                    }
                
                }
                return 0;
            }
            public long MaxFlow(int source, int sink) {
                long maxFlow = 0;
                while (true) {

                    if (!BFS(source, sink))
                        break;
                    for (int i = 0; i < verticesCount; i++) {
                        pointer[i] = 0;
                    }
                    while (true) {
                        long pushed = DFS(source, sink, long.MaxValue);
                        if (pushed == 0) break;
                        maxFlow += pushed;
                    
                    
                    
                    
                    }
                
                
                }
            return maxFlow;
            
            }
        }


        static void Main(string[] args)
        {
            // 1. Пример транзитивного замыкания
    List<int>[] graph = new List<int>[4];
            // ... заполнение графа
            int[,] closure = TransitiveClosureBFS(graph);

            // 2. Пример алгоритма Дейкстры
            List<WeightEdge>[] weightedGraph = new List<WeightEdge>[5];
            // ... заполнение взвешенного графа
            int[] distances = Dijkstra(weightedGraph, 0);

            // 3. Пример алгоритма Диница
            DinicalAlgorithm dinic = new DinicalAlgorithm(6);
            dinic.AddEdge(0, 1, 16);
            // ... добавление ребер
            long maxFlow = dinic.MaxFlow(0, 5);

        }
    }
}
