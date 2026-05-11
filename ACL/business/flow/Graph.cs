namespace ACL.business.flow
{
    class Graph<T>
    {
        public List<Node<T>> Heads { get; set; } = new List<Node<T>>();
        public List<Node<T>> Nodes { get; set; } = new List<Node<T>>();
        public List<Edge<T>> Edges { get; set; } = new List<Edge<T>>();


        public List<Node<T>> FindNodes(Func<Node<T>, bool> fn)
        {
            return Nodes.Where(fn).ToList();
        }

        public Node<T> AddNode(T t)
        {
            var node = new Node<T>() { Data = t };
            Nodes.Add(node);
            return node;
        }

        public void AddEdge(Edge<T> edge)
        {
            Edges.Add(edge);
            if (edge.From != null)
            {
                edge.From.Edges.Add(edge);
            }

            if (edge.To != null)
            {
                edge.To.Edges.Add(edge);
            }
        }

        public List<Node<T>> TerminalNodes()
        {
            return Nodes.Where(x => x.IsTerminal).ToList();
        }

        public void TraverseVertices(Action<Node<T>> action)
        {
            foreach (var item in Nodes)
            {
                action(item);
            }
        }

        public void TraverseEdge(Action<Edge<T>> action)
        {
            foreach (var item in Edges)
            {
                action(item);
            }
        }
    }

    class Node<T>
    {
        public T? Data { get; set; }

        public List<Edge<T>> Edges { get; set; } = new List<Edge<T>>();

        /// <summary>
        /// 从本节点出发的边
        /// </summary>
        public List<Edge<T>> FromEdges => Edges.Where(x => x.From == this).ToList();

        /// <summary>
        /// 指向本节点的边
        /// </summary>
        public List<Edge<T>> ToEdges => Edges.Where(x => x.To == this).ToList();
        /// <summary>
        /// 指向的顶点
        /// </summary>
        public List<Node<T>> ToNodes => FromEdges.Where(x => x.To != null).Select(x => x.To).ToList();
        public bool IsTerminal
        {
            get { return Edges.Count == 0; }
        }


        public bool IsHead { get; set; }
    }

    class Edge<T>
    {
        public Node<T>? From { get; set; }

        public Node<T>? To { get; set; }

        public bool FromVisited { get; set; }
        public bool ToVisited { get; set; }
    }
}
