import React, { useState, useEffect } from 'react';

const ApiData = ({ endpoint = '/api/values', title = 'API Data' }) => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [attemptedEndpoints, setAttemptedEndpoints] = useState([]);

  // Using proxy, so we can use relative URLs
  const API_BASE_URL = '';

  useEffect(() => {
    const testUrls = [
      // Try direct connection first (bypassing proxy)
      `https://localhost:7006${endpoint}`,
      `http://localhost:7006${endpoint}`,
      `https://localhost:7006/api/values`,
      `http://localhost:7006/api/values`,
      `https://localhost:7006/api/weatherforecast`,
      `http://localhost:7006/api/weatherforecast`,
      // Then try with proxy
      endpoint,
      '/api/values',
      '/api/weatherforecast'
    ];

    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);
        const attempted = [];

        for (const testUrl of testUrls) {
          try {
            attempted.push(testUrl);
            console.log(`Trying URL: ${testUrl}`);

            const fetchOptions = {
              method: 'GET',
              headers: {
                'Accept': 'application/json',
              },
            };

            // Only add CORS settings for relative URLs (proxy)
            if (!testUrl.startsWith('http')) {
              fetchOptions.mode = 'cors';
              fetchOptions.credentials = 'include';
              fetchOptions.headers['Content-Type'] = 'application/json';
            }

            const response = await fetch(testUrl, fetchOptions);

            console.log(`${testUrl} - Status: ${response.status}, OK: ${response.ok}`);

            if (response.ok) {
              let result;
              const contentType = response.headers.get('content-type');
              
              if (contentType && contentType.includes('application/json')) {
                result = await response.json();
              } else {
                result = await response.text();
              }

              setData({ 
                endpoint: testUrl, 
                data: result,
                status: response.status,
                contentType: contentType 
              });
              setAttemptedEndpoints(attempted);
              return; // Success, exit the loop
            }
          } catch (endpointError) {
            console.log(`${testUrl} failed:`, endpointError.message);
          }
        }

        // If we get here, all endpoints failed
        setAttemptedEndpoints(attempted);
        throw new Error(`All URLs failed. Check console for details.`);

      } catch (err) {
        console.error('API call failed:', err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [endpoint]);

  if (loading) {
    return (
      <div style={{ padding: '20px', border: '1px solid #ddd', margin: '10px', borderRadius: '5px' }}>
        <h3>{title}</h3>
        <p>Loading...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: '20px', border: '1px solid #ff0000', margin: '10px', borderRadius: '5px', backgroundColor: '#ffe6e6' }}>
        <h3>{title}</h3>
        <p style={{ color: 'red' }}>Error: {error}</p>
        <div>
          <h4>Debug Information:</h4>
          <div style={{ textAlign: 'left', fontSize: '12px', backgroundColor: '#f0f0f0', padding: '10px', marginBottom: '10px' }}>
            <strong>Attempted URLs:</strong>
            <ul>
              {attemptedEndpoints.map((url, index) => (
                <li key={index}>{url}</li>
              ))}
            </ul>
          </div>
          <h4>Next Steps:</h4>
          <ol style={{ textAlign: 'left', fontSize: '14px' }}>
            <li><strong>Open browser dev tools (F12) → Console tab</strong> to see detailed error messages</li>
            <li><strong>Test direct API access:</strong> Try opening <a href="https://localhost:7006/api/values" target="_blank" rel="noopener noreferrer">https://localhost:7006/api/values</a> in a new tab</li>
            <li><strong>Check certificate:</strong> Your browser might be blocking self-signed SSL certificates</li>
            <li><strong>Try HTTP instead:</strong> Test <a href="http://localhost:7006/api/values" target="_blank" rel="noopener noreferrer">http://localhost:7006/api/values</a></li>
            <li><strong>CORS headers needed:</strong> Access-Control-Allow-Origin: http://localhost:3001</li>
          </ol>
        </div>
      </div>
    );
  }

  return (
    <div style={{ padding: '20px', border: '1px solid #00aa00', margin: '10px', borderRadius: '5px', backgroundColor: '#e6ffe6' }}>
      <h3>{title}</h3>
      <div>
        <h4 style={{ color: 'green' }}>✅ Success! Connected to: {data.endpoint}</h4>
        <p><strong>Status:</strong> {data.status}</p>
        <p><strong>Content-Type:</strong> {data.contentType}</p>
        <pre style={{ 
          backgroundColor: '#f5f5f5', 
          padding: '10px', 
          borderRadius: '3px', 
          overflow: 'auto',
          fontSize: '12px'
        }}>
          {typeof data.data === 'string' ? data.data : JSON.stringify(data.data, null, 2)}
        </pre>
      </div>
    </div>
  );
};

export default ApiData;