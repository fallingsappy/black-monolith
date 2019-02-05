using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulGraphMethods
{
    class BinaryNode
    {
        public BinaryNode left { get; set; }
        public BinaryNode right { get; set; }
        public BinaryNode parent { get; set; }
        public int data;
    }

    class Tree
    {
        public void printTree(BinaryNode root)
        {
            if (root != null)
            {
                Console.Write(root.data);
                if (root.left != null || root.right != null)
                {
                    Console.Write("(");

                    if (root.left == null || root.left.data == 0)
                        Console.Write("NULL");
                    else
                        printTree(root.left);

                    Console.Write(",");

                    if (root.right == null || root.right.data == 0)
                        Console.Write("NULL");
                    else
                        printTree(root.right);

                    Console.Write(")");
                }
            }
        }

        public BinaryNode insert(BinaryNode root, int value)
        {
            if (root == null)
            {
                root = new BinaryNode();
                root.data = value;
            }
            else if (value < root.data)
            {
                root.left = insert(root.left, value);
            }
            else
            {
                root.right = insert(root.right, value);
            }

            return root;
        }

        public void preOrderTravers(BinaryNode root)
        {
            if (root != null && root.data != 0)
            {
                Console.Write(root.data);
                Console.Write(" ");

                preOrderTravers(root.left);
                preOrderTravers(root.right);
            }
            else
                return;
        }

        public void inOrderTravers(BinaryNode root)
        {
            if (root != null && root.data != 0)
            {
                preOrderTravers(root.left);
                Console.Write(root.data);
                Console.Write(" ");
                preOrderTravers(root.right);
            }
            else
                return;
        }

        public void postOrderTravers(BinaryNode root)
        {
            if (root != null && root.data != 0)
            {
                preOrderTravers(root.left);
                preOrderTravers(root.right);
                Console.Write(root.data);
                Console.Write(" ");
            }
            else
                return;
        }

        public string FindNode(BinaryNode root, int s)
        {
            if (root == null)
                return "Узел не найден";
            else if (s.CompareTo(root.data) < 0)
                return FindNode(root.left, s);
            else if (s.CompareTo(root.data) > 0)
                return FindNode(root.right, s);

            return "Узел найден";
        }

    }
}
