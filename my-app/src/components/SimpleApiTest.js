import React, { useState } from 'react';

const SimpleApiTest = () => {
  const [endpoint, setEndpoint] = useState('/leaderboard-ui');
  const [response, setResponse] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const testApi = async () => {
    if (!endpoint.trim()) {
      setError('Please enter an endpoint path');
      return;
    }

    setLoading(true);
    setError(null);
    setResponse(null);

    try {
      // Use relative URL to leverage the proxy
      const url = endpoint;
      console.log('Testing via proxy:', url, '(proxied to https://localhost:7006)');
      
      const res = await fetch(url, {
        method: 'GET',
        headers: {
          'Accept': 'application/json, text/plain, */*',
        },
      });

      const contentType = res.headers.get('content-type');
      let data;
      
      if (contentType && contentType.includes('application/json')) {
        data = await res.json();
      } else {
        data = await res.text();
      }

      setResponse({
        status: res.status,
        ok: res.ok,
        data: data,
        url: `https://localhost:7006${url} (via proxy)`
      });

    } catch (err) {
      console.error('API Error:', err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const testConnection = async () => {
    setLoading(true);
    setError(null);
    setResponse(null);

    try {
      // Use relative URL to leverage the proxy
      const url = '/';
      console.log('Testing base connection via proxy:', url, '(proxied to https://localhost:7006)');
      
      const res = await fetch(url, {
        method: 'GET',
        headers: {
          'Accept': '*/*',
        },
      });

      const data = await res.text();

      setResponse({
        status: res.status,
        ok: res.ok,
        data: data,
        url: 'https://localhost:7006/ (via proxy)'
      });

    } catch (err) {
      console.error('Connection Error:', err);
      setError(`Cannot connect to localhost:7006 - ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ 
      maxWidth: '800px', 
      margin: '20px auto', 
      padding: '20px', 
      border: '1px solid #ddd', 
      borderRadius: '8px',
      backgroundColor: '#f9f9f9'
    }}>
      <h2>API Connection Test</h2>
      
      {/* Test basic connection */}
      <div style={{ marginBottom: '30px', padding: '15px', backgroundColor: 'white', borderRadius: '5px' }}>
        <h3>Step 1: Test Base Connection</h3>
        <p>First, let's check if your API server is running:</p>
        <button 
          onClick={testConnection}
          disabled={loading}
          style={{ 
            padding: '10px 20px',
            fontSize: '16px',
            backgroundColor: loading ? '#ccc' : '#007cba',
            color: 'white',
            border: 'none',
            borderRadius: '5px',
            cursor: loading ? 'not-allowed' : 'pointer'
          }}
        >
          {loading ? 'Testing...' : 'Test Connection (via proxy)'}
        </button>
      </div>

      {/* Test leaderboard endpoint */}
      <div style={{ marginBottom: '30px', padding: '15px', backgroundColor: 'white', borderRadius: '5px' }}>
        <h3>Step 2: Test Leaderboard Endpoint</h3>
        <p>Test your leaderboard API:</p>
        <button 
          onClick={() => {
            setEndpoint('/leaderboard-ui');
            setTimeout(() => testApi(), 100);
          }}
          disabled={loading}
          style={{ 
            padding: '10px 20px',
            fontSize: '16px',
            backgroundColor: loading ? '#ccc' : '#28a745',
            color: 'white',
            border: 'none',
            borderRadius: '5px',
            cursor: loading ? 'not-allowed' : 'pointer',
            marginBottom: '15px'
          }}
        >
          {loading ? 'Testing...' : 'Test https://localhost:7006/leaderboard-ui'}
        </button>
        <p>Or enter a different endpoint path:</p>
        <div style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
          <span style={{ fontWeight: 'bold' }}>https://localhost:7006</span>
          <input
            type="text"
            value={endpoint}
            onChange={(e) => setEndpoint(e.target.value)}
            placeholder="/leaderboard-ui"
            style={{ 
              padding: '8px', 
              fontSize: '14px',
              minWidth: '200px',
              border: '1px solid #ddd',
              borderRadius: '3px'
            }}
            onKeyPress={(e) => e.key === 'Enter' && testApi()}
          />
          <button 
            onClick={testApi}
            disabled={loading}
            style={{ 
              padding: '8px 16px',
              backgroundColor: loading ? '#ccc' : '#28a745',
              color: 'white',
              border: 'none',
              borderRadius: '3px',
              cursor: loading ? 'not-allowed' : 'pointer'
            }}
          >
            {loading ? 'Testing...' : 'Test'}
          </button>
        </div>
      </div>

      {/* Display results */}
      {error && (
        <div style={{ 
          padding: '15px', 
          backgroundColor: '#ffe6e6', 
          border: '1px solid #ff4444',
          borderRadius: '5px',
          marginBottom: '20px'
        }}>
          <h4 style={{ color: '#cc0000', margin: '0 0 10px 0' }}>Connection Failed</h4>
          <p style={{ margin: '0 0 10px 0' }}><strong>Error:</strong> {error}</p>
          <div style={{ fontSize: '14px' }}>
            <p><strong>Troubleshooting:</strong></p>
            <ul>
              <li>Make sure your API is running on port 7006</li>
              <li>Check if you're using HTTP vs HTTPS</li>
              <li>Your browser might block self-signed certificates</li>
              <li>Try opening the URL directly in a new browser tab first</li>
            </ul>
          </div>
        </div>
      )}

      {response && (
        <div style={{ 
          padding: '15px', 
          backgroundColor: response.ok ? '#e6ffe6' : '#ffe6e6',
          border: `1px solid ${response.ok ? '#44aa44' : '#ff4444'}`,
          borderRadius: '5px'
        }}>
          <h4 style={{ 
            color: response.ok ? '#006600' : '#cc0000', 
            margin: '0 0 10px 0' 
          }}>
            {response.ok ? '✅ Success!' : '❌ Failed'}
          </h4>
          
          <p><strong>URL:</strong> {response.url}</p>
          <p><strong>Status:</strong> {response.status} {response.ok ? '(OK)' : '(Error)'}</p>
          
          <details style={{ marginTop: '15px' }}>
            <summary style={{ cursor: 'pointer', fontWeight: 'bold' }}>
              View Response Data
            </summary>
            <pre style={{ 
              backgroundColor: '#f5f5f5', 
              padding: '10px', 
              marginTop: '10px',
              borderRadius: '3px',
              overflow: 'auto',
              maxHeight: '400px',
              fontSize: '12px'
            }}>
              {typeof response.data === 'string' ? response.data : JSON.stringify(response.data, null, 2)}
            </pre>
          </details>
        </div>
      )}
    </div>
  );
};

export default SimpleApiTest;