import React from 'react';

export function HelloWorld() {
  return (
    <div className="hello-block">
      <h1 className="hello-title">Hello, World!</h1>
      <p className="hello-sub">
        Just a test <span className="cursor" aria-hidden="true">|</span>
      </p>
    </div>
  );
}

// 同时提供默认导出，兼容 import HelloWorld from './HelloWorld'
export default HelloWorld;