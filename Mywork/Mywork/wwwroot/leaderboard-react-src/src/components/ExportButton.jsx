import React from 'react';
import { rowsToCsv } from '../api/leaderboardService';

export function ExportButton({ rows, pushMessage }) {
  function exportCsv() {
    if (!rows.length) { pushMessage('无数据可导出', 'warning'); return; }
    const csv = rowsToCsv(rows);
    const blob = new Blob(['\uFEFF' + csv], { type:'text/csv;charset=utf-8;' });
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = 'leaderboard.csv';
    document.body.appendChild(a); a.click(); a.remove();
  }
  return (
    <button className="btn btn-sm btn-outline-secondary mb-3" type="button" onClick={exportCsv}>
      导出当前区间 CSV
    </button>
  );
}