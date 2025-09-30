using System.Runtime.CompilerServices;

public sealed class OrderStatisticAvlTree
{
    public sealed class Node
    {
        public long CustomerId;
        public decimal Score;

        public Node? Left;
        public Node? Right;

        public int Height = 1;
        public int SubtreeCount = 1; // number of nodes in subtree (including self)

        public Node(long customerId, decimal score)
        {
            CustomerId = customerId;
            Score = score;
        }
    }

    private Node? _root;

    public int Count => _root?.SubtreeCount ?? 0;

    // Ordering: higher Score first; tie => lower CustomerId first
    private static int Compare(long c1, decimal s1, long c2, decimal s2)
    {
        int scoreCmp = s2.CompareTo(s1); // descending
        if (scoreCmp != 0) return scoreCmp;
        return c1.CompareTo(c2);         // ascending by customerId
    }

    public Node Insert(long customerId, decimal score)
    {
        _root = InsertInternal(_root, customerId, score, out var inserted);
        return inserted!;
    }

    public void Remove(Node node)
    {
        _root = RemoveInternal(_root, node.CustomerId, node.Score);
    }

    public Node? SelectByRank(int rank)
    {
        if (rank <= 0 || rank > Count) return null;
        return SelectByRankInternal(_root, rank);
    }

    public int GetRank(Node node)
    {
        // Rank is 1-based
        int rank = 0;
        var current = _root;
        while (current != null)
        {
            int cmp = Compare(node.CustomerId, node.Score, current.CustomerId, current.Score);
            if (cmp == 0)
            {
                rank += (current.Left?.SubtreeCount ?? 0) + 1;
                return rank;
            }
            else if (cmp < 0)
            {
                // node should be in left subtree
                current = current.Left;
            }
            else
            {
                // skip left subtree + current
                rank += (current.Left?.SubtreeCount ?? 0) + 1;
                current = current.Right;
            }
        }
        throw new InvalidOperationException("Node not found in tree.");
    }

    #region AVL Core

    private static Node? InsertInternal(Node? root, long customerId, decimal score, out Node? inserted)
    {
        if (root == null)
        {
            inserted = new Node(customerId, score);
            return inserted;
        }

        int cmp = Compare(customerId, score, root.CustomerId, root.Score);
        if (cmp < 0)
        {
            root.Left = InsertInternal(root.Left, customerId, score, out inserted);
        }
        else
        {
            root.Right = InsertInternal(root.Right, customerId, score, out inserted);
        }

        Update(root);
        return Balance(root);
    }

    private static Node? RemoveInternal(Node? root, long customerId, decimal score)
    {
        if (root == null) return null;

        int cmp = Compare(customerId, score, root.CustomerId, root.Score);
        if (cmp < 0)
        {
            root.Left = RemoveInternal(root.Left, customerId, score);
        }
        else if (cmp > 0)
        {
            root.Right = RemoveInternal(root.Right, customerId, score);
        }
        else
        {
            // Remove this node
            if (root.Left == null) return root.Right;
            if (root.Right == null) return root.Left;

            // Replace with inorder successor (smallest in right subtree)
            var successor = MinNode(root.Right);
            root.CustomerId = successor.CustomerId;
            root.Score = successor.Score;
            root.Right = RemoveInternal(root.Right, successor.CustomerId, successor.Score);
        }

        Update(root);
        return Balance(root);
    }

    private static Node MinNode(Node node)
    {
        while (node.Left != null) node = node.Left;
        return node;
    }

