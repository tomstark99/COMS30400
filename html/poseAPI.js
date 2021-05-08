
let pose;
let poseString = "320.00,240.00,0.7193217277526855,308.457553558647,150.69552653946604,0.37067362666130066,220.29520209537378,134.97983638770862,0.763607919216156,343.6068779125548,134.4750381677936,0.5079049468040466,207.22150717097648,125.17828928332113,0.06637368351221085,382.8497965851722,250.36627399526373,0.6798007488250732,126.03583025421082,267.8783894142909,0.9432628154754639,418.1560519331845,437.300819709287,0.891068696975708,63.42853984869944,443.89335647196333,0.7256055474281311,366.4546129252711,529.1151282475938,0.010084007866680622,96.36494359542519,523.6664154469154,0.0019054475706070662,345.1760453787463,552.2994138483415,0.024226510897278786,180.76492956507275,549.035212050172,0.003815719624981284,340.7119255735163,540.3847514072589,0.010884284973144531,178.85120823137012,550.4224786888553,0.00705913407728076,339.19948028542143,560.7587062545687,0.029223235324025154,175.99135817142954,557.0438257109585,0.0046616848558187485,";

let poseOff = false;
let poseLabel = "Y";

let overlay;

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
  if (poseOff) {
    return 'E';
  }
  return poseLabel;
}
