using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxyPlotTest
{
    public class Node
    {
        public int Depth;            //深度
        public double[] Data = new double[2]; //儲存資料
        public Node Left;      //左節點
        public Node Right;    //右節點
        public Node(double[] x, int sp)
        {
            x.CopyTo(Data, 0);
            Depth = sp;
            Left = null;
            Right = null;
        }
        public void PrintInorder(Node root)
        {
            if (root == null)
                return;
            PrintInorder(root.Left);
            foreach (double x in root.Data) Console.Write(x.ToString() + "  :  ");
            Console.WriteLine(" Depth : " + root.Depth.ToString());
            Console.WriteLine("------");
            PrintInorder(root.Right);
        }

        public void PrintInorder(Node root, double[] target)
        {
            if (root == null)
                return;
            PrintInorder(root.Left, target);
            foreach (double x in root.Data) Console.Write(x.ToString() + "  :  ");
            Console.WriteLine(Math.Sqrt((root.Data[0] - target[0]) * (root.Data[0] - target[0]) + (root.Data[1] - target[1]) * (root.Data[1] - target[1])));
            Console.WriteLine();
            Console.WriteLine("------");
            PrintInorder(root.Right, target);
        }

        public void printNode()
        {
            foreach (double x in Data) Console.Write(x.ToString() + "------");
            Console.WriteLine(Depth);
            Console.WriteLine();
        }


    }
    /// <summary>
    /// a k-d tree (short for k-dimensional tree) is a space-partitioning data structure for organizing points in a k-dimensional space
    /// </summary>
    class KDtree
    {
        public int Count = 0;  //節點的數量
        public Node Root;     //樹根
        /// <summary>
        /// 輸入一個List，裡面的元素是長度為2的一維陣列
        /// </summary>
        /// <param name="Points"> 是一個List</param>
        public KDtree(List<double[]> Points)
        {
            Root = kdtree(Points, 0);
        }


        private int Compare(double[] a, double[] b, int sp)
        {
            if (a[sp] > b[sp])
            {
                return 1;
            }
            else if (a[sp] == b[sp])
            {
                return 0;
            }
            else
            {
                return -1;
            }

        }
        /// <summary>
        /// 從根節點插入新節點
        /// </summary>
        /// <param name="root">根節點</param>
        /// <param name="item">插入的元素為長度為2的一維陣列</param>
        /// <param name="sp">根據陣列index的位置來比較大小</param>
        /// <returns></returns>
        private Node InsertionPoint(Node root, double[] item, int sp)
        {

            if (root != null && item[sp] > root.Data[sp])
            {
                root.Right = InsertionPoint(root.Right, item, (sp + 1) % 2);
            }
            else if (root != null)
            {
                root.Left = InsertionPoint(root.Left, item, (sp + 1) % 2);
            }
            if (root == null)
            {
                this.Count += 1;
                root = new Node(item, (sp + 1) % 2);
            }
            return root;
        }
        public Node kdtree(List<double[]> points, int depth)
        {
            int split = depth % 2;
            points.Sort((x, y) => Compare(x, y, split));

            Node node = new Node(points[points.Count / 2], depth);
            Count += 1;
            if (points.Count > 2)
            {
                List<double[]> Rl = points.GetRange(0, points.Count / 2);
                List<double[]> Rr = points.GetRange((points.Count / 2) + 1, points.Count - 1 - (points.Count / 2));

                node.Left = kdtree(Rl, depth + 1);
                node.Right = kdtree(Rr, depth + 1);
            }
            else if (points.Count == 2)
            {
                node.Left = InsertionPoint(node.Left, points[0], depth);
                node.Right = null;
            }
            else
            {
                node.Left = null;
                node.Right = null;
            }

            return node;

        }
        private double Distance(double[] source, double[] target)
        {
            double distance = 0;
            for (int i = 0; i < 2; ++i) distance += ((source[i] - target[i]) * (source[i] - target[i]));
            distance = Math.Sqrt(distance);
            return distance;
        }
        private Node Closest(Node a, Node b, double[] target)
        {
            if (a == null) return b;
            if (b == null) return a;
            double da = Distance(a.Data, target);
            double db = Distance(b.Data, target);
            return (da > db ? b : a);
        }
        public Node NearestSearch(Node root, double[] target, int depth)
        {
            if (root == null) return null;

            Node nextBranch = null;
            Node otherBranch = null;

            if (target[depth % 2] < root.Data[depth % 2])
            {
                nextBranch = root.Left;
                otherBranch = root.Right;
            }
            else
            {
                nextBranch = root.Right;
                otherBranch = root.Left;
            }

            Node temp = NearestSearch(nextBranch, target, depth + 1);
            Node best = Closest(root, temp, target);

            double radius = Distance(best.Data, target);

            double dis = Math.Abs(target[depth % 2] - root.Data[depth % 2]);

            if (radius >= dis)
            {
                temp = NearestSearch(otherBranch, target, depth + 1);
                best = Closest(temp, best, target);
            }
            return best;
        }
        /// <summary>
        /// 找出最靠近target的節點
        /// </summary>
        /// <param name="target">你要找尋的目標</param>
        /// <returns>回傳節點</returns>
        public Node NearestNode(double[] target)
        {
            return NearestSearch(Root, target, 0);
        }
    }
}
