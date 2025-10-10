import React from 'react';
export function NeighborsTable({ rows }) {
  return (
    <div className="table-responsive">
      <table className="table table-bordered table-sm">
        <thead className="table-light">
          <tr><th>Rank</th><th>CustomerId</th><th>Score</th></tr>
        </thead>
        <tbody>
        {rows.map(r => (
          <tr key={r.customerId}>
            <td>{r.rank}</td><td>{r.customerId}</td><td>{r.score}</td>
          </tr>
        ))}
        </tbody>
      </table>
    </div>
  );
}