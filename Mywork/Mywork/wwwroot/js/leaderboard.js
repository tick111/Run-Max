(() => {
    const msgBox = document.getElementById('msg');
    let autoTimer = null;

    function showMessage(text, kind = 'success') {
        if (!text) { msgBox.className = 'alert d-none'; return; }
        msgBox.textContent = text;
        msgBox.className = 'alert alert-' + kind;
    }

    async function apiFetch(url, options = {}) {
        try {
            const res = await fetch(url, options);
            if (!res.ok) {
                const txt = await res.text();
                throw new Error(`${res.status} ${res.statusText} - ${txt}`);
            }
            const ct = res.headers.get('content-type') || '';
            if (ct.includes('application/json')) return await res.json();
            return await res.text();
        } catch (e) {
            showMessage(e.message, 'danger');
            throw e;
        }
    }

    // 更新分数
    document.getElementById('btnUpdate').addEventListener('click', async () => {
        const id = document.getElementById('updCustomerId').value.trim();
        const delta = document.getElementById('updDelta').value.trim();
        if (!id || !delta) return showMessage('请填写 CustomerId 与 Delta', 'warning');
        showMessage('提交中...', 'info');
        const data = await apiFetch(`/api/customer/${id}/score/${delta}`, { method: 'POST' });
        showMessage(`更新成功：${data.customerId} 当前总分 = ${data.score}`, 'success');
        if (autoTimer) loadRange();
    });

    // 区间查询
    document.getElementById('btnRange').addEventListener('click', () => loadRange());

    async function loadRange() {
        const s = document.getElementById('rangeStart').value;
        const e = document.getElementById('rangeEnd').value;
        let url = '/api/leaderboard';
        if (s && e) url += `?start=${s}&end=${e}`;
        showMessage('获取区间中...', 'info');
        const data = await apiFetch(url);
        fillRangeTable(data);
        showMessage(`区间返回 ${data.length} 条`, 'success');
    }

    // 邻居查询
    document.getElementById('btnNeighbors').addEventListener('click', async () => {
        const id = document.getElementById('nbCustomerId').value.trim();
        const h = document.getElementById('nbHigh').value || 0;
        const l = document.getElementById('nbLow').value || 0;
        if (!id) return showMessage('请输入 CustomerId', 'warning');
        showMessage('获取邻居中...', 'info');
        try {
            const data = await apiFetch(`/api/leaderboard/${id}?high=${h}&low=${l}`);
            fillNeighborsTable(data);
            showMessage(`邻居返回 ${data.length} 条`, 'success');
        } catch {
            // 已在 apiFetch 处理错误显示
        }
    });

    // 自动刷新
    document.getElementById('btnToggleAuto').addEventListener('click', (ev) => {
        if (autoTimer) {
            clearInterval(autoTimer);
            autoTimer = null;
            ev.currentTarget.textContent = '自动刷新: 关';
            showMessage('已关闭自动刷新', 'info');
        } else {
            loadRange();
            autoTimer = setInterval(loadRange, 5000);
            ev.currentTarget.textContent = '自动刷新: 开';
            showMessage('已开启自动刷新(5s)', 'info');
        }
    });

    document.getElementById('btnExport')?.addEventListener('click', () => {
        const rows = Array.from(document.querySelectorAll('#tblRange tbody tr'))
            .map(tr => Array.from(tr.children).slice(0,3).map(td => td.textContent.trim()));
        if (rows.length === 0) { showMessage('无数据可导出', 'warning'); return; }
        let csv = 'Rank,CustomerId,Score\r\n' + rows.map(r => r.join(',')).join('\r\n');
        const blob = new Blob([csv], { type:'text/csv;charset=utf-8;' });
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = 'leaderboard.csv';
        document.body.appendChild(a);
        a.click();
        a.remove();
    });

    function fillRangeTable(data) {
        const tbody = document.querySelector('#tblRange tbody');
        tbody.innerHTML = '';
        data.forEach(r => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${r.rank}</td>
                <td>${r.customerId}</td>
                <td>${r.score}</td>
                <td>
                    <button class="btn btn-sm btn-outline-primary" data-delta="10" data-id="${r.customerId}">+10</button>
                    <button class="btn btn-sm btn-outline-danger ms-1" data-delta="-10" data-id="${r.customerId}">-10</button>
                </td>`;
            tbody.appendChild(tr);
        });
        tbody.querySelectorAll('button[data-delta]').forEach(btn => {
            btn.addEventListener('click', async () => {
                const cid = btn.dataset.id;
                const d = btn.dataset.delta;
                await apiFetch(`/api/customer/${cid}/score/${d}`, { method: 'POST' });
                showMessage(`已为 ${cid} 调整 ${d}`, 'success');
                loadRange();
            });
        });
    }

    function fillNeighborsTable(data) {
        const tbody = document.querySelector('#tblNeighbors tbody');
        tbody.innerHTML = '';
        data.forEach(r => {
            const tr = document.createElement('tr');
            tr.innerHTML = `<td>${r.rank}</td><td>${r.customerId}</td><td>${r.score}</td>`;
            tbody.appendChild(tr);
        });
    }

    // 初始加载（默认 1~50）
    loadRange().catch(() => { });
})();