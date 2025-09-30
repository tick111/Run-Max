using System.Runtime.CompilerServices;

namespace Assesment.Leaderboard;

internal sealed class OrderStatisticAvlTree
{
    internal sealed class Node
    {
        public long CustomerId;
        public decimal Score;
        public Node? Left, Right;
        public int Height = 1;
        public int SubtreeCount = 1;
        public Node(long id, decimal score) { CustomerId = id; Score = score; }
    }
    private Node? _root;
    public int Count => _root?.SubtreeCount ?? 0;
    private static int Compare(long c1, decimal s1, long c2, decimal s2)
    {
        int scoreCmp = s2.CompareTo(s1);
        if (scoreCmp != 0) return scoreCmp;
        return c1.CompareTo(c2);
    }
    public Node Insert(long customerId, decimal score)
    { _root = InsertInternal(_root, customerId, score, out var inserted); return inserted!; }
    public void Remove(Node node) => _root = RemoveInternal(_root, node.CustomerId, node.Score);
    public Node? SelectByRank(int rank)
    { if (rank <=0 || rank>Count) return null; return SelectByRankInternal(_root, rank);}    
    public int GetRank(Node node)
    { int rank=0; var cur=_root; while(cur!=null){int cmp=Compare(node.CustomerId,node.Score,cur.CustomerId,cur.Score); if(cmp==0){rank+=(cur.Left?.SubtreeCount??0)+1; return rank;} if(cmp<0) cur=cur.Left; else {rank+=(cur.Left?.SubtreeCount??0)+1; cur=cur.Right;}} throw new InvalidOperationException("Node not found"); }
    private static Node? InsertInternal(Node? root,long cid,decimal score,out Node? inserted){ if(root==null){ inserted=new Node(cid,score); return inserted;} int cmp=Compare(cid,score,root.CustomerId,root.Score); if(cmp<0) root.Left=InsertInternal(root.Left,cid,score,out inserted); else root.Right=InsertInternal(root.Right,cid,score,out inserted); Update(root); return Balance(root);}    
    private static Node? RemoveInternal(Node? root,long cid,decimal score){ if(root==null) return null; int cmp=Compare(cid,score,root.CustomerId,root.Score); if(cmp<0) root.Left=RemoveInternal(root.Left,cid,score); else if(cmp>0) root.Right=RemoveInternal(root.Right,cid,score); else { if(root.Left==null) return root.Right; if(root.Right==null) return root.Left; var succ=Min(root.Right); root.CustomerId=succ.CustomerId; root.Score=succ.Score; root.Right=RemoveInternal(root.Right,succ.CustomerId,succ.Score);} Update(root); return Balance(root);}    
    private static Node Min(Node n){ while(n.Left!=null) n=n.Left; return n; }
    private static Node? SelectByRankInternal(Node? node,int rank){ while(node!=null){int left=node.Left?.SubtreeCount??0; if(rank==left+1) return node; if(rank<=left) node=node.Left; else {rank-=left+1; node=node.Right;}} return null; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private static void Update(Node n){ n.Height=1+Math.Max(n.Left?.Height??0,n.Right?.Height??0); n.SubtreeCount=1+(n.Left?.SubtreeCount??0)+(n.Right?.SubtreeCount??0);}    
    private static int BalanceFactor(Node n)=>(n.Left?.Height??0)-(n.Right?.Height??0);    
    private static Node Balance(Node n){ int bf=BalanceFactor(n); if(bf>1){ if(BalanceFactor(n.Left!)<0) n.Left=RotateLeft(n.Left!); return RotateRight(n);} if(bf<-1){ if(BalanceFactor(n.Right!)>0) n.Right=RotateRight(n.Right!); return RotateLeft(n);} return n;}    
    private static Node RotateRight(Node y){ var x=y.Left!; var t2=x.Right; x.Right=y; y.Left=t2; Update(y); Update(x); return x;}    
    private static Node RotateLeft(Node x){ var y=x.Right!; var t2=y.Left; y.Left=x; x.Right=t2; Update(x); Update(y); return y;} }
