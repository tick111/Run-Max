# In-Memory Leaderboard Service (Mywork)

## 概述
本项目是一个基于 .NET 8 (Razor Pages + Minimal APIs) 的内存排行榜服务。  
特点：
- 分数 > 0 的用户才进入排行榜
- 排名：按分数降序；同分按 CustomerId 升序
- 数据结构：AVL + 子树节点数 (Order Statistic) + 字典
- O(log N) 更新；O(log N + k) 区间查询
- 无持久化（进程退出数据清空）

## 运行