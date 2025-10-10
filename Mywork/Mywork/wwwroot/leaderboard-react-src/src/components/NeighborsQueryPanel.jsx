import React, { useState } from 'react';
import { getNeighbors } from '../api/leaderboardService';

export function NeighborsQueryPanel({ setRows, pushMessage }) {
  const [cid, setCid] = useState('');
  const [high, setHigh] = useState(1);
  const [low, setLow] = useState(1);

  async function submit() {
    if (!cid) { pushMessage('请输入 CustomerId', 'warning'); return; }
    try {
      pushMessage('获取邻居中...', 'info');
      const data = await getNeighbors(cid, high, low);
      setRows(data);
      pushMessage(`邻居返回 ${data.length} 条`, 'success');
    } catch (e) {
      pushMessage(e.message, 'danger');
    }
  }

  return (
    <div className="card shadow-sm h-100">
      <div className="card-header">邻居查询</div>
      <div className="card-body">
        <label className="form-label">Customer ID</label>
        <input type="number" className="form-control mb-2" value={cid} onChange={e => setCid(e.target.value)} />
        <label className="form-label">High (上方)</label>
        <input type="number" className="form-control mb-2" value={high} onChange={e => setHigh(e.target.value)} />
        <label className="form-label">Low (下方)</label>
        <input type="number" className="form-control mb-3" value={low} onChange={e => setLow(e.target.value)} />
        <button className="btn btn-info w-100 text-white" type="button" onClick={submit}>查询邻居</button>
      </div>
    </div>
  );
}