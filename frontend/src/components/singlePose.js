import { Navbar, Nav } from 'react-bootstrap'
import './singlepose.css';

const SinglePose = ({picture, pose_title}) => {
    return(
        <div className="single_pose_div">
            <Navbar className={"navbar-change"} expand="lg">
                <h1 className="pose_title_text">{pose_title}</h1>
            </Navbar>
            <img className="gif-image" src={picture + ".gif"} alt={"falo"}/>
        </div>
    )
}

export default SinglePose