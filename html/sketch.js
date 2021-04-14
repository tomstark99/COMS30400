// ml5.js: Pose Estimation with PoseNet
// The Coding Train / Daniel Shiffman
// https://thecodingtrain.com/Courses/ml5-beginners-guide/7.1-posenet.html
// https://youtu.be/OIo-DIOkNVg
// https://editor.p5js.org/codingtrain/sketches/ULA97pJXR

var canvas

let textSiz = 20;

let video;

let poseNet;
let pose;
let skeleton;
let poseString = "320.00,240.00,0.7193217277526855,308.457553558647,150.69552653946604,0.37067362666130066,220.29520209537378,134.97983638770862,0.763607919216156,343.6068779125548,134.4750381677936,0.5079049468040466,207.22150717097648,125.17828928332113,0.06637368351221085,382.8497965851722,250.36627399526373,0.6798007488250732,126.03583025421082,267.8783894142909,0.9432628154754639,418.1560519331845,437.300819709287,0.891068696975708,63.42853984869944,443.89335647196333,0.7256055474281311,366.4546129252711,529.1151282475938,0.010084007866680622,96.36494359542519,523.6664154469154,0.0019054475706070662,345.1760453787463,552.2994138483415,0.024226510897278786,180.76492956507275,549.035212050172,0.003815719624981284,340.7119255735163,540.3847514072589,0.010884284973144531,178.85120823137012,550.4224786888553,0.00705913407728076,339.19948028542143,560.7587062545687,0.029223235324025154,175.99135817142954,557.0438257109585,0.0046616848558187485,";

let poseSet = false;
let pose1;
let pose2;
let pose3;

let poseLabel = "Y";
let poseSentence = "";
let poseLag = 0;

let overlay;

function setup() {
  // init canvas
  canvas = createCanvas(320, 240);
  canvas.parent('unityContainer');
  canvas.position(0, 0);

  // init video
  video = createCapture(VIDEO);
  video.hide();

  // init posenet
  var optionsPose = {
    architecture: 'ResNet50',
    imageScaleFactor: 0.5,
    outputStride: 16,
    // flipHorizontal: false,
    minConfidence: 0.2,
    maxPoseDetections: 1,
    scoreThreshold: 0.2,
    // nmsRadius: 20,
    // detectionType: 'single',
    inputResolution: 161,
    // multiplier: 1,
    quantBytes: 1,
  };

  poseNet = ml5.poseNet(video, optionsPose, modelLoaded);
  poseNet.on('pose', gotPoses);

  // init overlay
  overlay = loadImage('overlays/Neutral.png');
}

function modelLoaded() {
  console.log('poseNet ready');
}

// Checks for head position
// Gesture given based on what third of the window the nose is withing according to
// |N|N|N|
// |O|N|I|
// |Q|C|W|
function noseLabel(){
  // normalise nose position e.g. 0<x,y<1
  var normNosePos = createVector(pose1.nose.x/(2 * width), pose1.nose.y/(2 * height));
  if(normNosePos.x > 0 && normNosePos.x < 0.37 && normNosePos.y > 1/3 && normNosePos.y < 2/3){
    return 'I';
  }else if (normNosePos.x > 0.62 && normNosePos.x < 1 && normNosePos.y > 1/3 && normNosePos.y < 2/3) {
    return 'O';
  }else if (normNosePos.x > 0 && normNosePos.x < 1/3 && normNosePos.y > 2/3 && normNosePos.y < 1) {
    return 'W';
  } else if (normNosePos.x > 2/3 && normNosePos.x < 1 && normNosePos.y > 2/3 && normNosePos.y < 1) {
    return 'Q';
  }else if (normNosePos.x > 1/3 && normNosePos.x < 2/3 && normNosePos.y > 2/3 && normNosePos.y < 1) {
    return 'C';
  }else{
    return 'N';
  }
}

