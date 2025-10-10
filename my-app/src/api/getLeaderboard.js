export async function getLeaderboard({ baseUrl = '', start, end, signal } = {}) {
  const params = new URLSearchParams();
  if (start != null) params.append('start', String(start));
  if (end != null) params.append('end', String(end));

  const url = `${baseUrl}/api/leaderboard${params.toString() ? `?${params.toString()}` : ''}`;

  const res = await fetch(url, { method: 'GET', headers: { Accept: 'application/json' }, signal });
  if (!res.ok) {
    const txt = await res.text().catch(() => '');
    throw new Error(`Leaderboard request failed: ${res.status} ${res.statusText} ${txt}`);
  }
  const data = await res.json();
  return data; // expected array of entries
}
