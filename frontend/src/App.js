import React, {useEffect} from "react";
import Unity, { UnityContent } from "react-unity-webgl";
import './App.css';
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Link
} from "react-router-dom";
import { Navbar,Nav } from 'react-bootstrap'
import "react-bootstrap/dist/react-bootstrap.min.js"
import useScript from './hooks/useScript';
import ReactPlayer from "react-player"
import SinglePose from './components/singlePose';

const unityContext = new UnityContent(
  'Build/webstef4.json',
  'Build/UnityLoader.js'
);

const Game = () => {
  useScript('sketch.js');
    return (
      <div id="game-div">
        <Unity unityContent={unityContext} className="falo" />
      </div>
    )
}

const NavBarGame = () => {
    return(
      <Navbar id="navbar-custom" expand="lg">
        <Navbar.Brand href="/"><span className="nav-text-title">Freight</span></Navbar.Brand>
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

const About = () => {
  return (
      <div>
          <ReactPlayer
              url="https://www.youtube.com/watch?v=dQw4w9WgXcQ"
          />
      <p className="abstract"> Freight is a hide `n' fight game in which you and your partner must break into a train station, avoid (or fight) guards, retrieve a high value treasure and escape without being caught!</p>
      <p className="abstract"> The game starts you and a partner around the outskirts of a train station, with the objective to find a way into the station itself. You and your friend must explore the edges of the train station, attempting to keep quiet and avoid the patrolling guards. As you both explore the edges you will come across a way to break in, a slightly broken fence that looks like you can pull it apart. A new objective pops up to both players, telling them to break open the fence so they can get inside of the station. Using the hinted pose the players can pull the fence open and complete the objective of `Breaking open the fence'.</p>
    </div>
  )
}

const Pose = () => {
  return(
    <div>
      <SinglePose picture={"move_forward"} pose_title={"Move Forward"}/>
      <SinglePose picture={"move_left"} pose_title={"Move Left"}/>
      <SinglePose picture={"move_right"} pose_title={"Move Right"}/>
      <SinglePose picture={"crouch"} pose_title={"Crouch"}/>
      <SinglePose picture={"pull_apart"} pose_title={"Pull Apart"}/>
      <SinglePose picture={"open_door"} pose_title={"Open Door"}/>
      <SinglePose picture={"close_door"} pose_title={"Close Door"}/>
      <SinglePose picture={"pick_up"} pose_title={"Pick Up"}/>
      <SinglePose picture={"throw"} pose_title={"Throw"}/>
      <SinglePose picture={"ladder_climb"} pose_title={"Ladder Climb"}/>
    </div>
    
  )
} 

function App() {
    

    return (
      <Router>
      <div>

        <NavBarGame/>

        {/* A <Switch> looks through its children <Route>s and
            renders the first one that matches the current URL. */}
        <Switch>
          <Route path="/about">
            <h1 className="title">About</h1>
            <About />
          </Route>
          <Route path="/pose">
            <Pose />
          </Route>
          <Route path="/">
            <Game />
          </Route>
        </Switch>
      </div>
    </Router>
    )
};

export default App;