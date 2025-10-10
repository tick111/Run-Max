import logo from './logo.svg';
import './App.css';
import Hello from './HelloWorld';
import HelloMessage from './HelloMessage';
import LoginControl from './LoginControl';
import LeaderboardList from './components/LeaderboardList';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
           <Hello/>
           <HelloMessage/>
           <LoginControl></LoginControl>
          <LeaderboardList baseUrl="http://localhost:7006" start={1} end={20} />
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}

export default App;
