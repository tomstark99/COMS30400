// ml5.js: Pose Estimation with PoseNet
// The Coding Train / Daniel Shiffman
// https://thecodingtrain.com/Courses/ml5-beginners-guide/7.1-posenet.html
// https://youtu.be/OIo-DIOkNVg
// https://editor.p5js.org/codingtrain/sketches/ULA97pJXR


var canvas

let textSiz = 20;

let video;

let poseNet;
let skeleton;

let poseSet = false;
let pose1;
let pose2;
let pose3;

let poseSentence = "";
let poseLag = 0;

function setup() {
    // init canvas
    canvas = createCanvas(320, 240);
    canvas.parent('root');
    canvas.position(1500, 82);

    // init video
    video = createCapture(VIDEO);
    video.hide();

    // init posenet
    var optionsPose = {
        architecture: 'MobileNetV1',
        imageScaleFactor: 1,
        outputStride: 16,
        // flipHorizontal: false,
        minConfidence: 0.2,
        maxPoseDetections: 1,
        scoreThreshold: 0.2,
        nmsRadius: 1,
        // detectionType: 'single',
        inputResolution: 289,
        multiplier: 1,
        quantBytes: 4,
    };

    poseNet = ml5.poseNet(video, optionsPose, modelLoaded);
    poseNet.on('pose', gotPoses);

    // init overlay
    overlay = loadImage('overlays/new.png');
}

function modelLoaded() {
    console.log('poseNet ready');
    //posenet.singlePose(video);
}

// Checks for head position
// Gesture given based on what third of the window the nose is withing according to
// |N|N|N|
// |O|N|I|
// |Q|C|W|
function noseLabel() {
    // normalise nose position e.g. 0<x,y<1
    var normNosePos = createVector(pose1.nose.x / (2 * width), pose1.nose.y / (2 * height));
    if (normNosePos.x > 1 / 3 && normNosePos.x < 2 / 3 && normNosePos.y > 0.2 && normNosePos.y < 0.4) {
        return 'F';
    } else if (normNosePos.x > 0 && normNosePos.x < 0.37 && normNosePos.y > 1 / 3 && normNosePos.y < 2 / 3) {
        return 'I';
    } else if (normNosePos.x > 0.62 && normNosePos.x < 1 && normNosePos.y > 1 / 3 && normNosePos.y < 2 / 3) {
        return 'O';
    } else if (normNosePos.x > 0 && normNosePos.x < 1 / 3 && normNosePos.y > 2 / 3 && normNosePos.y < 1) {
        return 'W';
    } else if (normNosePos.x > 2 / 3 && normNosePos.x < 1 && normNosePos.y > 2 / 3 && normNosePos.y < 1) {
        return 'Q';
    } else if (normNosePos.x > 1 / 3 && normNosePos.x < 2 / 3 && normNosePos.y > 2 / 3 && normNosePos.y < 1) {
        return 'C';
    } else {
        return 'N';
    }
}

function handsLabel() {
    if (pose1.leftWrist.x < width * 2 && pose1.leftWrist.x > 0 && pose1.leftWrist.y < height * 2 && pose1.leftWrist.y > 0) {
        // normalise wrist positions e.g. 0<x,y<1
        var normLeftWristVector = createVector((pose3.leftWrist.x - pose1.leftWrist.x) / (2 * width), (pose3.leftWrist.y - pose1.leftWrist.y) / (2 * height));
        var normRightWristVector = createVector((pose3.rightWrist.x - pose1.rightWrist.x) / (2 * width), (pose3.rightWrist.y - pose1.rightWrist.y) / (2 * width));
        var normLeftWristPos = createVector(pose1.leftWrist.x / (2 * width), pose1.leftWrist.y / (2 * height));
        if (normLeftWristVector.x < -0.1) {
            // Pick up, left hand moving left to right
            poseLag = 3;
            return "B";
        } else if ((normLeftWristVector.x > 0.07 && normRightWristVector.x < -0.07)) {
            // Pull apart, both hands moving apart
            poseLag = 4;
            return 'P';
        } else if (normLeftWristVector.y > 0.07 && normRightWristVector.y > 0.07) {
            // Close door pose, both hands moving down
            poseLag = 4;
            return "U";
        } else if (normLeftWristVector.y < -0.07 && normRightWristVector.y < -0.07) {
            // Open door pose, both hands moving up
            poseLag = 8;
            return "D";
        } else if (normLeftWristVector.y > 0.4) {
            // Throw rock pose
            poseLag = 4;
            return "R";
        } else if ((normLeftWristVector.y > 0.1 && normRightWristVector.y < -0.1) || (normLeftWristVector.y < -0.1 && normRightWristVector.y > 0.1)) {
            // Ladder climb, hands moving in opposite directions
            poseLag = 12;
            return 'L';
        } else {
            return 'N';
        }
    }
    return 'N';
}

