import React, { useCallback, useEffect, useState } from 'react';
import { getRange } from '../api/leaderboardService';
import { UpdateScoreForm } from '../components/UpdateScoreForm';
import { RangeQueryPanel } from '../components/RangeQueryPanel';
import { RangeTable } from '../components/RangeTable';
import { NeighborsQueryPanel } from '../components/NeighborsQueryPanel';
import { NeighborsTable } from '../components/NeighborsTable';
import { ExportButton } from '../components/ExportButton';
import { MessageBar } from '../components/MessageBar';

export function LeaderboardDashboard() {
  const [msg, setMsg] = useState(null);
  const [rangeRows, setRangeRows] = useState([]);
  const [neighborRows, setNeighborRows] = useState([]);
  const [start, setStart] = useState(1);
  const [end, setEnd] = useState(10);
  const [auto, setAuto] = useState(false);

  const pushMessage = (text, kind='info') => setMsg({ text, kind });

  const loadRange = useCallback(async () => {
    try {
      pushMessage('获取区间中...', 'info');
      const data = await getRange(start, end);
      setRangeRows(data);
      pushMessage(`区间返回 ${data.length} 条`, 'success');
    } catch (e) {
      pushMessage(e.message, 'danger');
    }
  }, [start, end]);

  useEffect(() => { loadRange(); }, [loadRange]);

  useEffect(() => {
    if (!auto) return;
    const id = setInterval(loadRange, 5000);
    return () => clearInterval(id);
  }, [auto, loadRange]);

  function toggleAuto() {
    setAuto(a => !a);
    pushMessage(!auto ? '已开启自动刷新(5s)' : '已关闭自动刷新', 'info');
  }

  return (
    <div className="container-fluid">
      <h2 className="mt-3 mb-3">排行榜可视化 (React)</h2>

      <div className="row g-4">
        <div className="col-lg-4">
          <UpdateScoreForm onUpdated={loadRange} autoRefresh={auto} pushMessage={pushMessage} />
        </div>
        <div className="col-lg-4">
          <RangeQueryPanel
            start={start} end={end}
            setStart={setStart} setEnd={setEnd}
            auto={auto} toggleAuto={toggleAuto}
            query={loadRange}
          />
        </div>
        <div className="col-lg-4">
          <NeighborsQueryPanel setRows={setNeighborRows} pushMessage={pushMessage} />
        </div>
      </div>

      <hr className="my-4" />
      <MessageBar msg={msg} />

      <h4 className="mt-3">区间结果</h4>
      <RangeTable rows={rangeRows} refresh={loadRange} pushMessage={pushMessage} />
      <ExportButton rows={rangeRows} pushMessage={pushMessage} />

      <h4>邻居结果</h4>
      <NeighborsTable rows={neighborRows} />
    </div>
  );
}