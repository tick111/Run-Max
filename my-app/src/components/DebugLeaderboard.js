import React, { useState, useEffect } from 'react';

const DebugLeaderboard = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);

        console.log('开始获取数据...');
        const response = await fetch('https://localhost:7006/api/leaderboard?start=1&end=50', {
          method: 'GET',
          mode: 'cors',
          headers: {
            'Accept': 'application/json',
          },
        });

        console.log('响应状态:', response.status);
        console.log('响应头:', response.headers.get('content-type'));

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const contentType = response.headers.get('content-type');
        let result;
        
        if (contentType && contentType.includes('application/json')) {
          result = await response.json();
          console.log('JSON数据:', result);
        } else {
          result = await response.text();
          console.log('文本数据:', result);
        }

        setData(result);
      } catch (err) {
        console.error('获取数据失败:', err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return <div style={{ padding: '20px' }}>正在加载排行榜...</div>;
  }

  if (error) {
    return (
      <div style={{ padding: '20px', color: 'red' }}>
        错误: {error}
      </div>
    );
  }

  return (
    <div style={{ padding: '20px', border: '1px solid #ccc', margin: '20px' }}>
      <h2>🔍 调试信息</h2>
      
      <div style={{ marginBottom: '20px' }}>
        <h3>数据类型:</h3>
        <p><strong>{typeof data}</strong></p>
      </div>

      <div style={{ marginBottom: '20px' }}>
        <h3>是否为数组:</h3>
        <p><strong>{Array.isArray(data) ? '是' : '否'}</strong></p>
      </div>

      {Array.isArray(data) && (
        <div style={{ marginBottom: '20px' }}>
          <h3>数组长度:</h3>
          <p><strong>{data.length}</strong></p>
        </div>
      )}

      <div style={{ marginBottom: '20px' }}>
        <h3>原始数据:</h3>
        <pre style={{ 
          backgroundColor: '#f5f5f5', 
          padding: '10px', 
          overflow: 'auto',
          fontSize: '12px',
          border: '1px solid #ddd'
        }}>
          {typeof data === 'string' ? data : JSON.stringify(data, null, 2)}
        </pre>
      </div>

      {Array.isArray(data) && data.length > 0 && (
        <div>
          <h3>渲染表格测试:</h3>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr style={{ backgroundColor: '#f8f9fa' }}>
                <th style={{ border: '1px solid #ddd', padding: '8px' }}>排名</th>
                <th style={{ border: '1px solid #ddd', padding: '8px' }}>客户ID</th>
                <th style={{ border: '1px solid #ddd', padding: '8px' }}>分数</th>
              </tr>
            </thead>
            <tbody>
              {data.slice(0, 5).map((item, index) => (
                <tr key={index}>
                  <td style={{ border: '1px solid #ddd', padding: '8px' }}>{index + 1}</td>
                  <td style={{ border: '1px solid #ddd', padding: '8px' }}>
                    {item.customerId || item.CustomerId || item.id || 'N/A'}
                  </td>
                  <td style={{ border: '1px solid #ddd', padding: '8px' }}>
                    {item.score || item.Score || item.points || 'N/A'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default DebugLeaderboard;