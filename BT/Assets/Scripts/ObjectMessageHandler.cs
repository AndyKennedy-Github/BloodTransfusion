using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ObjectMessageHandler : MonoBehaviour
{
    public bool jump = false;
    public bool toScale = false;
    public bool toMove = false;
    public bool follow = false;
    public bool pressed = false;
    public bool correctSet = false;
    public bool chosenSet = false;
    public bool trackTime = false;
    public bool isEmpty;
    public bool playSound;
    public List<bool> stages = new List<bool>();
    public bool stageset;
    public float timertrack = 0;
    public float interval;
    public float pointTotal = 0;
    public int salineAmount = 5;
    public int time = 0;
    public float percentComplete = 0.0f;
    public string inputText;
    public string followTarget;
    public float movementSpeed = 1f;
    public Material normal, good, bad;
    private Vector3 movement;
    public Vector3 scale = new Vector3(5, 5, 5);
    public Vector3 pos = new Vector3(5, 5, 5);
    public Vector3 offset = new Vector3(0.0f,0.2f,-0.10f);
    public AudioSource aSource;
    public AudioClip clip;
    string radialMenuResult;

    private Rigidbody rb; //This object's ridid body
    Animator animator;
    IKController ikcontroller;
    MeshRenderer mr;
    // Start is called before the first frame update

    //instance of expression parser to handle number, string, and bool expressions
    public ExpressionParser ep;

    // Start is called before the first frame update
    // This starts the message handler
    void Start()
    {
        ep = new ExpressionParser();
        animator = GetComponent<Animator>();
        oldpos = this.transform.position;
        ikcontroller = GetComponent<IKController>();
        mr = GetComponent<MeshRenderer>();
    }
    void OnMouseDown()
    {
        // Destroy the gameObject after clicking on it
        print("Clicked on :" + this.name);
        Pressed();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (centerButton==null){
            centerButton = Resources.Load<Texture2D>("RadialMenu_center_01");              
        }
        if (answerButton == null)
        {
            answerButton = Resources.Load<Texture2D>("RadialMenu_Response_01");
        }

        //MenuStart();
        choices = new string[2];
        
        question = "Your Answer?";
        //print(this.name + ": Start: Setting Question: "+ question);

        choices[0] = "Y";
        choices[1] = "N";
        radialMenuActive = false;
        GameObject go=this.gameObject;
        Vector3 mpos= go.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(mpos);

        //print("The screen position is " + screenPos);
        center.x = screenPos.x;
        center.y = Screen.height - screenPos.y;//GUI starts in upper-left, not bottom-left
        MenuSetup();
    }

    public virtual bool HandleMessage(string msg, string param = null)
    {
        /*
        ParamData res;
        if (param[0] == '$') 
            param =  ep.EvaluateParam(param);
        */
        print(this.name + ": Handle Message " + msg + " for " + this.name + " with param = "+ param);
        if (msg == "follow")
        {
            if (param != "false")
            {
                followTarget = param;
                follow = true;
                print("I will follow " + param);
            }
            else
            {
                follow = false;
            }
        }
        //Need a command to switch scenes, for now uses scene name in the param to trans
        if (msg == "switchtoscene")
        {
            print("I'm going to scene " + param);
            SceneManager.LoadScene(param);
        }
        //These two keywords relate to buttons, just so they can be tracked within the scene
        if (msg == "reset")
        {
            pressed = false;
        }

        if (msg == "pressed")
        {
            return pressed;
        }

        if(msg == "isempty")
        {
            return isEmpty;
        }

        if (msg == "gain")
        {
            if (param != null)
            {
                pointTotal += int.Parse(param);
            }
            else
            {
                pointTotal += 1;
            }
        }

        if(msg == "lose")
        {
            if(pointTotal > 0)
            {
                if (param != null)
                {
                    pointTotal += int.Parse(param);
                }
                else
                {
                    pointTotal += 1;
                }
            }
        }

        if(msg == "reducesaline")
        {
            if(salineAmount > 0)
            {
                salineAmount--;
            }
        }

        if(msg == "raiseinterval")
        {
            interval--;
        }

        if(msg == "playsound")
        {
            playSound = true;
            interval = float.Parse(param);
        }

        if(msg == "stopsound")
        {
            playSound = false;
            aSource.Stop();
            timertrack = 0.0f;
        }

        if (msg == "raisesaline")
        {
                salineAmount++;
        }

        if (msg == "setsaline")
        {
            salineAmount = int.Parse(param);
            if(salineAmount < 0)
            {
                salineAmount = 0;
            }
        }

        if(msg == "salineamount")
        {
            if(salineAmount == int.Parse(param))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if(msg == "play")
        {
            animator.SetTrigger(param);
            print("I'm playing " + param);
        }

        // JUMP
        if (msg == "jump")
        {
            jump = true;
            print("imma jump");
        }

        if(msg == "increasetime")
        {
            time += 15;
        }

        if(msg == "decreasetime")
        {
            if(time > 0)
            {
                time -= 15;
            }
        }

        if(msg == "timeequals")
        {
            if(int.Parse(param) == time)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if(msg == "says")
        {
            if(param == inputText)
            {
                Debug.Log("My name is " + gameObject.name + " my input text equals " + inputText);
                return true;
            }
            else
            {
                return false;
            }
        }

        if(msg == "setstages")
        {
            if(param == "2")
            {
                bool compDone = false;
                bool phoneDone = false;
                bool GXMpatient = false;
                bool GXMblood = false;
                bool GXMvalid = false;
                bool consentDates = false;
                bool consentSignature = false;
                bool consentPatient = false;
                bool consentIC = false;

                stages.Add(compDone);
                stages.Add(phoneDone);
                stages.Add(GXMpatient);
                stages.Add(GXMblood);
                stages.Add(GXMvalid);
                stages.Add(consentDates);
                stages.Add(consentSignature);
                stages.Add(consentPatient);
                stages.Add(consentIC);

                stageset = true;
            }
            if(param == "3")
            {
                bool boxFridge = false;
                bool openBox = false;
                bool removeSelector = false;
                bool openFridge = false;
                bool iceTable = false;
                bool iceBox = false;
                bool selectorBox = false;
                bool GXMbox = false;
                bool closeBox = false;

                stages.Add(boxFridge);
                stages.Add(openBox);
                stages.Add(removeSelector);
                stages.Add(openFridge);
                stages.Add(iceTable);
                stages.Add(iceBox);
                stages.Add(selectorBox);
                stages.Add(GXMbox);
                stages.Add(closeBox);

            }
            if(param == "1")
            {
                bool IDDesk = false;
                bool FormDesk = false;
                bool PatientMove = false;
                bool NurseCall = false;
                bool PatName = false;
                bool PatID = false;
                bool Blood = false;
                bool IDBlood = false;

                stages.Add(IDDesk);
                stages.Add(FormDesk);
                stages.Add(PatientMove);
                stages.Add(NurseCall);
                stages.Add(PatName);
                stages.Add(PatID);
                stages.Add(Blood);
                stages.Add(IDBlood);
         }
            if (param == "4")
            {
                bool RightSet = false;
                bool DishAdded = false;

                stages.Add(RightSet);
                stages.Add(DishAdded);
            }
            if(param == "5")
            {
                bool PatientClick = false;
                bool NurseClick = false;
                bool CorrectName = false;
                bool GXMCorrect = false;
                bool LabelCorrect = false;
                bool BloodBagCorrect = false;
                bool TransfusionStart = false;
                bool GXMBlood = false;

                stages.Add(PatientClick);
                stages.Add(NurseClick);
                stages.Add(CorrectName);
                stages.Add(GXMCorrect);
                stages.Add(LabelCorrect);
                stages.Add(BloodBagCorrect);
                stages.Add(TransfusionStart);
                stages.Add(GXMBlood);
            }
            if (param == "6")
            {
                bool Educate = false;
                bool VitalsOn = false;
                bool Sanitizer = false;
                bool Gloves = false;
                bool AlcoholFirst = false;
                bool Syringe = false;
                bool Trash = false;
                bool InfusionCombination = false;
                bool BloodPole = false;
                bool Squeeze = false;
                bool Clamp = false;
                bool CuffOn = false;
                bool Disinfect = false;
                bool Release = false;

                stages.Add(Educate);
                stages.Add(VitalsOn);
                stages.Add(Sanitizer);
                stages.Add(Gloves);
                stages.Add(AlcoholFirst);
                stages.Add(Syringe);
                stages.Add(Trash);
                stages.Add(InfusionCombination);
                stages.Add(BloodPole);
                stages.Add(Squeeze);
                stages.Add(Clamp);
                stages.Add(CuffOn);
                stages.Add(Disinfect);
                stages.Add(Release);
            }
            if(param == "7")
            {
                bool MonitorFirst = false;
                bool CannulaFirst = false;
                bool PatientSymptomFirst = false;
                bool CannulaSecond = false;
                bool PatientSymptomSecond = false;
                bool Chamber = false;
                bool MonitorSecond = false;
                bool CannulaThird = false;
                bool PatientSymptomThird = false;
                bool Clamp = false;
                bool Soap = false;
                bool Gloves = false;
                bool Wipe = false;
                bool Syringe = false;
                bool Leave = false;

                stages.Add(MonitorFirst);
                stages.Add(CannulaFirst);
                stages.Add(PatientSymptomFirst);
                stages.Add(CannulaSecond);
                stages.Add(PatientSymptomSecond);
                stages.Add(Chamber);
                stages.Add(MonitorSecond);
                stages.Add(CannulaThird);
                stages.Add(PatientSymptomThird);
                stages.Add(Clamp);
                stages.Add(Soap);
                stages.Add(Gloves);
                stages.Add(Wipe);
                stages.Add(Syringe);
                stages.Add(Leave);
            }
        }

        if(msg == "perfect")
        {
            if(gameObject.GetComponent<UITextShower>() != null)
            {
                gameObject.GetComponent<UITextShower>().perfect = true;
            }
        }
        
        if (msg == "good")
        {
            if (gameObject.GetComponent<UITextShower>() != null)
            {
                gameObject.GetComponent<UITextShower>().good = true;
            }
        }

        if (msg == "bad")
        {
            if (gameObject.GetComponent<UITextShower>() != null)
            {
                gameObject.GetComponent<UITextShower>().bad = true;
            }
        }

        if (msg == "complete")
        {
            stages[int.Parse(param)] = true;
        }

        if(msg == "iscomplete")
        {
            return stages[int.Parse(param)];
        }

        if(msg == "rangecomplete")
        {
            char[] separators = new char[] { ' ', ',', '-' };

            string[] temp = param.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            int start = int.Parse(temp[0]);
            int stop = int.Parse(temp[1]);
            int totalComplete = 0;

            for(int i = start; i < stop +1; i++)
            {
                if(stages[i])
                {
                    totalComplete++;
                }
            }

            Debug.LogWarning("Total: " + totalComplete);
            if(totalComplete == stop - start + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if(msg == "getcompletepercent")
        {
            Debug.LogWarning(pointTotal);
            percentComplete = (pointTotal/stages.Count) * 100;
            Debug.LogWarning(percentComplete);
        }

        if(msg == "allcomplete")
        {
            
            if (percentComplete == 100)
            {
                return true;
            }
            else
            {
                return false;
            }    
        }

        if(msg == "outlineon")
        {
            Material[] mats = mr.materials;
            if (mats.Length == 1)
            {
                System.Array.Resize(ref mats, mats.Length + 1);
                mats[1] = normal;
            }
            mr.materials = mats;
        }

        if(msg == "outlineoff")
        {
            Material[] mats = mr.materials;
            if (mats.Length == 2)
            {
                System.Array.Resize(ref mats, mats.Length - 1);
                
            }
            mr.materials = mats;
        }

        if(msg == "percentachieved")
        {
            if(percentComplete > int.Parse(param))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if(msg == "showtext")
        {
            if(GetComponent<UITextShower>() != null)
            {
                GetComponent<UITextShower>().DisplayText();
            }
        }

        if(msg == "stageset")
        {
            return stageset;
        }
        /////////////////////////////////////////////////////////////////////

        // ON
        if (msg == "on")
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
        // OFF
        if (msg == "off")
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
        if(msg == "delete")
        {
            Destroy(this.gameObject);
        }

        // ROTATEY
        if (msg == "rotatey")
        {
            float duration = float.Parse(param);
            //float duration = 2f;
            print("Start Rotating Door" + this.name);
            StartCoroutine(RotateMe(duration));
        }

        if(msg == "rotatetoy")
        {
            transform.DORotate(new Vector3(transform.rotation.x, float.Parse(param), transform.rotation.z), 1);
        }


        if (msg == "rotatetox")
        {
            transform.DORotate(new Vector3(float.Parse(param), transform.rotation.y, transform.rotation.z), 1);
        }


        if (msg == "rotatetoz")
        {
            transform.DORotate(new Vector3(transform.position.x, transform.rotation.y, float.Parse(param)), 1);
        }

        if(msg == "matchrotation")
        {
            GameObject go = GameObject.Find(param);
            transform.rotation = go.transform.rotation;
            print(transform.rotation);
        }

        if(msg == "correctset")
        {
            correctSet = true;
        }

        if(msg == "iscorrect")
        {
            return correctSet;
        }

        if(msg == "playerchose")
        {
            chosenSet = true;
        }

        if(msg == "playerreplace")
        {
            chosenSet = false;
        }

        if(msg == "ischosen")
        {
            return chosenSet;
        }

        if(msg == "parentto" || msg == "attachto")
        {
            GameObject go = GameObject.Find(param);
            if(transform.parent != null)
            {
                transform.parent = null;
                transform.parent = go.transform;
            }
            else
            {
                transform.parent = go.transform;
            }
        }

        if(msg == "changeshader")
        {
            Material[] mats = mr.materials;
            if(param == "normal")
            {
                mats[1] = normal;
                mr.materials = mats;
            }
            else if(param == "good")
            {
                mats[1] = good;
                mr.materials = mats;
            }
            else if (param == "bad")
            {
                mats[1] = bad;
                mr.materials = mats;
            }
            else if(param == "off")
            {
                Array.Resize(ref mats, mats.Length - 1);
            }
        }

        if(msg == "shadercheck")
        {
            Material[] mats = mr.materials;
            if (param == "good")
            {
                if (mats[1].name == good.name)
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning("Current Highlight " + mats[1].name);
                    return false;
                }
            }
            else if(param == "bad")
            {
                
                if (mats[1].name == bad.name)
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning("Current Highlight " + mats[1].name);
                    return false;
                }
            }
            else if(param == "normal")
            {
                if (mats[1].name == normal.name)
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning("Current Highlight " + mats[1].name);
                    return false;
                }
            }
        }

        // SCALE
        if (msg == "scale")
        {
            print("hello, I am scaling");
            toScale = true;
            scale = getVector3(param);
            print("The scale is " + scale);

            //do something...
        }

        // GRAB
        if (msg == "grab")
        {
            print("grab ");


            GameObject go=GameObject.Find(param); //moveTo object's position
                print("grabbing game object "+ go.name);
            ikcontroller.rightHandObj= go.transform;
            ikcontroller.lookObj = go.transform;
            StartCoroutine(Grab(1.0f));


            //do something...
        }
        if (msg == "release")
        {
            print("release ");

            
//            GameObject go=GameObject.Find(param); //moveTo object's position
//                print("releasing game object "+ go.name);
//            ikcontroller.rightHandObj= go;
//            ikcontroller.lookObj = go;
            StartCoroutine(Release(1.0f));


            //do something...
        }
        // MOVETO
        if (msg == "moveto" || msg== "align")
        {
            print("hello, I am moving");
            toMove = true;
            Vector3 oldpos = transform.position;
            
            if ((param[0] == '-'  || System.Char.IsDigit (param[0]))){ //moveTo position
                pos = getVector3(param);
                //transform.DOMove(pos, 2);
                transform.DOMove(pos, 2).SetEase(Ease.Linear);
            }
            else{

                GameObject go=GameObject.Find(param); //moveTo object's position
                print("moving to position of game object "+ go.name);
                pos= go.transform.position;
                if (msg=="align")
                    transform.rotation = go.transform.rotation;
                //transform.DOMove(pos, 2);
                transform.DOMove(pos, 2).SetEase(Ease.Linear);
            }

            print("The position is " + pos);

            //do something...
        }


        // RADIALMENU
        //Each object can have its own GUI menu element, so multiple ones can be present at a time.
        //Menu.on turns on the menu at either this object's location or a location that you give it.
        //[ObjectHandlingMessage] Menu.on [OptionalObjectOrPositionToLookat]
        //MedForm Menu.on MedForm/SignatureLine
        //MedForm Menu.on 0.0 1.0 0.3
        if (msg == "menu.on")
        {
            print("Setup and Turn radialMenu on for "+ this.name);
            radialMenuResult="";
            //toMove = true;

            Vector3 mpos;
            GameObject go=this.gameObject;
            mpos= go.transform.position;
            if (param!=null){
                if  (System.Char.IsDigit (param[0])){ //moveTo position
                    mpos = getVector3(param);
                }else{
                    print("getting game object for param " + param);
                    go=GameObject.Find(param); //moveTo object's position
                    print("getting position of game object "+ go.name);
                    mpos= go.transform.position;
                }
            }
            Vector3 screenPos = Camera.main.WorldToScreenPoint(mpos);

            print("The screen position is " + screenPos);
            center.x = screenPos.x;
            center.y = Screen.height - screenPos.y;//GUI starts in upper-left, not bottom-left
            MenuSetup(); 
            radialMenuActive=true;

            //do something...
        }

        //MedForm Menu.Question Is it signed?
        if (msg == "menu.question")
        {
            print(this.name + ": mhandler: Setting Question to "+ param);
            question = param;
            print(this.name + ": mhandler: Question: "+ question);
        }
        if (msg == "menu.choices")
        {
            char[] separators = new char[] { ' '};
            string [] tmp = param.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            print("tmp legnth = "+ tmp.Length + ",sep=" + tmp);
            choices = tmp;
            print("choices legnth = "+ choices.Length + ",sep=" + choices);
        }
        if (msg == "menu.done")
        {
            return !radialMenuActive;
        }
        if (msg == "menu.result")
        {
            print("radialMenuResult: param ="+ param);
            print("radialResult= "+ radialMenuResult);
            print("conditional = " + (param == radialMenuResult));
            return (param == radialMenuResult);
            
        }


        //LOOKATME
        // [Object] lookAtMe [Offset]
        // e.g.: /ExamRoom1/Desk lookAtMe  0.0,1.0,1.0   
        if (msg == "lookatme")
        {
            print("lookAtMe");
            //toMove = true;
            Vector3 mpos;
            //BUG Needs to be fixed: DOESNT HANDLE NEGATIVE SIGN
            if (param!=null && (param[0] == '-' || System.Char.IsDigit (param[0]))){ //moveTo position
                offset = getVector3(param);
            }
            {
                GameObject go=this.gameObject;//GameObject.Find(param); //moveTo object's position
                print("getting position of game object "+ go.name);
                mpos= go.transform.position;
            }
            //transform.position = pos;
            Camera.main.transform.position = mpos + offset;
            Camera.main.transform.LookAt(mpos);
        } //lookAtMe

        // LOOKAAT
        // lookAt targetObject offset
        // lookAt targetObject viewerObject(for position)
        if (msg == "lookat")
        {
            print("lookAt" + param);
            //toMove = true;
            Vector3 vpos,tpos;
            /*  Need to support both object and offset*/
            char[] separators = new char[] { ' '};

            string[] temp = param.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            param = temp[0];
            GameObject go=GameObject.Find(param); //moveTo object's position
            print("getting position of target game object "+ go.name);
            tpos= go.transform.position;
            if (temp.Length>1)
            {
                string offsetStr = temp[1];
                if ((offsetStr[0] == '-' ) || System.Char.IsDigit (offsetStr[0])){ //moveTo position
                    pos = getVector3(offsetStr);
                    offset = getVector3(temp[1]);
                    vpos= tpos + offset;
                }else{
                    GameObject vgo=GameObject.Find(offsetStr); //moveTo object's position
                    vpos= vgo.transform.position;
                }
                Camera.main.transform.position = vpos;
            }

            //transform.position = pos;
            Camera.main.transform.LookAt(tpos);
        } //lookat


        if (msg == "ison")
        {
            return this.transform.GetChild(0).gameObject.activeSelf;
        } //isOn

        return true;
    }

    ////////////////////////////////////////////////////////////////////////////
    //Helper functions

    //Get Vector3 in form of either  1.0 2.0 3.0   or 1.0,2.0,3.0
    public Vector3 getVector3(string rString)
    {
        print("getVector3:"+ rString);
        char[] separators = new char[] { ' ', ',' };

        string[] temp = rString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        //string[] temp = rString.Split(' ');
//        print("getVector3x:"+ temp[0]);
//        print("getVector3y:"+ temp[1]);
//        print("getVector3z:"+ temp[2]);
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);
//        print("getVector3: ("+x+","+y+","+z+")");
        Vector3 rValue = new Vector3(x, y, z);
        return rValue;
    }

    private IEnumerator Grab(float duration)
    {
        ikcontroller.ikActive = true;
        float t = 0.0f;
        while (t < duration)
        {
            //print("Grabbing "+ transform.rotation);
            t += Time.deltaTime;
            ikcontroller.ikStrength = t/duration;
            yield return null;
        }
    }
    private IEnumerator Release(float duration)
    {
        float t = 0.0f;
        while (t < duration)
        {
            //print("Grabbing "+ transform.rotation);
            t += Time.deltaTime;
            ikcontroller.ikStrength = 1.0f - t/duration;
            yield return null;
        }
        ikcontroller.ikActive = false;
    }

    private IEnumerator RotateMe(float duration)
    {
        Quaternion startRot = transform.rotation;
        float t = 0.0f;
        while (t < duration)
        {
            //print("Rotating "+ transform.rotation);
            t += Time.deltaTime;
            transform.rotation = startRot * Quaternion.AngleAxis(t / duration * 360f, transform.up); //or transform.right if you want it to be locally based
            yield return null;
        }
        transform.rotation = startRot;
    }

    //Jump behavior
    public float jumpVelocity = 8.5f;

    public float fallMult = 2.5f;
    public float lowJumpMult = 2f;
    public Vector3 gravity = new Vector3(0f, -10f, 0f);

    private void Jump()
    {
        if (rb)
        {
            if (jump)
            {
                rb.velocity = Vector3.up * jumpVelocity;
            }
            else
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up + gravity * (fallMult - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0)
            {
                rb.velocity += Vector3.up + gravity * (lowJumpMult - 1) * Time.deltaTime;
            }
            jump = false;
        }
    }

    private void Move()
    {
        if (toMove)
        {
            transform.position = pos;
        }
        print("at MOVE the pos is = to" + pos);
        toMove=false;
    }
    private void Scale()
    {
        if (toScale)
        {
            transform.localScale = scale;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        print("at SCALE the scale is = to" + scale);
        toScale=false;
    }
    private void followPlayer()
    {

        var player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = player.transform.position - transform.position;
        transform.LookAt(player.transform);

        //rb.MovePosition((Vector3)transform.position + transform.forward *  Time.fixedDeltaTime);//(direction * movementSpeed * Time.fixedDeltaTime));
        this.transform.position = ((Vector3)transform.position + transform.forward *  Time.fixedDeltaTime);//(direction * movementSpeed * Time.fixedDeltaTime));


    }
    public void Pressed()
    {
        pressed = true;
    }
    private void FollowObj()
    {
        var player = GameObject.FindGameObjectWithTag(followTarget);
        Vector3 direction = player.transform.position - transform.position;
        transform.LookAt(player.transform);
        //rb.MovePosition((Vector3)transform.position + transform.forward *  Time.fixedDeltaTime);//(direction * movementSpeed * Time.fixedDeltaTime));

        this.transform.position = ((Vector3)transform.position + transform.forward * Time.fixedDeltaTime * .5f);
        //(direction * movementSpeed * Time.fixedDeltaTime));
    }

Vector3 oldpos ;

    // Update is called once per frame
    private void Update()
    {
        Vector3 pos = this.transform.position;
        float velocity = ((oldpos - pos)/Time.deltaTime).magnitude;
        //print("Velocity = "+ velocity);
        /*if (animator)
            animator.SetFloat("Speed",velocity);
        */
        oldpos = this.transform.position;
        if(salineAmount > 0)
        {
            isEmpty = false;
        }
        else
        {
            isEmpty = true;
        }

        if(playSound)
        {
            timertrack += Time.deltaTime;

            if(timertrack >= interval)
            {
                aSource.PlayOneShot(clip);
                timertrack = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        if (jump)
        {
            Jump();
            print("I jumped");
        }
        if (toScale)
        {
            Scale();
            print("I have scaled by" + scale);
        }
        if (toMove)
        {
            Move();
            print("I have moved by" + pos);
        }
        if (follow)
        {
            follow = true;
            FollowObj();
        }
    }

    ///////////////////////////////////////////////////////////////////////

    // GUI Radial Menu Stuff

    public  Vector2 center = new Vector2(500,500); // position of center button
    public int radius = 125;  // pixels radius to center of button;
    public Texture centerButton;
    public Texture answerButton;
    public RectOffset rectOff;
    // public Texture [] normalButtons;// : Texture[];
    // public Texture [] selectedButtons;// : Texture[];
    public string question;
    public string [] choices;
    
    private int ringCount;// : int; 
    private Rect centerRect;// : Rect;
    private Rect[] ringRects;// : Rect[];
    private float angle;// : float;
    private bool showButtons = false;
    private int index;// : int;
    private int menusize = 100 ;
    public float menuSizeFraction=0.5f;
    void MenuSetup () {
        menusize = (int) ((float) Screen.height*menuSizeFraction/3.0f);
        radius = menusize * 125/100;
        if (choices == null)
            return;
        ringCount = choices.Length;
        angle = 360.0f / ringCount;
        int centerButton_width = menusize * 3;
        int centerButton_height = menusize * 3;
        centerRect.x = center.x - centerButton_width  * 0.5f;
        centerRect.y = center.y - centerButton_height * 0.5f;
        centerRect.width = centerButton_width;
        centerRect.height = centerButton_height;
        
        ringRects = new Rect[ringCount];
        
        var w = menusize;//normalButtons[0].width;
        var h = menusize;//normalButtons[0].height;
        var rect = new Rect(0, 0, w, h);
        
        var v = new Vector2(radius * 1.5f,0);
        
        for (var i = 0; i < ringCount; i++) {
            rect.x = center.x + v.x - w * 0.5f;
            rect.y = center.y + v.y - h * 0.5f;
            ringRects[i] = rect;
            v = Quaternion.AngleAxis(angle, Vector3.forward) * v;
        }
    }
    public bool radialMenuActive = false;



    void OnGUI() {
        if (!radialMenuActive || ringCount==0)
            return;
        var e = Event.current;
        
        if (e.type == EventType.MouseDown && centerRect.Contains(e.mousePosition)) {
            showButtons = true;
            index = -1;
        }    
                
        if (e.type == EventType.MouseUp) {
            if (showButtons) {
                Debug.Log("User selected #"+index);// + ", " + choices[index]);
                if (index>=0){
                    Debug.Log("User selected #"+choices[index]);
                    radialMenuResult = choices[index];
                    radialMenuActive=false;
                }
            }
            showButtons = false;
        }
            
        if (e.type == EventType.MouseDrag) {
            var v = e.mousePosition - center;
            var a = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            a += angle / 2.0f;
            if (a < 0) a = a + 360.0f;
    
            index = (int) (a / angle);
        }

        //    GUIContent content;
        //       content = new GUIContent(question, centerButton, "This is a tooltip");
        rectOff.left = 50;
        rectOff.right = 50;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.wordWrap = true;
        style.fontSize = menusize/4; //change the font size        
        style.alignment = TextAnchor.MiddleCenter;
        style.padding = rectOff;
        GUI.DrawTexture(centerRect, centerButton);
        GUI.Label(centerRect, question, style);
        //print(this.name + "'s question is "+ question);

        if (showButtons) {
            for (var i = 0; i < choices.Length; i++) {
                if (i != index){ 
                    GUI.DrawTexture(ringRects[i], answerButton);
                    GUI.Label(ringRects[i], choices[i], style);
                    //GUI.DrawTexture(ringRects[i], normalButtons[i]);
                }else{
                    GUI.DrawTexture(ringRects[i], answerButton);
                    GUI.Box(ringRects[i], choices[i], style);
                }
            }
        }
    }
}