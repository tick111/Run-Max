import React, { useState } from 'react';

const ApiTester = () => {
  const [endpoint, setEndpoint] = useState('/');
  const [method, setMethod] = useState('GET');
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const testEndpoint = async () => {
    setLoading(true);
    setError(null);
    setResult(null);

    try {
      console.log(`Testing: https://localhost:7006${endpoint}`);
      
      const response = await fetch(`https://localhost:7006${endpoint}`, {
        method: method,
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        },
      });

      console.log(`Response status: ${response.status}`);
      console.log(`Response headers:`, [...response.headers.entries()]);

      let data;
      const contentType = response.headers.get('content-type');
      
      if (contentType && contentType.includes('application/json')) {
        data = await response.json();
      } else {
        data = await response.text();
      }

      setResult({
        status: response.status,
        statusText: response.statusText,
        headers: [...response.headers.entries()],
        data: data,
        url: `https://localhost:7006${endpoint}`
      });

    } catch (err) {
      console.error('Test failed:', err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const commonEndpoints = [
    '/'
  ];

  return (
    <div style={{ padding: '20px', border: '1px solid #ccc', margin: '10px', borderRadius: '5px' }}>
      <h3>API Endpoint Tester</h3>
      
      <div style={{ marginBottom: '20px' }}>
        <h4>Test Base Connection:</h4>
        <button 
          onClick={() => {
            setEndpoint('/');
            setTimeout(() => testEndpoint(), 100);
          }}
          style={{ 
            padding: '10px 20px', 
            fontSize: '14px',
            backgroundColor: '#007cba',
            color: 'white',
            border: 'none',
            borderRadius: '3px',
            cursor: 'pointer'
          }}
        >
          Test https://localhost:7006/
        </button>
      </div>

      <div style={{ marginBottom: '20px' }}>
        <h4>Custom Endpoint Test:</h4>
        <div style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
          <span>https://localhost:7006</span>
          <input
            type="text"
            value={endpoint}
            onChange={(e) => setEndpoint(e.target.value)}
            placeholder="/your-endpoint"
            style={{ padding: '5px', minWidth: '200px' }}
          />
          <select 
            value={method} 
            onChange={(e) => setMethod(e.target.value)}
            style={{ padding: '5px' }}
          >
            <option value="GET">GET</option>
            <option value="POST">POST</option>
            <option value="PUT">PUT</option>
            <option value="DELETE">DELETE</option>
          </select>
          <button 
            onClick={testEndpoint}
            disabled={loading}
            style={{ 
              padding: '5px 15px',
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

      {error && (
        <div style={{ backgroundColor: '#ffe6e6', padding: '10px', borderRadius: '3px', marginBottom: '10px' }}>
          <strong style={{ color: 'red' }}>Error:</strong> {error}
          <div style={{ fontSize: '12px', marginTop: '5px' }}>
            <p>Common issues:</p>
            <ul>
              <li>SSL Certificate not trusted - try opening <a href={`https://localhost:7006${endpoint}`} target="_blank" rel="noopener noreferrer">the URL</a> in a new tab first</li>
              <li>CORS not configured for this origin</li>
              <li>API not running on port 7006</li>
            </ul>
          </div>
        </div>
      )}

      {result && (
        <div style={{ 
          backgroundColor: result.status >= 200 && result.status < 300 ? '#e6ffe6' : '#ffe6e6', 
          padding: '10px', 
          borderRadius: '3px' 
        }}>
          <h4>Response:</h4>
          <p><strong>URL:</strong> {result.url}</p>
          <p><strong>Status:</strong> {result.status} {result.statusText}</p>
          
          <details style={{ marginTop: '10px' }}>
            <summary style={{ cursor: 'pointer', fontWeight: 'bold' }}>Response Headers</summary>
            <pre style={{ fontSize: '11px', backgroundColor: '#f0f0f0', padding: '5px' }}>
              {result.headers.map(([key, value]) => `${key}: ${value}`).join('\n')}
            </pre>
          </details>

          <details style={{ marginTop: '10px' }}>
            <summary style={{ cursor: 'pointer', fontWeight: 'bold' }}>Response Data</summary>
            <pre style={{ 
              fontSize: '11px', 
              backgroundColor: '#f0f0f0', 
              padding: '10px', 
              overflow: 'auto',
              maxHeight: '300px'
            }}>
              {typeof result.data === 'string' ? result.data : JSON.stringify(result.data, null, 2)}
            </pre>
          </details>
        </div>
      )}
    </div>
  );
};

export default ApiTester;