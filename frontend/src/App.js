import React from "react";
import Unity, { UnityContent } from "react-unity-webgl";
import './App.css';
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Link
} from "react-router-dom";

const unityContext = new UnityContent(
  'Build/webstef4.json',
  'Build/UnityLoader.js'
);

class Game extends React.Component {
  render() {
    return (
      <div>
        <h1 id="title">Freight</h1>
        <Unity unityContent={unityContext} className="falo" />
      </div>
    )
  }
}

class App extends React.Component {
  // return (<Unity unityContext={unityContext} />);
  render() {
    return (
      <Router>
      <div>
        <nav>
          <ul>
            <li>
              <Link to="/">Home</Link>
            </li>
            <li>
              <Link to="/about">About</Link>
            </li>
            <li>
              <Link to="/users">Users</Link>
            </li>
          </ul>
        </nav>

        {/* A <Switch> looks through its children <Route>s and
            renders the first one that matches the current URL. */}
        <Switch>
          <Route path="/about">
            <h1>about</h1>
          </Route>
          <Route path="/users">
            <h1>users</h1>
          </Route>
          <Route path="/">
            <Game />
          </Route>
        </Switch>
      </div>
    </Router>
    )
  }
};

export default App;