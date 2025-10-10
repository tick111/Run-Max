import React, { useState, useEffect } from 'react';

const Leaderboard = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchLeaderboard = async () => {
      try {
        setLoading(true);
        setError(null);

        // 直接连接到API，不使用credentials避免CORS问题
        console.log('直接连接到: https://localhost:7006/leaderboard-ui');
        
        const response = await fetch('https://localhost:7006/leaderboard-ui', {
          method: 'GET',
          mode: 'cors',
          headers: {
            'Accept': 'application/json, text/html, */*',
          },
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const contentType = response.headers.get('content-type');
        let result;
        
        if (contentType && contentType.includes('application/json')) {
          result = await response.json();
        } else {
          result = await response.text();
        }

        setData(result);
      } catch (err) {
        console.error('Leaderboard fetch failed:', err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchLeaderboard();
  }, []);

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
        <h2>🏆 Leaderboard</h2>
        <p>Loading leaderboard data...</p>
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
        <h2>🏆 Leaderboard</h2>
        <p style={{ color: '#cc0000' }}>Error loading leaderboard: {error}</p>
      </div>
    );
  }

  // 渲染排行榜表格
  const renderLeaderboard = () => {
    // 如果是HTML，直接渲染
    if (typeof data === 'string' && data.includes('<')) {
      return (
        <div 
          dangerouslySetInnerHTML={{ __html: data }}
          style={{ 
            backgroundColor: 'white',
            padding: '15px',
            borderRadius: '5px',
            border: '1px solid #eee'
          }}
        />
      );
    }

    // 如果是JSON数组，渲染为表格
    if (Array.isArray(data)) {
      return (
        <div style={{ 
          backgroundColor: 'white',
          padding: '15px',
          borderRadius: '5px',
          border: '1px solid #eee',
          overflowX: 'auto'
        }}>
          <table style={{ 
            width: '100%', 
            borderCollapse: 'collapse',
            fontFamily: 'Arial, sans-serif'
          }}>
            <thead>
              <tr style={{ backgroundColor: '#f8f9fa' }}>
                <th style={{ border: '1px solid #ddd', padding: '12px', textAlign: 'left' }}>排名</th>
                <th style={{ border: '1px solid #ddd', padding: '12px', textAlign: 'left' }}>客户ID</th>
                <th style={{ border: '1px solid #ddd', padding: '12px', textAlign: 'right' }}>分数</th>
              </tr>
            </thead>
            <tbody>
              {data.map((item, index) => {
                const rank = index + 1;
                const rowColor = rank <= 3 ? '#fff3cd' : 'white';
                return (
                  <tr key={index} style={{ backgroundColor: rowColor }}>
                    <td style={{ border: '1px solid #ddd', padding: '8px' }}>
                      {rank <= 3 ? `🥇🥈🥉`[rank-1] : ''} {rank}
                    </td>
                    <td style={{ border: '1px solid #ddd', padding: '8px' }}>
                      {item.customerId || item.CustomerId || 'N/A'}
                    </td>
                    <td style={{ border: '1px solid #ddd', padding: '8px', textAlign: 'right' }}>
                      {item.score || item.Score || 0}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      );
    }

    // 如果是其他格式，显示原始数据
    return (
      <div style={{ 
        backgroundColor: 'white',
        padding: '15px',
        borderRadius: '5px',
        border: '1px solid #eee'
      }}>
        <pre style={{ 
          whiteSpace: 'pre-wrap',
          fontFamily: 'inherit',
          margin: 0 
        }}>
          {typeof data === 'string' ? data : JSON.stringify(data, null, 2)}
        </pre>
      </div>
    );
  };

  return (
    <div style={{ 
      padding: '20px',
      border: '1px solid #ddd',
      borderRadius: '8px',
      margin: '20px',
      backgroundColor: '#f9f9f9'
    }}>
      <h2 style={{ textAlign: 'center', marginBottom: '20px' }}>🏆 排行榜</h2>
      {renderLeaderboard()}
    </div>
  );
};

export default Leaderboard;