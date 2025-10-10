(function () {
    const startInput = document.getElementById('startRank');
    const endInput = document.getElementById('endRank');
    const loadBtn = document.getElementById('loadBtn');
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const body = document.getElementById('lbBody');
    const statusText = document.getElementById('statusText');
    const overlay = document.getElementById('loadingOverlay');

    let lastBlockSize = 50;

    function showLoading(show) {
        overlay.classList.toggle('hidden', !show);
    }

    function setStatus(msg, isError) {
        statusText.textContent = msg || '';
        statusText.style.color = isError ? '#b91c1c' : '#555';
    }

    function parseRange() {
        const s = parseInt(startInput.value, 10);
        const e = parseInt(endInput.value, 10);
        if (isNaN(s) || s < 1) throw new Error('Start rank must be >= 1.');
        if (isNaN(e) || e < s) throw new Error('End rank must be >= start.');
        return { s, e };
    }

    async function loadRange() {
        let range;
        try {
            range = parseRange();
        } catch (err) {
            setStatus(err.message, true);
            return;
        }
        showLoading(true);
        setStatus('Loading...');
        try {
            const url = `/api/leaderboard?start=${range.s}&end=${range.e}`;
            const res = await fetch(url, { cache: 'no-store' });
            if (!res.ok) throw new Error(`Request failed (${res.status})`);
            const data = await res.json();
            renderRows(data);
            lastBlockSize = range.e - range.s + 1;
            setStatus(`Loaded ranks ${range.s} - ${range.e} (${data.length} rows).`);
        } catch (err) {
            console.error(err);
            setStatus(err.message, true);
            renderRows([]);
        } finally {
            showLoading(false);
        }
    }

    function renderRows(rows) {
        body.innerHTML = '';
        if (!rows || rows.length === 0) {
            body.innerHTML = `<tr class="empty-row"><td colspan="3">No data.</td></tr>`;
            return;
        }
        const frag = document.createDocumentFragment();
        for (const r of rows) {
            const tr = document.createElement('tr');
            if (r.rank === 1) tr.classList.add('top-1');
            else if (r.rank === 2) tr.classList.add('top-2');
            else if (r.rank === 3) tr.classList.add('top-3');

            tr.innerHTML = `
                <td>${r.rank}</td>
                <td>${r.customerId}</td>
                <td>${formatScore(r.score)}</td>
            `;
            frag.appendChild(tr);
        }
        body.appendChild(frag);
    }

    function formatScore(score) {
        if (typeof score === 'number') return score.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 4 });
        // if backend sends as string
        const n = Number(score);
        return isNaN(n) ? score : n.toLocaleString(undefined, { maximumFractionDigits: 4 });
    }

    loadBtn?.addEventListener('click', () => loadRange());
    prevBtn?.addEventListener('click', () => {
        try {
            const { s } = parseRange();
            const newEnd = s - 1;
            const newStart = Math.max(1, newEnd - lastBlockSize + 1);
            if (newEnd < 1) return;
            startInput.value = String(newStart);
            endInput.value = String(newEnd);
            loadRange();
        } catch { /* ignore */ }
    });
    nextBtn?.addEventListener('click', () => {
        try {
            const { e } = parseRange();
            const newStart = e + 1;
            const newEnd = newStart + lastBlockSize - 1;
            startInput.value = String(newStart);
            endInput.value = String(newEnd);
            loadRange();
        } catch { /* ignore */ }
    });

    // Auto-load initial
    loadRange();
})();