    private static Node? SelectByRankInternal(Node? node, int rank)
    {
        while (node != null)
        {
            int leftSize = node.Left?.SubtreeCount ?? 0;
            if (rank == leftSize + 1)
                return node;
            if (rank <= leftSize)
            {
                node = node.Left;
            }
            else
            {
                rank -= leftSize + 1;
                node = node.Right;
            }
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Update(Node n)
    {
        n.Height = 1 + Math.Max(n.Left?.Height ?? 0, n.Right?.Height ?? 0);
        n.SubtreeCount = 1 + (n.Left?.SubtreeCount ?? 0) + (n.Right?.SubtreeCount ?? 0);
    }

    private static int BalanceFactor(Node n) => (n.Left?.Height ?? 0) - (n.Right?.Height ?? 0);

    private static Node Balance(Node n)
    {
        int bf = BalanceFactor(n);
        if (bf > 1)
        {
            if (BalanceFactor(n.Left!) < 0)
                n.Left = RotateLeft(n.Left!);
            return RotateRight(n);
        }
        if (bf < -1)
        {
            if (BalanceFactor(n.Right!) > 0)
                n.Right = RotateRight(n.Right!);
            return RotateLeft(n);
        }
        return n;
    }

    private static Node RotateRight(Node y)
    {
        var x = y.Left!;
        var t2 = x.Right;

        x.Right = y;
        y.Left = t2;

        Update(y);
        Update(x);
        return x;
    }

    private static Node RotateLeft(Node x)
    {
        var y = x.Right!;
        var t2 = y.Left;

        y.Left = x;
        x.Right = t2;

        Update(x);
        Update(y);
        return y;
    }

    #endregion
}using System.Runtime.CompilerServices;

public sealed class OrderStatisticAvlTree
{
    public sealed class Node
    {
        public long CustomerId;
        public decimal Score;

        public Node? Left;
        public Node? Right;

        public int Height = 1;
        public int SubtreeCount = 1; // number of nodes in subtree (including self)

        public Node(long customerId, decimal score)
        {
            CustomerId = customerId;
            Score = score;
        }
    }

    private Node? _root;

    public int Count => _root?.SubtreeCount ?? 0;

    // Ordering: higher Score first; tie => lower CustomerId first
    private static int Compare(long c1, decimal s1, long c2, decimal s2)
    {
        int scoreCmp = s2.CompareTo(s1); // descending
        if (scoreCmp != 0) return scoreCmp;
        return c1.CompareTo(c2);         // ascending by customerId
    }

    public Node Insert(long customerId, decimal score)
    {
        _root = InsertInternal(_root, customerId, score, out var inserted);
        return inserted!;
    }

    public void Remove(Node node)
    {
        _root = RemoveInternal(_root, node.CustomerId, node.Score);
    }

    public Node? SelectByRank(int rank)
    {
        if (rank <= 0 || rank > Count) return null;
        return SelectByRankInternal(_root, rank);
    }

    public int GetRank(Node node)
    {
        // Rank is 1-based
        int rank = 0;
        var current = _root;
        while (current != null)
        {
            int cmp = Compare(node.CustomerId, node.Score, current.CustomerId, current.Score);
            if (cmp == 0)
            {
                rank += (current.Left?.SubtreeCount ?? 0) + 1;
                return rank;
            }
            else if (cmp < 0)
            {
                // node should be in left subtree
                current = current.Left;
            }
            else
            {
                // skip left subtree + current
                rank += (current.Left?.SubtreeCount ?? 0) + 1;
                current = current.Right;
            }
        }
        throw new InvalidOperationException("Node not found in tree.");
    }

    #region AVL Core

    private static Node? InsertInternal(Node? root, long customerId, decimal score, out Node? inserted)
    {
        if (root == null)
        {
            inserted = new Node(customerId, score);
            return inserted;
        }

        int cmp = Compare(customerId, score, root.CustomerId, root.Score);
        if (cmp < 0)
        {
            root.Left = InsertInternal(root.Left, customerId, score, out inserted);
        }
        else
        {
            root.Right = InsertInternal(root.Right, customerId, score, out inserted);
        }

        Update(root);
        return Balance(root);
    }

    private static Node? RemoveInternal(Node? root, long customerId, decimal score)
    {
        if (root == null) return null;

        int cmp = Compare(customerId, score, root.CustomerId, root.Score);
        if (cmp < 0)
        {
            root.Left = RemoveInternal(root.Left, customerId, score);
        }
        else if (cmp > 0)
        {
            root.Right = RemoveInternal(root.Right, customerId, score);
        }
        else
        {
            // Remove this node
            if (root.Left == null) return root.Right;
            if (root.Right == null) return root.Left;

            // Replace with inorder successor (smallest in right subtree)
            var successor = MinNode(root.Right);
            root.CustomerId = successor.CustomerId;
            root.Score = successor.Score;
            root.Right = RemoveInternal(root.Right, successor.CustomerId, successor.Score);
        }

        Update(root);
        return Balance(root);
    }

    private static Node MinNode(Node node)
    {
        while (node.Left != null) node = node.Left;
        return node;
    }

    private static Node? SelectByRankInternal(Node? node, int rank)
    {
        while (node != null)
        {
            int leftSize = node.Left?.SubtreeCount ?? 0;
            if (rank == leftSize + 1)
                return node;
            if (rank <= leftSize)
            {
                node = node.Left;
            }
            else
            {
                rank -= leftSize + 1;
                node = node.Right;
            }
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Update(Node n)
    {
        n.Height = 1 + Math.Max(n.Left?.Height ?? 0, n.Right?.Height ?? 0);
        n.SubtreeCount = 1 + (n.Left?.SubtreeCount ?? 0) + (n.Right?.SubtreeCount ?? 0);
    }

    private static int BalanceFactor(Node n) => (n.Left?.Height ?? 0) - (n.Right?.Height ?? 0);

    private static Node Balance(Node n)
    {
        int bf = BalanceFactor(n);
        if (bf > 1)
        {
            if (BalanceFactor(n.Left!) < 0)
                n.Left = RotateLeft(n.Left!);
            return RotateRight(n);
        }
        if (bf < -1)
        {
            if (BalanceFactor(n.Right!) > 0)
                n.Right = RotateRight(n.Right!);
            return RotateLeft(n);
        }
        return n;
    }

    private static Node RotateRight(Node y)
    {
        var x = y.Left!;
        var t2 = x.Right;

        x.Right = y;
        y.Left = t2;

        Update(y);
        Update(x);
        return x;
    }

    private static Node RotateLeft(Node x)
    {
        var y = x.Right!;
        var t2 = y.Left;

        y.Left = x;
        x.Right = t2;

        Update(x);
        Update(y);
        return y;
    }

    #endregion
}using System.Runtime.CompilerServices;

public sealed class OrderStatisticAvlTree
{
    public sealed class Node
    {
        public long CustomerId;
        public decimal Score;

        public Node? Left;
        public Node? Right;

        public int Height = 1;
        public int SubtreeCount = 1; // number of nodes in subtree (including self)

        public Node(long customerId, decimal score)
        {
            CustomerId = customerId;
            Score = score;
        }
    }

    private Node? _root;

    public int Count => _root?.SubtreeCount ?? 0;

    // Ordering: higher Score first; tie => lower CustomerId first
    private static int Compare(long c1, decimal s1, long c2, decimal s2)
    {
        int scoreCmp = s2.CompareTo(s1); // descending
        if (scoreCmp != 0) return scoreCmp;
        return c1.CompareTo(c2);         // ascending by customerId
    }

    public Node Insert(long customerId, decimal score)
    {
        _root = InsertInternal(_root, customerId, score, out var inserted);
        return inserted!;
    }

    public void Remove(Node node)
    {
        _root = RemoveInternal(_root, node.CustomerId, node.Score);
    }

    public Node? SelectByRank(int rank)
    {
        if (rank <= 0 || rank > Count) return null;
        return SelectByRankInternal(_root, rank);
    }

    public int GetRank(Node node)
    {
        // Rank is 1-based
        int rank = 0;
        var current = _root;
        while (current != null)
        {
            int cmp = Compare(node.CustomerId, node.Score, current.CustomerId, current.Score);
            if (cmp == 0)
            {
                rank += (current.Left?.SubtreeCount ?? 0) + 1;
                return rank;
            }
            else if (cmp < 0)
            {
                // node should be in left subtree
                current = current.Left;
            }
            else
            {
                // skip left subtree + current
                rank += (current.Left?.SubtreeCount ?? 0) + 1;
                current = current.Right;
            }
        }
        throw new InvalidOperationException("Node not found in tree.");
    }

    #region AVL Core

    private static Node? InsertInternal(Node? root, long customerId, decimal score, out Node? inserted)
    {
        if (root == null)
        {
            inserted = new Node(customerId, score);
            return inserted;
        }

        int cmp = Compare(customerId, score, root.CustomerId, root.Score);
        if (cmp < 0)
        {
            root.Left = InsertInternal(root.Left, customerId, score, out inserted);
        }
        else
        {
            root.Right = InsertInternal(root.Right, customerId, score, out inserted);
        }

        Update(root);
        return Balance(root);
    }

    private static Node? RemoveInternal(Node? root, long customerId, decimal score)
    {
        if (root == null) return null;

        int cmp = Compare(customerId, score, root.CustomerId, root.Score);
        if (cmp < 0)
        {
            root.Left = RemoveInternal(root.Left, customerId, score);
        }
        else if (cmp > 0)
        {
            root.Right = RemoveInternal(root.Right, customerId, score);
        }
        else
        {
            // Remove this node
            if (root.Left == null) return root.Right;
            if (root.Right == null) return root.Left;

            // Replace with inorder successor (smallest in right subtree)
            var successor = MinNode(root.Right);
            root.CustomerId = successor.CustomerId;
            root.Score = successor.Score;
            root.Right = RemoveInternal(root.Right, successor.CustomerId, successor.Score);
        }

        Update(root);
        return Balance(root);
    }

    private static Node MinNode(Node node)
    {
        while (node.Left != null) node = node.Left;
        return node;
    }

    private static Node? SelectByRankInternal(Node? node, int rank)
    {
        while (node != null)
        {
            int leftSize = node.Left?.SubtreeCount ?? 0;
            if (rank == leftSize + 1)
                return node;
            if (rank <= leftSize)
            {
                node = node.Left;
            }
            else
            {
                rank -= leftSize + 1;
                node = node.Right;
            }
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Update(Node n)
    {
        n.Height = 1 + Math.Max(n.Left?.Height ?? 0, n.Right?.Height ?? 0);
        n.SubtreeCount = 1 + (n.Left?.SubtreeCount ?? 0) + (n.Right?.SubtreeCount ?? 0);
    }

    private static int BalanceFactor(Node n) => (n.Left?.Height ?? 0) - (n.Right?.Height ?? 0);

    private static Node Balance(Node n)
    {
        int bf = BalanceFactor(n);
        if (bf > 1)
        {
            if (BalanceFactor(n.Left!) < 0)
                n.Left = RotateLeft(n.Left!);
            return RotateRight(n);
        }
        if (bf < -1)
        {
            if (BalanceFactor(n.Right!) > 0)
                n.Right = RotateRight(n.Right!);
            return RotateLeft(n);
        }
        return n;
    }

    private static Node RotateRight(Node y)
    {
        var x = y.Left!;
        var t2 = x.Right;

        x.Right = y;
        y.Left = t2;

        Update(y);
        Update(x);
        return x;
    }

    private static Node RotateLeft(Node x)
    {
        var y = x.Right!;
        var t2 = y.Left;

        y.Left = x;
        x.Right = t2;

        Update(x);
        Update(y);
        return y;
    }

    #endregion
}