import React from 'react';
import logo from './logo.svg';
import './App.css';
import Hello from './Hello';
import HelloMessage from './HelloMessage';
import LoginControl from './LoginControl';
import ApiCall from './ApiCall';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <Hello />
        <HelloMessage />
        <LoginControl />
        <ApiCall />
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
