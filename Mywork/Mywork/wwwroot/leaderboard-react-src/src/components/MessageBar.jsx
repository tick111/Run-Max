import React from 'react';
export function MessageBar({ msg }) {
  if (!msg) return null;
  return (
    <div className={`alert alert-${msg.kind || 'info'} py-2 my-2`} role="alert" style={{ fontSize: 14 }}>
      {msg.text}
    </div>
  );
}