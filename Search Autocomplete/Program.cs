using System;
using System.Collections.Generic;
using System.Linq;

namespace Search_Autocomplete
{
    class Program
    {
        static void Main(string[] args)
        {
            AutocompleteSystem auto = new AutocompleteSystem(new string[] { }, new int[] { });
            while (true)
            {
                Console.WriteLine("Press enter to exit the search! AND esc to clear the window");
                ConsoleKeyInfo c;
                Console.WriteLine("type...");
                c = Console.ReadKey();
                Console.WriteLine("\n autocomplete results are below...");
                if (c.Key == ConsoleKey.Enter) break;
                if (c.Key == ConsoleKey.Escape) Console.Clear();
                Console.WriteLine(string.Join(",", (auto.AutoComplete(c.KeyChar))));
            }
        }

        public class Node : IComparer<Node>
        {
            public Dictionary<char, Node> children;
            public bool isEnd;
            public string data;
            public int rank;
            public List<Node> hot;

            public Node()
            {
                this.children = new Dictionary<char, Node>();
                this.isEnd = false;
                this.rank = 0;
                hot = new List<Node>();
            }

            public int Compare(Node x, Node y)
            {
                if (x.rank == y.rank)
                    return x.data.CompareTo(y.data);
                return y.rank - x.rank;
            }

            public void Update(Node n)
            {
                if (!this.hot.Contains(n))
                {
                    this.hot.Add(n);
                }
                this.hot = this.hot.OrderBy(x => x.data).ToList();
                if (this.hot.Count > 3)
                {
                    hot.Remove(this.hot[this.hot.Count - 1]);
                }
            }
        }

        class AutocompleteSystem
        {
            private Node root;
            private Node current;
            private string keyword;

            public AutocompleteSystem(string[] sentences, int[] times)
            {
                this.root = new Node();
                this.current = root;
                this.keyword = "";

                for (int i = 0; i < sentences.Length; i++)
                {
                    this.AddRecord(sentences[i], times[i]);
                }
            }

            public void AddRecord(string sentence, int t)
            {
                Node node = this.root;
                List<Node> visited = new List<Node>();
                foreach (char c in sentence)
                {
                    if (!node.children.ContainsKey(c))
                        node.children.Add(c, new Node() { data = c.ToString() });
                    node = node.children[c];
                    visited.Add(node);
                }
                node.isEnd = true;
                node.data = sentence;
                node.rank += t;

                foreach (Node i  in visited)
                {
                    i.Update(node);
                }
            }

            public List<string> AutoComplete(char c)
            {
                List<string> res = new List<string>();
                if (c == '#')
                {
                    AddRecord(keyword, 1);
                    keyword = "";
                    current = root;
                    return new List<string> { };
                }

                this.keyword += c;
                if (current != null)
                {
                    if (!current.children.ContainsKey(c))
                        return new List<string> { };
                    else
                        current = current.children[c];
                }
                else
                    return new List<string> { };

                foreach (Node node in current.hot)
                {
                    res.Add(node.data);
                }
                return res;
            }
        }
    }
}
