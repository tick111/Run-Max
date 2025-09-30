using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// 添加内存缓存用于存储客户数据
builder.Services.AddMemoryCache();
// 添加并发字典用于线程安全的数据访问
builder.Services.AddSingleton<CustomerScoreStore>();

var app = builder.Build();

// 配置路由
app.MapPost("/customer/{customerid:long}/score/{score:decimal}", async (
    long customerid,
    decimal score,
    CustomerScoreStore store) =>
{
    if (score < -1000 || score > 1000)
    {
        return Results.BadRequest("Score must be between -1000 and 1000");
    }

    var newScore = await store.UpdateScoreAsync(customerid, score);
    return Results.Ok(new { Score = newScore });
});

app.MapGet("/leaderboard", async (
    int start,
    int end,
    CustomerScoreStore store) =>
{
    if (start < 1 || end < start)
    {
        return Results.BadRequest("Invalid start or end parameters");
    }

    var customers = await store.GetCustomersByRankRangeAsync(start, end);
    return Results.Ok(customers);
});

app.MapGet("/leaderboard/{customerid:long}", async (
    long customerid,
    int high = 0,
    int low = 0,
    CustomerScoreStore store) =>
{
    if (high < 0 || low < 0)
    {
        return Results.BadRequest("High and low parameters must be non-negative");
    }

    var result = await store.GetCustomerWithNeighborsAsync(customerid, high, low);
    if (result == null)
    {
        return Results.NotFound("Customer not found");
    }

    return Results.Ok(result);
});

app.Run();

// 客户模型
public class Customer
{
    public long Id { get; set; }
    public decimal Score { get; set; }
    public int Rank { get; set; }
}

// 客户分数存储和管理
public class CustomerScoreStore
{
    // 使用并发字典确保线程安全
    private readonly Dictionary<long, decimal> _customerScores = new Dictionary<long, decimal>();
    // 用于排序的客户列表，按分数降序和ID升序排列
    private List<Customer> _sortedCustomers = new List<Customer>();
    // 用于同步的锁对象
    private readonly object _lock = new object();

    public async Task<decimal> UpdateScoreAsync(long customerId, decimal scoreDelta)
    {
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                // 更新客户分数
                if (_customerScores.ContainsKey(customerId))
                {
                    _customerScores[customerId] += scoreDelta;
                }
                else
                {
                    _customerScores[customerId] = scoreDelta;
                }

                // 确保分数不会低于0
                if (_customerScores[customerId] < 0)
                {
                    _customerScores[customerId] = 0;
                }

                // 重新排序并计算排名
                UpdateSortedCustomers();

                return _customerScores[customerId];
            }
        });
    }

    public async Task<List<Customer>> GetCustomersByRankRangeAsync(int start, int end)
    {
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                return _sortedCustomers
                    .Where(c => c.Rank >= start && c.Rank <= end)
                    .OrderBy(c => c.Rank)
                    .ToList();
            }
        });
    }

    public async Task<List<Customer>> GetCustomerWithNeighborsAsync(long customerId, int high, int low)
    {
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                var customer = _sortedCustomers.FirstOrDefault(c => c.Id == customerId);
                if (customer == null)
                {
                    return null;
                }

                int index = _sortedCustomers.IndexOf(customer);

                // 计算要获取的范围
                int startIndex = Math.Max(0, index - high);
                int endIndex = Math.Min(_sortedCustomers.Count - 1, index + low);

                return _sortedCustomers.GetRange(startIndex, endIndex - startIndex + 1);
            }
        });
    }

    private void UpdateSortedCustomers()
    {
        // 获取所有分数大于0的客户并排序
        _sortedCustomers = _customerScores
            .Where(kvp => kvp.Value > 0)
            .Select(kvp => new Customer { Id = kvp.Key, Score = kvp.Value })
            .OrderByDescending(c => c.Score)
            .ThenBy(c => c.Id)
            .ToList();

        // 计算排名
        for (int i = 0; i < _sortedCustomers.Count; i++)
        {
            _sortedCustomers[i].Rank = i + 1;
        }
    }
}
