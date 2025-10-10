const headers = { 'Accept': 'application/json' };

async function http(url, options = {}) {
  const res = await fetch(url, { headers, ...options });
  const text = await res.text();
  if (!res.ok) {
    console.error('API raw error:', text);
    throw new Error(`${res.status} ${res.statusText}${text ? ' - ' + text : ''}`);
  }
  const ct = res.headers.get('content-type') || '';
  return ct.includes('application/json') ? JSON.parse(text) : text;
}

export const updateScore = (customerId, delta) =>
  http(`/api/customer/${customerId}/score/${delta}`, { method: 'POST' });

export const getRange = (start, end) => {
  let url = '/api/leaderboard';
  if (start && end) url += `?start=${start}&end=${end}`;
  return http(url);
};

export const getNeighbors = (customerId, high = 0, low = 0) =>
  http(`/api/leaderboard/${customerId}?high=${high}&low=${low}`);

export function rowsToCsv(rows) {
  if (!rows?.length) return '';
  const header = 'Rank,CustomerId,Score';
  const body = rows.map(r => `${r.rank},${r.customerId},${r.score}`).join('\r\n');
  return header + '\r\n' + body;
}
