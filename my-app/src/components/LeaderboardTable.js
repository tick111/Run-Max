import React, { useState, useEffect } from 'react';

const LeaderboardTable = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [startRank, setStartRank] = useState(1);
  const [endRank, setEndRank] = useState(50);

  const fetchLeaderboard = async () => {
    try {
      setLoading(true);
      setError(null);

      console.log(`获取排行榜数据: ${startRank} 到 ${endRank}`);
      const response = await fetch(`https://localhost:7006/api/leaderboard?start=${startRank}&end=${endRank}`, {
        method: 'GET',
        mode: 'cors',
        headers: {
          'Accept': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result = await response.json();
      console.log('获取到的数据:', result);
      setData(Array.isArray(result) ? result : []);
    } catch (err) {
      console.error('获取排行榜失败:', err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchLeaderboard();
  }, [startRank, endRank]);

  const handleRangeChange = () => {
    fetchLeaderboard();
  };

  if (loading) {
    return (
      <div style={{ 
        padding: '20px', 
        textAlign: 'center',
        border: '1px solid #ddd',
        borderRadius: '8px',
        margin: '20px',
        backgroundColor: '#f9f9f9'
      }}>
        <h2 style={{ color: '#333' }}>🏆 排行榜</h2>
        <p style={{ color: '#666' }}>正在加载数据...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ 
        padding: '20px', 
        textAlign: 'center',
        border: '1px solid #ff4444',
        borderRadius: '8px',
        margin: '20px',
        backgroundColor: '#ffe6e6'
      }}>
        <h2 style={{ color: '#333' }}>🏆 排行榜</h2>
        <p style={{ color: '#cc0000' }}>加载失败: {error}</p>
        <button 
          onClick={fetchLeaderboard}
          style={{
            padding: '8px 16px',
            backgroundColor: '#007cba',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer'
          }}
        >
          重试
        </button>
      </div>
    );
  }

  return (
    <div style={{ 
      padding: '20px',
      border: '1px solid #ddd',
      borderRadius: '8px',
      margin: '20px',
      backgroundColor: '#f9f9f9'
    }}>
      <h2 style={{ textAlign: 'center', marginBottom: '20px', color: '#333' }}>🏆 排行榜</h2>
      
      {/* 控制面板 */}
      <div style={{ 
        marginBottom: '20px', 
        padding: '15px', 
        backgroundColor: 'white', 
        borderRadius: '5px',
        border: '1px solid #eee'
      }}>
        <h4 style={{ marginBottom: '10px', color: '#333' }}>查询设置</h4>
        <div style={{ display: 'flex', gap: '10px', alignItems: 'center', flexWrap: 'wrap' }}>
          <label style={{ color: '#333' }}>
            开始排名: 
            <input 
              type="number" 
              value={startRank} 
              onChange={(e) => setStartRank(Number(e.target.value))}
              min="1"
              style={{ marginLeft: '5px', padding: '4px', width: '80px', color: '#333', backgroundColor: 'white' }}
            />
          </label>
          <label style={{ color: '#333' }}>
            结束排名: 
            <input 
              type="number" 
              value={endRank} 
              onChange={(e) => setEndRank(Number(e.target.value))}
              min="1"
              style={{ marginLeft: '5px', padding: '4px', width: '80px', color: '#333', backgroundColor: 'white' }}
            />
          </label>
          <button 
            onClick={handleRangeChange}
            style={{
              padding: '6px 12px',
              backgroundColor: '#28a745',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            查询
          </button>
          <button 
            onClick={fetchLeaderboard}
            style={{
              padding: '6px 12px',
              backgroundColor: '#007cba',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            刷新
          </button>
        </div>
      </div>

      {/* 排行榜表格 */}
      <div style={{ 
        backgroundColor: 'white',
        borderRadius: '5px',
        border: '1px solid #eee',
        overflowX: 'auto'
      }}>
        {data.length === 0 ? (
          <div style={{ padding: '20px', textAlign: 'center', color: '#555' }}>
            暂无排行榜数据
          </div>
        ) : (
          <table style={{ 
            width: '100%', 
            borderCollapse: 'collapse',
            fontFamily: 'Arial, sans-serif'
          }}>
            <thead>
              <tr style={{ backgroundColor: '#f8f9fa' }}>
                <th style={{ border: '1px solid #ddd', padding: '12px', textAlign: 'center', color: '#333' }}>排名</th>
                <th style={{ border: '1px solid #ddd', padding: '12px', textAlign: 'left', color: '#333' }}>客户ID</th>
                <th style={{ border: '1px solid #ddd', padding: '12px', textAlign: 'right', color: '#333' }}>分数</th>
              </tr>
            </thead>
            <tbody>
              {data.map((item, index) => {
                const rank = startRank + index;
                const rowColor = rank <= 3 ? '#fff3cd' : rank % 2 === 0 ? '#f8f9fa' : 'white';
                const medal = rank === 1 ? '🥇' : rank === 2 ? '🥈' : rank === 3 ? '🥉' : '';
                
                return (
                  <tr key={index} style={{ backgroundColor: rowColor }}>
                    <td style={{ 
                      border: '1px solid #ddd', 
                      padding: '10px', 
                      textAlign: 'center',
                      fontWeight: rank <= 3 ? 'bold' : 'normal',
                      color: '#333'
                    }}>
                      {medal} {rank}
                    </td>
                    <td style={{ border: '1px solid #ddd', padding: '10px', color: '#333' }}>
                      {item.customerId || item.CustomerId || 'N/A'}
                    </td>
                    <td style={{ 
                      border: '1px solid #ddd', 
                      padding: '10px', 
                      textAlign: 'right',
                      fontWeight: 'bold',
                      color: '#333'
                    }}>
                      {typeof item.score === 'number' ? item.score.toFixed(2) : (item.Score || 0)}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}
      </div>

      <div style={{ marginTop: '10px', fontSize: '14px', color: '#555', textAlign: 'center' }}>
        显示排名 {startRank} - {Math.min(endRank, startRank + data.length - 1)} 
        {data.length > 0 && ` (共 ${data.length} 条记录)`}
      </div>
    </div>
  );
};

export default LeaderboardTable;