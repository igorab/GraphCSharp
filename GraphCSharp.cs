using System;
using System.Collections;
using System.Collections.Generic;

#region sources
//https://russianblogs.com/article/85982525594/

//https://introprogramming.info/english-intro-csharp-book/read-online/chapter-17-trees-and-graphs/

// https://www.geeksforgeeks.org/detect-cycle-in-a-graph/

//https://kb.cnblogs.com/page/41707/
#endregion

namespace GraphCSharp
{
    class V
    {
        public int Num { get; set; }
    }

    abstract class Graph<T>
    {
        public const int MaxGraphSize = 25;

        public Graph()
        {
            numVertices = 0;
            listNodes = new List<T>();
        }

        protected List<T> listNodes; // список вершин
        protected int[,] edge = new int[MaxGraphSize, MaxGraphSize]; // матрица смежности
        protected int numVertices; // размер - число вершин графа
        protected int numEdges;
        protected bool[] visited = new bool[MaxGraphSize];

        protected abstract int FindVertex(ref T vertex, ref List<T> lst); // поиск вершины
        protected abstract int GetVertexPos(ref T vertex); // позиция вершины в списке
        protected abstract List<T> GraphDepthFirstSearch(ref T beginVertex);
        protected abstract List<T> GraphBreadthFirstSearch();
        protected abstract void ClearVisitFlag();

        // методы тестирование графа
        public abstract bool GraphEmpty();
        public abstract bool GraphFull();

        //методы обработки данных
        public abstract int NumberOfVertices();
        public abstract int NumberOfEdges();
        public abstract int GetWeight(ref T vertex1, ref T vertex2);
        public abstract List<T> GetNeighbors(ref T vertex);

        //методы модификиации графа
        public abstract int InsertVertex(ref T vertex);
        public abstract void InsertEdge(ref T vertex1, ref T vertex2, int weight);
        public abstract void DeleteVertex(ref T vertex);
        public abstract void DeleteEdge(ref T vertex1, ref T vertex2);

        //утилиты
        public abstract void CreateGraph(string _text);
        public abstract void DepthFirstSearch();
        public abstract void BreadthFirstSearch();
    }

    class RPGraph<T> : Graph<T>
    {
        //конструктор
        public RPGraph()
        {
            for (int i = 0; i < MaxGraphSize; i++)
                for (int j = 0; j < MaxGraphSize; j++)
                    edge[i, j] = 0;

            numVertices = 0;
            numEdges = 0;
        }

        // удалить грань
        public override void DeleteEdge(ref T vertex1, ref T vertex2)
        {
            int pos1, pos2;
            pos1 = GetVertexPos(ref vertex1);
            pos2 = GetVertexPos(ref vertex2);

            if (pos1 != -1 && pos2 != -1)
            {
                if (edge[pos1, pos2] > 0)
                {
                    edge[pos1, pos2] = 0;
                    edge[pos2, pos1] = 0;

                    numEdges--;
                }
            }
        }

        // удалить вершину
        public override void DeleteVertex(ref T vertex)
        {
            int pos = GetVertexPos(ref vertex);
            int row, col;

            if (pos == 0)
            {
                Console.WriteLine("Delete vertex: вершины нет в графе");
                return;
            }

            listNodes.Remove(vertex);
            numVertices--;

            //матрица смежности делится на три области
            for (row = 0; row < pos; row++)
                for (col = pos + 1; col < numVertices; col++)
                    edge[row, col - 1] = edge[row, col];

            for (row = pos + 1; row < numVertices; row++)
                for (col = pos + 1; col < numVertices; col++)
                    edge[row - 1, col - 1] = edge[row, col];

            for (row = pos + 1; row < numVertices; row++)
                for (col = 0; col < pos; col++)
                    edge[row - 1, col] = edge[row, col];

        }

        //позиция вершины в списке        
        protected override int GetVertexPos(ref T vertex)
        {            
            return FindVertex(ref vertex, ref listNodes);
        }

        // создает список вершин, смежных с vertex
        public override List<T> GetNeighbors(ref T vertex)
        {
            List<T> result = new List<T>();            
            int pos = GetVertexPos(ref vertex);

            if (pos == -1)
            {
                Console.WriteLine("GetNeighbors: такой вершины нет в графе");
                return result;
            }

            for (int i = 0; i < numVertices; ++i)
            {
                if (edge[pos, i] > 0)
                    result.Add(listNodes[i]);                
            }

            return result;
        }

