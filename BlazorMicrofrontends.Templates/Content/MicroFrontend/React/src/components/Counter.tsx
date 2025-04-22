import React, { useState } from 'react';
import ReactDOM from 'react-dom';

interface CounterProps {
  initialValue?: number;
}

const Counter: React.FC<CounterProps> = ({ initialValue = 0 }) => {
  const [count, setCount] = useState<number>(initialValue);
  
  return (
    <div className="card mb-4">
      <div className="card-header">
        <h3 className="card-title">Counter Component</h3>
      </div>
      <div className="card-body">
        <p>Current count: {count}</p>
        <button 
          className="btn btn-primary me-2" 
          onClick={() => setCount(count + 1)}
        >
          Increment
        </button>
        <button 
          className="btn btn-secondary" 
          onClick={() => setCount(count - 1)}
        >
          Decrement
        </button>
      </div>
    </div>
  );
};

// Define the Web Component
class CounterWebComponent extends HTMLElement {
  connectedCallback() {
    const mountPoint = document.createElement('div');
    this.attachShadow({ mode: 'open' }).appendChild(mountPoint);

    const initialValue = parseInt(this.getAttribute('initial-value') || '0', 10);
    
    // Add bootstrap styles to shadow DOM
    const style = document.createElement('style');
    style.textContent = `
      @import url('https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css');
      
      :host {
        display: block;
        margin: 1rem 0;
      }
    `;
    this.shadowRoot?.appendChild(style);
    
    ReactDOM.render(<Counter initialValue={initialValue} />, mountPoint);
  }
}

// Register the custom element
if (!customElements.get('react-counter')) {
  customElements.define('react-counter', CounterWebComponent);
}

export default Counter; 