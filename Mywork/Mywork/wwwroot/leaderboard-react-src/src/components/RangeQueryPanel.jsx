import React from 'react';
export function RangeQueryPanel({ start, end, setStart, setEnd, auto, toggleAuto, query }) {
  return (
    <div className="card shadow-sm h-100">
      <div className="card-header">按排名区间</div>
      <div className="card-body">
        <div className="row g-2">
          <div className="col-6">
            <label className="form-label">Start</label>
            <input type="number" className="form-control" value={start} onChange={e => setStart(e.target.value)} />
          </div>
          <div className="col-6">
            <label className="form-label">End</label>
            <input type="number" className="form-control" value={end} onChange={e => setEnd(e.target.value)} />
          </div>
        </div>
        <div className="d-flex gap-2 mt-3">
          <button className="btn btn-success flex-fill" type="button" onClick={query}>查询</button>
          <button className="btn btn-outline-secondary flex-fill" type="button" onClick={toggleAuto}>
            自动刷新: {auto ? '开' : '关'}
          </button>
        </div>
        <div className="form-text mt-2">自动刷新间隔 5 秒。</div>
      </div>
    </div>
  );
}