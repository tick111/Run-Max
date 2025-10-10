import React, { useState } from 'react';
import { updateScore } from '../api/leaderboardService';

export function UpdateScoreForm({ onUpdated, autoRefresh, pushMessage }) {
  const [cid, setCid] = useState('');
  const [delta, setDelta] = useState('');

  async function submit() {
    if (!cid || delta === '') {
      pushMessage('请填写 CustomerId 与 Delta', 'warning'); return;
    }
    try {
      pushMessage('提交中...', 'info');
      const res = await updateScore(cid, delta);
      pushMessage(`更新成功：${res.customerId} 当前总分=${res.score}`, 'success');
      if (autoRefresh) onUpdated && onUpdated();
    } catch (e) {
      pushMessage(e.message, 'danger');
    }
  }

  return (
    <div className="card shadow-sm h-100">
      <div className="card-header">更新 / 增减分</div>
      <div className="card-body">
        <label className="form-label">Customer ID</label>
        <input className="form-control mb-2" type="number" value={cid} onChange={e => setCid(e.target.value)} />
        <label className="form-label">Delta (+/-)</label>
        <input className="form-control mb-3" type="number" value={delta} onChange={e => setDelta(e.target.value)} />
        <button type="button" className="btn btn-primary w-100" onClick={submit}>提交</button>
        <div className="form-text mt-2">正数加分，负数减分；降到 ≤0 移除。</div>
      </div>
    </div>
  );
}