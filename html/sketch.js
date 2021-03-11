// ml5.js: Pose Estimation with PoseNet
// The Coding Train / Daniel Shiffman
// https://thecodingtrain.com/Courses/ml5-beginners-guide/7.1-posenet.html
// https://youtu.be/OIo-DIOkNVg
// https://editor.p5js.org/codingtrain/sketches/ULA97pJXR

var canvas

let video;
let poseNet;
let pose;
let skeleton;
let poseString = "";

let poseSet = false;
let pose1;
let pose2;
let pose3;

let brain;
let poseLabel = "Y";

function setup() {
  canvas = createCanvas(640, 480);
  canvas.position(0,600);
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
}

function brainLoaded() {
  console.log('pose classification ready!');
  classifyPose();
}

function classifyPose() {
  if (pose) {
      let inputs = [];
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
    brain.classify(inputs, gotResult);
  } else {
    setTimeout(classifyPose, 100);
  }
}

function gotResult(error, results) {

  if (results[0].confidence > 0.85) {
    poseLabel = results[0].label.toUpperCase();
  } else {
    poseLabel = 'N';
  }
  //console.log(results[0].confidence);
  classifyPose();
}


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
}


function modelLoaded() {
  console.log('poseNet ready');
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

function getGestureAsString() {
  return poseLabel;
}

function draw() {
  push();
  translate(video.width, 0);
  scale(-1, 1);
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

  fill(255, 0, 255);
  noStroke();
  textSize(512);
  textAlign(CENTER, CENTER);
  text(poseLabel, width / 2, height / 2);
}
