import React from "react";
import Unity, { UnityContent } from "react-unity-webgl";
import './App.css';
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Link
} from "react-router-dom";
import { Navbar,Nav,NavDropdown,Form,FormControl,Button } from 'react-bootstrap'
import "react-bootstrap/dist/react-bootstrap.min.js"

const unityContext = new UnityContent(
  'Build/webstef4.json',
  'Build/UnityLoader.js'
);

class Game extends React.Component {
  render() {
    return (
      <div>
        <Unity unityContent={unityContext} className="falo" />
      </div>
    )
  }
}

class NavBarGame extends React.Component {
  render() {
    return(
      <Navbar id="navbar-custom" expand="lg">
        <Navbar.Brand href="/"><span className="nav-text">Freight</span></Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="mr-auto">
            <Nav.Link href="/"><span className="nav-text">Game</span></Nav.Link>
            <Nav.Link href="/about"><span className="nav-text">About</span></Nav.Link>
            <Nav.Link href="/pose"><span className="nav-text">Pose Controls</span></Nav.Link>
          </Nav>
        </Navbar.Collapse>
      </Navbar>
    )
  }
}

class App extends React.Component {
  // return (<Unity unityContext={unityContext} />);
  render() {
    return (
      <Router>
      <div>

        <NavBarGame/>

        {/* A <Switch> looks through its children <Route>s and
            renders the first one that matches the current URL. */}
        <Switch>
          <Route path="/about">
            <h1>about</h1>
          </Route>
          <Route path="/pose">
            <h1>pose</h1>
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