function handsLabel(){
  //if(pose3.leftWrist.confidence>0.7 && pose3.rightWrist.confidence>0.7){
    // normalise wrist positions e.g. 0<x,y<1
    var normLeftWristVector = createVector((pose3.leftWrist.x-pose1.leftWrist.x)/(2 * width), (pose3.leftWrist.y-pose1.leftWrist.y)/(2 * height));
    var normRightWristVector = createVector((pose3.rightWrist.x-pose1.rightWrist.x)/(2 * width), (pose3.rightWrist.y-pose1.rightWrist.y)/(2 * width));
    var normLeftWristPos = createVector(pose1.leftWrist.x/(2 * width), pose1.leftWrist.y/(2 * height));
    if((normLeftWristVector.y>0.1 && normRightWristVector.y<-0.1) || (normLeftWristVector.y<-0.1 && normRightWristVector.y>0.1)){
      // Ladder climb, hands moving in opposite directions
      poseLag = 12;
      return 'L';
    } else if ((normLeftWristVector.x>0.07 && normRightWristVector.x<-0.07)) {
      // Pull apart, both hands moving apart
      poseLag = 8;
      return 'P';
    }else if (normLeftWristVector.y>0.07 && normRightWristVector.y>0.07) {
      // Pull up, both hands moving down
      poseLag = 8;
      return "U";
  }else if (normLeftWristPos.x>2/3 && normLeftWristPos.x<1 && normLeftWristPos.y>0.2 && normLeftWristPos.y<0.8) {
      // Move forward, left hand up
      return "F";
    }else{
      return 'N';
    }
  //}
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


function check() {
  if (pose) {
    poseString = "";
    for (let i = 0; i < pose.keypoints.length; i++) {
      let x = pose.keypoints[i].position.x;
      let y = pose.keypoints[i].position.y;
      poseString = poseString.concat(x, ",", y, ",");
    }
  }
  return poseString;
}

// Called by Unity
// Returns current pose x's, y's and confidence's in a string
function getPoseAsString() {
  if (pose) {
    poseString = "";
    for (let i = 0; i < pose.keypoints.length; i++) {
      let x = pose.keypoints[i].position.x;
      let y = pose.keypoints[i].position.y;
      let confidence = pose.keypoints[i].score;
      poseString = poseString.concat(x, ",", y, ",", confidence, ",");
    }
  }
  return poseString.slice(0, -1);
}

// Called by Unity
// Retruns gesture label
function getGestureAsString() {
  return poseLabel;
}

// Called by Unity
// Loads overlay
function loadOverlay(path) {
  overlay = loadImage(path);
}

// Called by Unity
// Clears overlay
function clearOverlay() {
  // alert("Clear Overlay");
  overlay = loadImage('');
}

// Draws to  canvas
// Called once per frame
function draw() {
  push();
  translate(video.width/2, 0);
  scale(-1/2, 1/2);
  image(video, 0, 0, video.width, video.height);

  if (pose) {
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
      fill(0,255,0);
      ellipse(x,y,16,16);
    }

    for (let i = 0; i < skeleton.length; i++) {
      let a = skeleton[i][0];
      let b = skeleton[i][1];
      strokeWeight(2);
      stroke(255);
      line(a.position.x, a.position.y,b.position.x,b.position.y);
    }
  }
  pop();

  // |N|N|N|
  // |O|N|I|
  // |Q|C|W|
  //
  // if (point.x > 1/3 && point.x < 2/3 && y > 2/3 && y < 1) {
  //   label = C;
  // }
  // ...
  // else {
  //   label = N;
  // }

  // fill(255, 0, 255);
  // noStroke();
  // textSize(256);
  // textAlign(CENTER, CENTER);
  // text(poseLabel, width / 2, height / 2);

  fill(255,255,255);
  rect(0, 0, canvas.width, textSiz);

  fill(0,0,0);
  textSize(textSiz);
  textAlign(LEFT, TOP);
  text(poseSentence, 0, 0);

  image(overlay, 0, 0, canvas.width, canvas.height);
}


// Move canvas around on mouse click
let onCanv = false;
let xOrigin = 0;
let yOrigin = 0;
let xCanvOrigin = 0;
let yCanvOrigin = 0;

// Returns true if mouse is within bounds of canvas
function mouseOnCanvas(){
  return mouseX > 0 && mouseX < width && mouseY > 0 && mouseY < height;
}

// Sets all necessary variables when canvas is clicked on
function mousePressed() {
  if (mouseOnCanvas()) {
    onCanv = true;
    xOrigin = winMouseX;// - bx;
    yOrigin = winMouseY;// - by;
    xCanvOrigin = canvas.position().x;
    yCanvOrigin = canvas.position().y;
  }
}

// Updates canvas position by amount the mouse has moved since clicked
function mouseDragged() {
  if (onCanv) {
    canvas.position(xCanvOrigin + (winMouseX - xOrigin), yCanvOrigin + (winMouseY - yOrigin))
  }
}


function mouseReleased() {
  onCanv = false;
}
