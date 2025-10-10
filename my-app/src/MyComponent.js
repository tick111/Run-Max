import React from "react";
class ChildComponent extends React.Component {
  focusInput = () => {
    this.inputRef.current.focus();
  };

  constructor(props) {
    super(props);
    this.inputRef = React.createRef();
  }

  render() {
    return <input type="text" ref={this.inputRef} />;
  }
}

class ParentComponent extends React.Component {
  constructor(props) {
    super(props);
    this.childRef = React.createRef();
  }

  handleClick = () => {
    this.childRef.current.focusInput();
  };

  render() {
    return (
      <div>
        <ChildComponent ref={this.childRef} />
        <button onClick={this.handleClick}>Focus Child Input</button>
      </div>
    );
  }
}

{/*const root = ReactDOM.createRoot(document.getElementById("root"));
// 渲染 MyComponent 组件
root.render(<MyComponent />);*/}
export default ParentComponent;