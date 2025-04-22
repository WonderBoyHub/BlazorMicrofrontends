import React, { useState } from 'react';
import { MicrofrontendApi } from 'blazor-microfrontends-react';
import './App.css';

function App() {
  const [count, setCount] = useState(0);
  
  return (
    <div className="container my-4">
      <h2>{MicrofrontendApi.getName()}</h2>
      
      <div className="card">
        <div className="card-header">
          <h3 className="card-title">Counter Example</h3>
        </div>
        <div className="card-body">
          <p>Current count: {count}</p>
          <button 
            className="btn btn-primary" 
            onClick={() => setCount(count + 1)}
          >
            Click me
          </button>
        </div>
      </div>
    </div>
  );
}

export default App; 