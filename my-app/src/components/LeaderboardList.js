import React, { useEffect, useState, useRef } from 'react';
import { getLeaderboard } from '../api/getLeaderboard';

export default function LeaderboardList({ baseUrl = '' , start = 1, end = 50 }) {
  const [items, setItems] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const abortRef = useRef(null);

  useEffect(() => {
    const controller = new AbortController();
    abortRef.current = controller;
    setLoading(true);
    setError(null);
    setItems(null);

    getLeaderboard({ baseUrl, start, end, signal: controller.signal })
      .then(data => setItems(data))
      .catch(err => {
        if (err.name === 'AbortError') return;
        setError(err.message || String(err));
      })
      .finally(() => setLoading(false));

    return () => controller.abort();
  }, [baseUrl, start, end]);

  if (loading) return <div>Loading leaderboard...</div>;
  if (error) return <div style={{color:'red'}}>Error: {error}</div>;
  if (!items || items.length === 0) return <div>No leaderboard entries</div>;

  return (
    <div style={{textAlign:'left', maxWidth:600, margin:'16px auto'}}>
      <h3>Leaderboard</h3>
      <ol start={start}>
        {items.map((it, idx) => (
          <li key={it.customerId ?? idx} style={{padding:'8px 0', borderBottom:'1px solid #eee'}}>
            <div><strong>{it.name ?? `Customer ${it.customerId}`}</strong></div>
            <div>Score: {String(it.score)}</div>
          </li>
        ))}
      </ol>
    </div>
  );
}
