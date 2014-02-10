using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewDriving : MonoBehaviour {

    public AudioClip collisionSound;   

    //Fahrzeugsteuerung
    public WheelCollider flWheelCollider;
    public WheelCollider frWheelCollider;
    public WheelCollider rlWheelCollider;
    public WheelCollider rrWheelCollider;
    public float maxTorque = 150.0f;
    public float maxBrakeTorque = 500.0f;
    public float maxSteerAngle = 30.0f;
    public float maxSpeedSteerAngle = 10.0f;
    public float maxSpeed = 200.0f;
    public float maxBackwardSpeed = 40.0f;
    public float currentSpeed = 0.0f;
    private bool isBraking = false;
    //Reifenvisualisierung
    public Transform flWheel;
    public Transform frWheel;
    public Transform rlWheel;
    public Transform rrWheel;
    public int turboForce = 500000;
    public int explosionForce = 500000;
    //wenn die Raeder sich falsch herum drehen, dann diesen Parameter aktivieren.
    public bool inverseWheelTurning = false;
    private int  wheelTurningParameter = 1;
    //Gaenge

    public List<int> gearSpeed;
    private int currentGear =0;
    //Vollbremsung
    public float FullBrakeTorque = 5000.00f;
    public AudioClip brakeSound ;
    public bool groundEffectsOn = true;

    //Autosound
    public AudioClip motorSound;
    private AudioSource motorAudioSource;

    //Autosound
    public AudioClip motorSoundLow;
    private AudioSource motorAudioSourceLow;

    //Autosound
    public AudioClip motorSoundMid;
    private AudioSource motorAudioSourceMid;

    //Autosound
    public AudioClip turboSound;
    private AudioSource turboAudioSource;

    //geschwindigkeit

    private WheelFrictionCurve frForwardFriction = new WheelFrictionCurve();
    private WheelFrictionCurve flForwardFriction = new WheelFrictionCurve();
    private WheelFrictionCurve rrForwardFriction = new WheelFrictionCurve();
    private WheelFrictionCurve rlForwardFriction = new WheelFrictionCurve();

    private WheelFrictionCurve frSidewaysFriction = new WheelFrictionCurve();
    private WheelFrictionCurve flSidewaysFriction = new WheelFrictionCurve();
    private WheelFrictionCurve rrSidewaysFriction = new WheelFrictionCurve();
    private WheelFrictionCurve rlSidewaysFriction = new WheelFrictionCurve();

    private float oldForwardFriction  =0.00f;
    private float oldSidewaysFriction  =0.00f;
    private float brakingForwardFriction  =0.03f;
    private float brakingSidewaysFriction =0.03f; 
    private float brakingSidewaysFrictionBackward = 0.01f;
    private float stopForwardFriction  =1;
    private float stopSidewaysFriction  =1;
    private AudioSource brakeAudioSource ;
    private bool isPlayingSound =false;

    //Dust
    public Transform DustL;
    public Transform DustR;

    public float centerOfMassY = 0;

    //Skidmarks
    public Transform skidmark;
    private Vector3 lastSkidmarkPosR;
    private Vector3 lastSkidmarkPosL;

    void Awake () {
  
	    if (inverseWheelTurning){
		    wheelTurningParameter = -1;
	    }
	    else{
		    wheelTurningParameter = 1;
	    }
        
        rigidbody.centerOfMass = new Vector3(0, centerOfMassY, 0);        
	    
	    oldForwardFriction = frWheelCollider.forwardFriction.stiffness;
	    oldSidewaysFriction = frWheelCollider.sidewaysFriction.stiffness;

	    InitSound();   	
	    
    }

    void Start (){
        

        frForwardFriction = frWheelCollider.forwardFriction;
        flForwardFriction = flWheelCollider.forwardFriction;
        rrForwardFriction = rrWheelCollider.forwardFriction;
        rlForwardFriction = rlWheelCollider.forwardFriction;

        frSidewaysFriction = frWheelCollider.sidewaysFriction;
        flSidewaysFriction = flWheelCollider.sidewaysFriction;
        rrSidewaysFriction = rrWheelCollider.sidewaysFriction;
        rlSidewaysFriction = rlWheelCollider.sidewaysFriction;

        
    }

    void InitSound(){
	    
	    brakeAudioSource = gameObject.AddComponent("AudioSource") as AudioSource;
	    brakeAudioSource.clip = brakeSound;
	    brakeAudioSource.loop = true;
	    brakeAudioSource.volume = 0.7f;
        //brakeAudioSource.rolloffMode = AudioRolloffMode.Linear;
	    brakeAudioSource.playOnAwake = false;
        

        turboAudioSource = gameObject.AddComponent("AudioSource") as AudioSource;
	    turboAudioSource.clip = turboSound;
	    turboAudioSource.loop = false;
	    turboAudioSource.volume = 1.0f;
        //turboAudioSource.rolloffMode = AudioRolloffMode.Linear;
	    turboAudioSource.playOnAwake = false;

        motorAudioSource = gameObject.AddComponent("AudioSource") as AudioSource;
	    motorAudioSource.clip = motorSound;
	    motorAudioSource.loop = true;
	    motorAudioSource.volume = 0.5f;
	    motorAudioSource.playOnAwake = false;
	    motorAudioSource.pitch = 0.1f;
        //motorAudioSource.rolloffMode = AudioRolloffMode.Linear;
	    motorAudioSource.Play();

        motorAudioSourceLow = gameObject.AddComponent("AudioSource") as AudioSource;
        motorAudioSourceLow.clip = motorSoundLow;
        motorAudioSourceLow.loop = true;
        motorAudioSourceLow.volume = 0.4f;
        motorAudioSourceLow.playOnAwake = false;
        motorAudioSourceLow.pitch = 0.1f;
        //motorAudioSourceLow.rolloffMode = AudioRolloffMode.Linear;
        motorAudioSourceLow.Play();

        motorAudioSourceMid = gameObject.AddComponent("AudioSource") as AudioSource;
        motorAudioSourceMid.clip = motorSoundMid;
        motorAudioSourceMid.loop = true;
        motorAudioSourceMid.volume = 0.6f;
        motorAudioSourceMid.playOnAwake = false;
        motorAudioSourceMid.pitch = 0.1f;
        //motorAudioSourceMid.rolloffMode = AudioRolloffMode.Linear;
        motorAudioSourceMid.Play();

    }

    void FixedUpdate () {

	        currentSpeed = (Mathf.PI * 2 * flWheelCollider.radius) * flWheelCollider.rpm *60 /1000;
	        currentSpeed = Mathf.Round(currentSpeed);

	        if (((currentSpeed> 0) && (Input.GetAxis("Vertical") <0 )) || ((currentSpeed< 0) && (Input.GetAxis("Vertical") > 0 ))){
	  	        isBraking = true;
	        }
	        else {
		        isBraking = false;
        		
		        flWheelCollider.brakeTorque =0;
		        frWheelCollider.brakeTorque =0;     		
	        }
        	
	        if (isBraking ==false) {
		        if ((currentSpeed < maxSpeed) && (currentSpeed > (maxBackwardSpeed*-1))){
        			
			        flWheelCollider.motorTorque =  maxTorque * Input.GetAxis("Vertical");
			        frWheelCollider.motorTorque = maxTorque * Input.GetAxis("Vertical");        			
        			
		        }
		        else {
			        flWheelCollider.motorTorque =  0;
			        frWheelCollider.motorTorque =  0;
		        }
	        }
	        else {
		        flWheelCollider.brakeTorque = maxBrakeTorque;
		        frWheelCollider.brakeTorque = maxBrakeTorque;
		        flWheelCollider.motorTorque =  0;
		        frWheelCollider.motorTorque =  0;
	        }
        	
	        //je hoeher die Geschwindigkeit,  desto geringer der maximale Einschlagwinkel.
	        float speedProcent = currentSpeed / maxSpeed;
            speedProcent = Mathf.Clamp(speedProcent, 0, 1);
	        float speedControlledMaxSteerAngle;
	        speedControlledMaxSteerAngle = maxSteerAngle -((maxSteerAngle-maxSpeedSteerAngle)*speedProcent);
	        flWheelCollider.steerAngle = speedControlledMaxSteerAngle  * Input.GetAxis("Horizontal");
	        frWheelCollider.steerAngle = speedControlledMaxSteerAngle  *  Input.GetAxis("Horizontal");

            FullBraking();
        	
	        SetCurrentGear();
	        GearSound();	
	
    }

    void FullBraking (){
	    if(Input.GetKey("space")){	
    	
		    rlWheelCollider.brakeTorque =FullBrakeTorque;
		    rrWheelCollider.brakeTorque =FullBrakeTorque;

		    if ((Mathf.Abs(rigidbody.velocity.z)>1) || (Mathf.Abs(rigidbody.velocity.x)>1)){
    			
                SetFriction(brakingForwardFriction, brakingSidewaysFriction,brakingSidewaysFrictionBackward);
			    SetBrakeEffects(true);	
		    }
		    else{
			    SetFriction(stopForwardFriction,stopSidewaysFriction);	
			    SetBrakeEffects(false);		
		    }
    		
	    }
	    else{		
		    rlWheelCollider.brakeTorque =0;
		    rrWheelCollider.brakeTorque =0;
		    SetFriction(oldForwardFriction,oldSidewaysFriction);	
		    SetBrakeEffects(false);		
	    }	
    }
    	
    void SetFriction(float MyForwardFriction , float MySidewaysFriction)
    {      

        frForwardFriction.stiffness = MyForwardFriction;
        frWheelCollider.forwardFriction = frForwardFriction;

        flForwardFriction.stiffness = MyForwardFriction;
        flWheelCollider.forwardFriction = flForwardFriction;

        rrForwardFriction.stiffness = MyForwardFriction;
        rrWheelCollider.forwardFriction = rrForwardFriction;

        rlForwardFriction.stiffness = MyForwardFriction;
        rlWheelCollider.forwardFriction = rlForwardFriction;

        frSidewaysFriction.stiffness = MySidewaysFriction;
        frWheelCollider.sidewaysFriction = frSidewaysFriction;

        flSidewaysFriction.stiffness = MySidewaysFriction;
        flWheelCollider.sidewaysFriction = flSidewaysFriction;

        rrSidewaysFriction.stiffness = MySidewaysFriction;
        rrWheelCollider.sidewaysFriction = rrSidewaysFriction;

        rlSidewaysFriction.stiffness = MySidewaysFriction;
        rlWheelCollider.sidewaysFriction = rlSidewaysFriction;
    		
    }

    void SetFriction(float MyForwardFriction , float MySidewaysFriction ,float MySidewaysFrictionBackward)
    {
       
        frForwardFriction.stiffness = MyForwardFriction;
        frWheelCollider.forwardFriction = frForwardFriction;

        flForwardFriction.stiffness = MyForwardFriction;
        flWheelCollider.forwardFriction = flForwardFriction;

        rrForwardFriction.stiffness = MyForwardFriction;
        rrWheelCollider.forwardFriction = rrForwardFriction;

        rlForwardFriction.stiffness = MyForwardFriction;
        rlWheelCollider.forwardFriction = rlForwardFriction;

        frSidewaysFriction.stiffness = MySidewaysFriction;
        frWheelCollider.sidewaysFriction = frSidewaysFriction;

        flSidewaysFriction.stiffness = MySidewaysFriction;
        flWheelCollider.sidewaysFriction = flSidewaysFriction;

        rrSidewaysFriction.stiffness = MySidewaysFrictionBackward;
        rrWheelCollider.sidewaysFriction = rrSidewaysFriction;

        rlSidewaysFriction.stiffness = MySidewaysFrictionBackward;
        rlWheelCollider.sidewaysFriction = rlSidewaysFriction;
    		
    }

    void SetBrakeEffects(bool PlayEffects){

	    bool isGrounding = false;
	    Vector3 skidmarkPos;
	    Vector3 relativePos;
	    Quaternion rotationToLastSkidmark;
	    if(PlayEffects == true){
		    WheelHit hit;
    		
		    if (rlWheelCollider.GetGroundHit(out hit))
            {
			 
			    DustL.particleEmitter.emit=true;
			    isGrounding = true;
			    //Skidmarks
			    skidmarkPos = hit.point;
			    skidmarkPos.y +=0.1f;
    	
    			
			    if (lastSkidmarkPosL != Vector3.zero){
				    relativePos = lastSkidmarkPosL - skidmarkPos;
				    rotationToLastSkidmark = Quaternion.LookRotation(relativePos);
				    Instantiate( skidmark,skidmarkPos,rotationToLastSkidmark);
			    }
			    lastSkidmarkPosL = skidmarkPos;
		    }
		    else{
			    	
			    DustL.particleEmitter.emit=false;	
			    lastSkidmarkPosL = Vector3.zero;			
		    }
    				
		    if (rrWheelCollider.GetGroundHit(out hit)){
			   
			    DustR.particleEmitter.emit=true;
			    isGrounding = true;
			    //Skidmarks
			    skidmarkPos = hit.point;
			    skidmarkPos.y +=0.1f;
    			
			    if (lastSkidmarkPosR != Vector3.zero){
				    relativePos = lastSkidmarkPosR - skidmarkPos;
				    rotationToLastSkidmark = Quaternion.LookRotation(relativePos);
				    Instantiate( skidmark,skidmarkPos,rotationToLastSkidmark);
			    }
			    lastSkidmarkPosR = skidmarkPos;
		    }
		    else{
			    		
			    DustR.particleEmitter.emit=false;	
			    lastSkidmarkPosR = Vector3.zero;			
		    }
    				
		    if((isPlayingSound==false)&&(isGrounding == true)){
			    isPlayingSound = true;
			    brakeAudioSource.Play();					
		    }
		    if(isGrounding == false){
			    isPlayingSound = false;
			    brakeAudioSource.Stop();					
		    }
    		
	    }
	    else{
		    isPlayingSound = false;
		    brakeAudioSource.Stop();

		    DustL.particleEmitter.emit=false;	
		    DustR.particleEmitter.emit=false;	
		    lastSkidmarkPosL = Vector3.zero;	
		    lastSkidmarkPosR = Vector3.zero;	
	    }
    	
    }
  
    void Update () {

	    RotateWheels();
	    SteelWheels();
    	
    }

    void RotateWheels()
    {
	    flWheel.Rotate(flWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter ,0,0);	
	    frWheel.Rotate(frWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter,0,0);	
	    rlWheel.Rotate(rlWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter,0,0);	
	    rrWheel.Rotate(rrWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter,0,0);	
    }

    void SteelWheels() 
    {
        flWheel.localEulerAngles = new Vector3(flWheel.localEulerAngles.x, flWheelCollider.steerAngle - flWheel.localEulerAngles.z, flWheel.localEulerAngles.z);
        frWheel.localEulerAngles = new Vector3(frWheel.localEulerAngles.x, frWheelCollider.steerAngle - frWheel.localEulerAngles.z, frWheel.localEulerAngles.z);
    }

    void SetCurrentGear()
    {
        int gearNumber;

        gearNumber = gearSpeed.Count;
    	
	    for (var i=0; i< gearNumber;i++){
		    if(gearSpeed[i]>currentSpeed){
			    currentGear = i;
			    break;
		    }
	    }
    }

    void GearSound(){
	    float tempMinSpeed = 0.00f;
	    float tempMaxSpeed = 0.00f;
	    float currentPitch  = 0.00f;
    	
	    switch (currentGear) {
		    case 0:
			    tempMinSpeed =0.00f;
			    tempMaxSpeed = gearSpeed[currentGear];
			    break;
    			
		    default:
			    tempMinSpeed = gearSpeed[currentGear -1];
			    tempMaxSpeed = gearSpeed[currentGear];
                break;
	    }
    	
	    currentPitch =(float)(((Mathf.Abs(currentSpeed) - tempMinSpeed)/(tempMaxSpeed-tempMinSpeed)) + 0.8);

        if (currentPitch > 2) {
            currentPitch = 2;
        }

	    motorAudioSource.pitch = currentPitch;

        motorAudioSourceLow.pitch = currentPitch;
        motorAudioSourceMid.pitch = currentPitch;   	
    	
    }    

}
