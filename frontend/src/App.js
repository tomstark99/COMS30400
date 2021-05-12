import React from "react";
import Unity, { UnityContent } from "react-unity-webgl";
import './App.css';

const unityContext = new UnityContent(
  'Build/webstef4.json',
  'Build/UnityLoader.js'
);

class App extends React.Component {
  // return (<Unity unityContext={unityContext} />);
  render() {
    return (
      <div id="falo">
        <Unity unityContent={unityContext} className={"falo"} />
      </div>
    )
  }
};

export default App;