        // вес ребра
        public override int GetWeight(ref T vertex1, ref T vertex2)
        {
            int pos1, pos2;

            pos1 = GetVertexPos(ref vertex1);
            pos2 = GetVertexPos(ref vertex2);

            int weight = edge[pos1, pos2];
            return weight;
        }

        // граф пуст?
        public override bool GraphEmpty()
        {
            return numVertices == 0;
        }

        // граф заполнен?
        public override bool GraphFull()
        {
            return numVertices == MaxGraphSize; ;
        }

        // добавить грань
        public override void InsertEdge(ref T vertex1, ref T vertex2, int weight)
        {
            int pos1, pos2;

            pos1 = GetVertexPos(ref vertex1);
            pos2 = GetVertexPos(ref vertex2);

            if (pos1 == -1 && pos2 == -1)
            {
                pos1 = InsertVertex(ref vertex1);
                pos2 = InsertVertex(ref vertex2);

            }
            else if (pos1 == -1)
            {
                pos1 = InsertVertex(ref vertex1);
            }
            else if (pos2 == -1)
            {
                pos2 = InsertVertex(ref vertex2);
            }

            edge[pos1, pos2] = weight;
            edge[pos2, pos1] = weight;
        }

        // добавить вершину
        public override int InsertVertex(ref T vertex)
        {
            int result = -1;

            if (!GraphFull())
            {
                listNodes.Add(vertex);

                result = numVertices;

                numVertices++;
            }

            return result;
        }

        // Число ребер
        public override int NumberOfEdges()
        {
            return numEdges;
        }

        // число вершин
        public override int NumberOfVertices()
        {
            return numVertices;
        }

        protected override int FindVertex(ref T vertex, ref List<T> lst)
        {
            int pos = -1;

            for (pos = lst.Count - 1; pos >= 0; --pos)
            {
                if (lst[pos].Equals(vertex))
                {
                    break;
                }
            }
            return pos;
        }

        //создать граф
        public override void CreateGraph(string _text)
        {
            Console.WriteLine("Enter the number of vertices: ");            
            numVertices = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter the number of edges: ");
            numEdges = Convert.ToInt32(Console.ReadLine());
            T item = default(T);

            Console.WriteLine("input vertex: ");

            for (int i = 0; i < numVertices; ++i)
            {
                Console.WriteLine("the first {0} vertices: ", i + 1);
                int vertex = Convert.ToInt32(Console.ReadLine());
                item = (T)(object)vertex;

                listNodes.Add(item);
            }

            T vert1 = default(T),
              vert2 = default(T);

            int pos1, pos2, weight;

            for (int i = 0; i < numEdges; ++i)
            {
                Console.WriteLine("Enter the first {0} edge(head, tail, weight):", i + 1);

                int vertex = Convert.ToInt32(Console.ReadLine());
                vert1 = (T)(object)vertex;
                vertex = Convert.ToInt32(Console.ReadLine());
                vert2 = (T)(object)vertex;
                weight = Convert.ToInt32(Console.ReadLine());
                
                pos1 = GetVertexPos(ref vert1);
                pos2 = GetVertexPos(ref vert2);

                edge[pos1, pos2] = weight;
                edge[pos2, pos1] = weight;
            }
        }

        protected override List<T> GraphDepthFirstSearch(ref T beginVertex)
        {
            List<T> result = new List<T>();
            List<T> adjLst;

            Stack<T> s = new Stack<T>();

            s.Push(beginVertex);

            int pos;
            T vertex;

            while (s.Count != 0)
            {
                vertex = s.Peek();
                s.Pop();

                if (FindVertex(ref vertex, ref result) == -1)
                {
                    pos = GetVertexPos(ref vertex);

                    visited[pos] = true;

                    result.Add(vertex);

                    adjLst = GetNeighbors(ref vertex);

                    foreach (T iter in adjLst)
                    {
                        T node = iter;

                        if (FindVertex(ref node, ref result) == -1)
                        {
                            s.Push(node);
                        }
                    }
                }
            }

            return result;
        }

