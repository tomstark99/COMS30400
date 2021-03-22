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

let brain;
let poseLabel = "Y";
let poseSentence = ""
let poseBufferLength = 5;
let poseBuffer = [];

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
  poseNet = ml5.poseNet(video, modelLoaded);
  poseNet.on('pose', gotPoses);

  // init pose recognition
  let options = {
    inputs: 66,
    outputs: 8,
    task: 'classification',
    debug: true
  }
  brain = ml5.neuralNetwork(options);
  const modelInfo = {
    model: 'model/model.json',
    metadata: 'model/model_meta.json',
    weights: 'model/model.weights.bin',
  };
  brain.load(modelInfo, brainLoaded);

  // init overlay
  overlay = loadImage('overlays/Neutral.png');

  // init poseBuffer
  for (var i = 0; i < poseBufferLength; i++) {
    poseBuffer.push('N');
  }
}

function brainLoaded() {
  console.log('pose classification ready!');
  classifyPose();
}

function modelLoaded() {
  console.log('poseNet ready');
}

// Generates classification from previous 3 poses
// Called by gotPoses
function classifyPose() {
  if (pose) {
      let inputs = [];
      // Create input ignoreing hips and legs
      for (let i = 0; i < pose.keypoints.length - 6; i++) {
        let x1 = pose1.keypoints[i].position.x;
        let y1 = pose1.keypoints[i].position.y;
        let x2 = pose2.keypoints[i].position.x;
        let y2 = pose2.keypoints[i].position.y;
        let x3 = pose3.keypoints[i].position.x;
        let y3 = pose3.keypoints[i].position.y;
        inputs.push(x1);
        inputs.push(y1);
        inputs.push(x2);
        inputs.push(y2);
        inputs.push(x3);
        inputs.push(y3);
      }
    // Classify pose
    brain.classify(inputs, gotResult);
  }
  // else {
  //   setTimeout(classifyPose, 100);
  // }
}


// Returns the modal element of an array
// If there are multiple modes returns the earliest seen
// e.g. getMode([1,1,2,2,3]) => 1
function getMode(array) {
  var dictionary = {};

  for (var i = 0; i < array.length; i++) {
    var val = array[i];
    if (dictionary[val] == null) {
      dictionary[val] = 1;
    } else {
      dictionary[val] += 1;
    }
  }

  // console.log(dictionary);

  var highestCount = 0;
  var highestVal = '';

  for (var key in dictionary) {
    if (dictionary[key] > highestCount) {
      highestCount = dictionary[key];
      highestVal = key;
    }
  }

  // console.log(highestVal);
  return highestVal
}

// Callback from brain.classify
// Only updates pose when confidence is high enough and
// buffer is significantly full (over 50%)
function gotResult(error, results) {

  var buffPoseLabel;

  if (results[0].confidence > 0.85) {
    buffPoseLabel = results[0].label.toUpperCase();
  } else {
    buffPoseLabel = 'N';
  }
  //console.log(results[0].confidence);
  poseBuffer.push(buffPoseLabel);
  poseBuffer.shift();

  console.log(poseBuffer);
  var tempPoseLabel = getMode(poseBuffer);

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
      default:
        poseSentaence = "";
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
      pose2 = pose;width
      pose3 = pose;
      poseSet = true;
    } else {
      pose1 = pose2;
      pose2 = pose3;
      pose3 = pose;
    }
  }
  classifyPose();
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

function loadOverlay(path) {
  overlay = loadImage(path);
}

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
