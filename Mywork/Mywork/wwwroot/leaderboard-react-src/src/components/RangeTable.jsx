import React from 'react';
import { updateScore } from '../api/leaderboardService';

export function RangeTable({ rows, refresh, pushMessage }) {
  async function adjust(customerId, delta) {
    try {
      await updateScore(customerId, delta);
      pushMessage(`已为 ${customerId} 调整 ${delta}`, 'success');
      refresh();
    } catch (e) {
      pushMessage(e.message, 'danger');
    }
  }

  return (
    <div className="table-responsive mb-3">
      <table className="table table-striped table-sm">
        <thead className="table-light">
          <tr>
            <th>Rank</th>
            <th>CustomerId</th>
            <th>Score</th>
            <th style={{ width: 180 }}>操作</th>
          </tr>
        </thead>
        <tbody>
          {rows.map(r => (
            <tr key={r.customerId}>
              <td>{r.rank}</td>
              <td>{r.customerId}</td>
              <td>{r.score}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary"
                  onClick={() => adjust(r.customerId, 10)}
                >+10</button>
                <button
                  className="btn btn-sm btn-outline-danger ms-1"
                  onClick={() => adjust(r.customerId, -10)}
                >-10</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}