        protected override List<T> GraphBreadthFirstSearch()
        {
            List<T> result = new List<T>();
            List<T> adjLst;

            Queue<T> q = new Queue<T>();
            T item;
            int pos;

            ClearVisitFlag();

            for (int i = 0; i < numVertices; ++i)
            {
                if (!visited[i])
                {
                    visited[i] = true;

                    result.Add(listNodes[i]);

                    q.Enqueue(listNodes[i]); //Push

                    while (q.Count != 0)
                    {
                        item = q.Peek(); // .front();

                        q.Dequeue();

                        adjLst = GetNeighbors(ref item);

                        foreach (T iter in adjLst)
                        {
                            T vertex = iter;

                            if (FindVertex(ref vertex, ref result) == -1)
                            {
                                result.Add(vertex);

                                pos = GetVertexPos(ref vertex);

                                visited[pos] = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public override void DepthFirstSearch()
        {
            int pos;

            List<T> vec1;

            ClearVisitFlag();

            foreach (T iter in listNodes)
            {
                T node = iter;
                pos = GetVertexPos(ref node);

                if (!visited[pos])
                {
                    vec1 = GraphDepthFirstSearch(ref node);

                    foreach (T iter2 in vec1)
                    {
                        Console.WriteLine(iter2);
                    }
                }
            }

            ClearVisitFlag();
        }

        public override void BreadthFirstSearch()
        {
            List<T> vec;

            ClearVisitFlag();

            vec = GraphBreadthFirstSearch();

            foreach (T iter in vec)
            {
                Console.WriteLine(iter);
            }

            ClearVisitFlag();
        }

        // очистить информацию о посещении
        protected override void ClearVisitFlag()
        {
            for (int i = 0; i < numEdges; ++i)
            {
                visited[i] = false;
            }
        }
    }

    class VertexIterator<T> : IEnumerable<T>
    {
        public void Reset() { }
        public bool EndOfList() { return true; }
        public bool Next() { return true; }
        public int Data() { return 1; }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    
    public class PathGraph
    {
        Graph<int> G;
        VertexIterator<int> viter; // итератор для вершин
        List<int> L;

        public static int CreateGraph()
        {
            RPGraph<int> graph = new RPGraph<int>();

            graph.CreateGraph("");
            graph.DepthFirstSearch();
            graph.BreadthFirstSearch();

            return 1;
        }


        public void Run()
        {
            G = new RPGraph<int>();
            viter = new VertexIterator<int>();

            G.CreateGraph("");

            for (viter.Reset(); !viter.EndOfList(); viter.Next())
            {
                Console.WriteLine("Вершины, смежные с вершиной " + viter.Data().ToString());

                int vdata = viter.Data();
                L = G.GetNeighbors(ref vdata);

                foreach (int V in L)
                    Console.WriteLine(V.ToString());
            }
        }
    }

    /// <summary>
    /// Циклический граф
    /// </summary>
    public class CycleGraph
    {
        private readonly int V;
        private readonly List<List<int>> adj;

        public CycleGraph(int V)
        {
            this.V = V;
            adj = new List<List<int>>(V);

            for (int i = 0; i < V; i++)
                adj.Add(new List<int>());
        }

        // This function is a variation of DFSUtil() in 
        // https://www.geeksforgeeks.org/archives/18212 
        private bool isCyclicUtil(int i, bool[] visited,
                                        bool[] recStack)
        {

            // Mark the current node as visited and 
            // part of recursion stack 
            if (recStack[i])
                return true;

            if (visited[i])
                return false;

            visited[i] = true;

            recStack[i] = true;
            List<int> children = adj[i];

            foreach (int c in children)
                if (isCyclicUtil(c, visited, recStack))
                    return true;

            recStack[i] = false;

            return false;
        }

        private void addEdge(int sou, int dest)
        {
            adj[sou].Add(dest);
        }

        // Returns true if the graph contains a 
        // cycle, else false. 
        // This function is a variation of DFS() in 
        // https://www.geeksforgeeks.org/archives/18212 
        private bool isCyclic()
        {

            // Mark all the vertices as not visited and 
            // not part of recursion stack 
            bool[] visited = new bool[V];
            bool[] recStack = new bool[V];


            // Call the recursive helper function to 
            // detect cycle in different DFS trees 
            for (int i = 0; i < V; i++)
                if (isCyclicUtil(i, visited, recStack))
                    return true;

            return false;
        }

        // Driver code 
        public static void Main_(String[] args)
        {
            CycleGraph graph = new CycleGraph(4);
            graph.addEdge(0, 1);
            graph.addEdge(0, 2);
            graph.addEdge(1, 2);
            graph.addEdge(2, 0);
            graph.addEdge(2, 3);
            graph.addEdge(3, 3);

            if (graph.isCyclic())
                Console.WriteLine("Graph contains cycle");
            else
                Console.WriteLine("Graph doesn't "
                                        + "contain cycle");
        }
    }



    class GraphCSharp
    {
        static void Main(string[] args)
        {
            PathGraph.CreateGraph();

            Console.WriteLine("Done");

            Console.ReadLine();
        }
    }
}
