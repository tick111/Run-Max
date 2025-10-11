import React from 'react';

class ApiCall extends React.Component {
  state = {
    loading: true,
    error: '',
    rows: [],
    sort: 'rank',    // rank | score | user
    order: 'asc'     // asc | desc
  };

  componentDidMount(){
    this.load();
  }

  load(){
    const { sort, order } = this.state;
    const url = `/api/leaderboard?start=1&end=20&sort=${sort}&order=${order}`;
    console.log('Fetching:', url);
    this.setState({loading:true,error:''});
    fetch(url)
      .then(r => { if(!r.ok) throw new Error('HTTP '+r.status); return r.json(); })
      .then(data => {
        const arr = Array.isArray(data)?data:[data];
        this.setState({loading:false, rows:arr});
      })
      .catch(e => this.setState({loading:false,error:e.message}));
  }

  toggleOrder(){
    this.setState(prev => ({ order: prev.order === 'asc' ? 'desc' : 'asc' }), () => this.load());
  }
  changeSort(next){
    this.setState({ sort: next, order: next==='rank'?'asc':'desc' }, () => this.load());
  }

  render(){
    const { loading, error, rows, sort, order } = this.state;
    return (
      <div style={{maxWidth:680, margin:'0 auto'}}>
        <h3>排行榜</h3>
        <div style={{marginBottom:10, display:'flex', gap:8, flexWrap:'wrap'}}>
          <button onClick={()=>this.changeSort('rank')} disabled={sort==='rank'}>按排名</button>
          <button onClick={()=>this.changeSort('score')} disabled={sort==='score'}>按分数</button>
          <button onClick={()=>this.changeSort('user')} disabled={sort==='user'}>按用户名</button>
          <button onClick={()=>this.toggleOrder()}>
            切换 {order==='asc'?'升序→降序':'降序→升序'}
          </button>
        </div>
        {loading && <p>加载中...</p>}
        {error && <p style={{color:'tomato'}}>错误: {error}</p>}
        {!loading && !error && rows.length===0 && <p>无数据</p>}
        {!loading && !error && rows.length>0 && (
          <table style={{borderCollapse:'collapse', width:'100%'}}>
            <thead>
              <tr style={{background:'#1e3a8a',color:'#fff'}}>
                <th style={th}>Rank {sort==='rank' && arrow(order)}</th>
                <th style={th}>User {sort==='user' && arrow(order)}</th>
                <th style={th}>Score {sort==='score' && arrow(order)}</th>
              </tr>
            </thead>
            <tbody>
              {rows.map(r=>(
                <tr key={r.customerId} style={r.rank%2?rowAlt:row}>
                  <td style={td}>{r.rank ?? r.Rank}</td>
                  <td style={td}>{r.userName ?? r.UserName}</td>
                  <td style={td}>{r.score ?? r.Score}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    );
  }
}

const arrow = o => o==='asc'?'↑':'↓';
const th = {padding:'6px 10px',textAlign:'left'};
const td = {padding:'6px 10px',borderBottom:'1px solid #e2e8f0'};
const row = {};
const rowAlt = {background:'#f1f5f9'};

export default ApiCall;