// Called by gotPoses
// Only updates pose when confidence is high enough and
// previous pose lag has finished
function gotResult() {

    poseLag--;

    var tempPoseLabel = poseLabel;

    if (poseLag < 0) {
        tempPoseLabel = noseLabel();
        if (tempPoseLabel == 'N') {
            tempPoseLabel = handsLabel();
        }
    }

    // Change displayed pose phrase
    if (tempPoseLabel !== poseLabel) {
        poseLabel = tempPoseLabel;
        // console.log("change");
        switch (tempPoseLabel) {
            case 'N':
                poseSentence = "No Action";
                break;
            case 'U':
                poseSentence = "Pull Up";
                break;
            case 'D':
                 poseSentence = "Open Door";
                 break;
            case 'B':
                poseSentence = "Pick Up";
                break;
            case 'L':
                poseSentence = "Ladder Climb";
                break;
            case 'P':
                poseSentence = "Pull Apart";
                break;
            case 'I':
                poseSentence = "Lean Right";
                break;
            case 'O':
                poseSentence = "Lean Left";
                break;
            case 'W':
                poseSentence = "Lie Right";
                break;
            case 'Q':
                poseSentence = "Lie Left";
                break;
            case 'C':
                poseSentence = "Crouch";
                break;
            case 'F':
                poseSentence = "Move Forward";
                break;
            case "R":
                poseSentence = "Throw";
                break;
            default:
                poseSentence = "";
        }
    }
}

// Callback from poseNet
// Populates all 3 pose variabless and calls pose classification
function gotPoses(poses) {
    if (poses.length > 0) {
        pose = poses[0].pose;
        skeleton = poses[0].skeleton;

        if (!poseSet) {
            pose1 = pose;
            pose2 = pose;
            pose3 = pose;
            poseSet = true;
        } else {
            pose1 = pose2;
            pose2 = pose3;
            pose3 = pose;
        }
    }
    gotResult();
}

// Called by Unity
// Loads overlay
function loadOverlay(path) {
    if (!poseOff) {
        overlay = loadImage(path);
    }
}

// Called by Unity
// Clears overlay
function clearOverlay() {
    if (!poseOff) {
        // alert("Clear Overlay");
        overlay = loadImage('');
    }
}

function turnOffPose() {
    if (!poseOff) {
        poseOff = true;
        remove();
    }
}

let n = 30;

// Draws to  canvas
// Called once per frame
function draw() {
    if (canvas) {
        push();
        translate(video.width / 2, 0);
        scale(-1 / 2, 1 / 2);
        image(video, 0, 0, video.width, video.height);

        if (pose) {
            console.log(pose.leftWrist);
            let eyeR = pose.rightEye;
            let eyeL = pose.leftEye;
            let d = dist(eyeR.x, eyeR.y, eyeL.x, eyeL.y);
            fill(255, 0, 0);
            ellipse(pose.nose.x, pose.nose.y, d);
            fill(0, 0, 255);
            ellipse(pose.rightWrist.x, pose.rightWrist.y, 32);
            ellipse(pose.leftWrist.x, pose.leftWrist.y, 32);

            for (let i = 0; i < pose.keypoints.length; i++) {
                let x = pose.keypoints[i].position.x;
                let y = pose.keypoints[i].position.y;
                fill(0, 255, 0);
                ellipse(x, y, 16, 16);
            }

            for (let i = 0; i < skeleton.length; i++) {
                let a = skeleton[i][0];
                let b = skeleton[i][1];
                strokeWeight(2);
                stroke(255);
                line(a.position.x, a.position.y, b.position.x, b.position.y);
            }
        }
        pop();

        fill(255, 255, 255);
        rect(0, 0, canvas.width, textSiz);

        fill(0, 0, 0);
        textSize(textSiz);
        textAlign(LEFT, TOP);
        text(poseSentence, 0, 0);

        image(overlay, 0, 0, canvas.width, canvas.height);
    }
}

// Move canvas around on mouse click
let onCanv = false;
let xOrigin = 0;
let yOrigin = 0;
let xCanvOrigin = 0;
let yCanvOrigin = 0;

// Returns true if mouse is within bounds of canvas
function mouseOnCanvas() {
    return mouseX > 0 && mouseX < width && mouseY > 0 && mouseY < height;
}

// Sets all necessary variables when canvas is clicked on
function mousePressed() {
    if (mouseOnCanvas() && canvas) {
        onCanv = true;
        xOrigin = winMouseX; // - bx;
        yOrigin = winMouseY; // - by;
        xCanvOrigin = canvas.position().x;
        yCanvOrigin = canvas.position().y;
    }
}

// Updates canvas position by amount the mouse has moved since clicked
function mouseDragged() {
    if (onCanv && canvas) {
        canvas.position(xCanvOrigin + (winMouseX - xOrigin), yCanvOrigin + (winMouseY - yOrigin))
    }
}

function mouseReleased() {
    onCanv = false;
}
