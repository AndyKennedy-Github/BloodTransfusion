using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;  //Use this if WWW is obsolete in Unity version
using System.Linq;

/////////////////////////////////////////////////////////////////////////////////////////
// Parameter expressions routines
/////////////////////////////////////////////////////////////////////////////////////////
#region Parameter expressions

//Data types for parameter expressions
public enum DataType { Number, Bool, String };

//Parameter data returned by expressions
public class ParamData
{
    public DataType dataType;
    public double number;
    public bool boolean;
    public string str;

    public static implicit operator string(ParamData v)
    {
        string temp = v.ToString();
        //        Debug.Log("string cast = "+ temp);
        return temp;
    }

    public static implicit operator float(ParamData v)
    {
        return (float)v.number;
    }

    public override string ToString()
    {
        Debug.Log("ParamData.ToString() " + dataType);
        if (dataType == DataType.Number)
            return number.ToString();
        if (dataType == DataType.Bool)
            return boolean.ToString();
        if (dataType == DataType.String)
            return str.ToString();
        return "Undefined Data Type";
    }
}

public class ExpressionParser
{
    public DataTable loDataTable;

    public DataType dataType;

    public ExpressionParser()
    {
        loDataTable = new DataTable();
        var loDataColumn = new DataColumn("EvalNum", typeof(double), "0");
        loDataTable.Columns.Add(loDataColumn);
        loDataColumn = new DataColumn("EvalString", typeof(string), "");
        loDataTable.Columns.Add(loDataColumn);
        loDataColumn = new DataColumn("EvalBool", typeof(bool), "false");
        loDataTable.Columns.Add(loDataColumn);
        loDataTable.Rows.Add(0);
        loDataTable.Rows.Add(1);
        dataType = DataType.Number;
    }

    public void SetVar(string name, ParamData value)
    {
        CreateVar(name, value);
        Debug.Log("Setting variable with name " + name);
        //        loDataTable.Rows[0][name] = value;
        if (value.dataType == DataType.Number)
        {
            loDataTable.Rows[0][name] = value.number;
            loDataTable.Rows[1][name] = value.number + 1;
        }
        if (value.dataType == DataType.Bool)
            loDataTable.Rows[0][name] = value.boolean;
        if (value.dataType == DataType.String)
            loDataTable.Rows[0][name] = value.str;
    }

    void CreateVar(string name, ParamData value)
    {
        if (loDataTable.Columns.Contains(name))
        {
            return;
        }
        Debug.Log("Creating variable with name " + name);
        DataColumn varColumn = new DataColumn();
        if (value.dataType == DataType.Number)
            varColumn.DataType = typeof(double);
        if (value.dataType == DataType.Bool)
            varColumn.DataType = typeof(bool);
        if (value.dataType == DataType.String)
            varColumn.DataType = typeof(string);
        varColumn.ColumnName = name;
        //        varColumn.DefaultValue = 50;
        loDataTable.Columns.Add(varColumn);
    }


    public ParamData EvaluateParam(string expression)
    {
        char[] separators = new char[] { ' ', '\t' };

        //var loDataColumn;
        //        if (!loDataTable.Columns.Contains("EvalNum"))
        //            return 0.0;
        if (expression[0] == '$')
        { //strip off leading $
            expression = expression.Substring(1);
        }
        Debug.Log("EvaluateParam: " + expression);
        ParamData ret = new ParamData();
        var loDataColumn = loDataTable.Columns["EvalNum"];
        try
        {
            loDataColumn.Expression = expression;
            ret.number = (double)(loDataTable.Rows[0]["EvalNum"]);
            ret.boolean = !(ret.number == 0f);
            ret.dataType = DataType.Number;
            return ret;
        }
        catch (Exception ex)
        {
            //Debug.Log("Ok Error:"+ ex.Message);
            loDataColumn = loDataTable.Columns["EvalBool"];
            try
            {
                loDataColumn.Expression = expression;
                ret.boolean = (bool)(loDataTable.Rows[0]["EvalBool"]);
                ret.dataType = DataType.Bool;
                return ret;

            }
            catch (Exception ex2)
            {
                //Debug.Log("Ok Error:"+ ex2.Message);
                loDataColumn = loDataTable.Columns["EvalString"];
                try
                {
                    loDataColumn.Expression = expression;
                    ret.str = (string)(loDataTable.Rows[0]["EvalString"]);
                    ret.dataType = DataType.String;
                    return ret;
                }
                catch (Exception ex3)
                {
                    string[] splitArray = expression.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    loDataColumn.Expression = splitArray[0];
                    string restparams = expression.StringAfter(splitArray[0]);
                    ret.str = (string)(loDataTable.Rows[0]["EvalString"]);
                    ret.dataType = DataType.String;
                    Debug.Log("Rest of params: " + restparams);
                    ret.str = ret.str + " " + restparams;
                    Debug.Log("Evaluated first part of params: " + ret.str);
                    return ret;

                }
            }
        }
    }
}
#endregion

/////////////////////////////////////////////////////////////////////////////////////////
// Game Manager command parsing/handling
/////////////////////////////////////////////////////////////////////////////////////////

public class CommandSequence
{
    public int commandNum = 0;
    public enum IFState { False, Condition, Then, Else };
    public enum WAITState { False, Condition, ConditionAny, ConditionAll };
    public enum CHOICEState { False, Choice, NotChoice };
    public IFState IFstate = IFState.False;
    public WAITState WAITstate = WAITState.False;
    public CHOICEState CHOICEstate = CHOICEState.False;
    public bool IFresult = true;
    public string commandLine;
    public int nestingLevel;
}

public class MyGameManager : MonoBehaviour
{
    public string[] CommandFiles; //contains list of Scenario files
    private int currentNestingLevel = 0;
    public string Prompt, Prompt1, Prompt2;
    Dictionary<string, string> fullnames = new Dictionary<string, string>();
    Dictionary<string, int> label = new Dictionary<string, int>();
    Dictionary<string, float> variables = new Dictionary<string, float>();

    List<string> GMcommands = new List<string> {
        "if",
        "then",
        "else",
        "endif",
        "do",
        "dochoice",
        "[",
        "choices",
        "choices[",
        "]",
        "load",
        "waitfor",
        "wait",
        "create",
        "goto",
        "prompt",
        "speaker1",
        "speaker2",
        "label"
    };

    string GUIcommandLine, GUIlastcommandLine;

    //array of hardcoded commands for testing
    string[] mycommands = {
        "/Room1/Enemy jump",
        "GM Sleep 1",
        "/Room1/Door rotateY 1.2",
        "GM Sleep 1",
        "/RoomLight off",
        "GM Sleep 1",
        "/RoomLight on",
        "Room1/Room scale 5.0,7.5"
    };

    //instance of expression parser to handle number, string, and bool expressions
    public ExpressionParser ep;

    // Start is called before the first frame update
    // This starts the 
    void Start()
    {
        ep = new ExpressionParser();

        StartCoroutine(exectueScenarioFiles(CommandFiles));
        //StartCoroutine(ExecuteCommands(commands));
    }

    //loads local or remote file
    IEnumerator exectueScenarioFiles(string[] fileNames)
    {
        foreach (string fileName in fileNames)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

            string result;

            if (filePath.Contains("://") || filePath.Contains(":///"))
            {
                //WWW www = new WWW(filePath);
                //yield return www;
                //result = www.text;
                UnityWebRequest webRequest = UnityWebRequest.Get(filePath); //use this if WWW is obsolete
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    print("Web request error:" + webRequest.error + " for " + filePath);
                    continue;
                }
                else
                {
                    result = webRequest.downloadHandler.text;
                }

            }
            else
            {
                result = System.IO.File.ReadAllText(filePath);
            }

            Debug.Log("Loaded file: " + fileNames);

            string[] linesInFile = result.Split('\n');
            yield return StartCoroutine(ExecuteCommands(linesInFile));
        }
    }


    //Executes array of commands
    IEnumerator ExecuteCommands(string[] commands)
    {
        CommandSequence cs = new CommandSequence();
        cs.nestingLevel = currentNestingLevel;
        for (int i = 0; i < commands.Length; i++)
        {
            string objName = "", command = "", cparams = "";
            cs.commandNum = i;
            if (!GetCommandParts(cs, ref commands, ref objName, ref command, ref cparams))
                continue;


            if (objName.Length < 1 || objName[0] == '#')  //skip comments
                continue;


            //First check IF states and DoChoice blocks
            if (IFstateSwitch(command, cs))
            { //handle IF-THEN-ELSE-ENDIF commands
                print("cs.IFstate after stateswitch = " + cs.IFstate);
                continue;
            }

            //Check if Game Manager commands or expressions (begin with $)
            if (objName == "GM" || (objName[0] == '$' && (command == null || command[0] == '=')) || GMcommands.Contains(objName.ToLower())) //objName=="GM" || objName == "if" .. //special Game Manager commands
            {
                //execute GameManager commands
                print("GM command : " + command + " for object " + objName);
                //Handle Game Manager commands

                if (command == "waitfor")
                { //handle WAITFOR commands to wait on next line's boolean value
                    if (cparams != null && cparams.ToLower() == "any")
                        cs.WAITstate = CommandSequence.WAITState.ConditionAny;
                    else if (cparams != null && cparams.ToLower() == "all")
                        cs.WAITstate = CommandSequence.WAITState.ConditionAll;
                    else
                        cs.WAITstate = CommandSequence.WAITState.Condition;
                    continue;
                }


                if (objName[0] == '$')
                { //expression
                    var varname = objName.Substring(1);
                    print("variable name = " + varname);

                    print("command = " + command);
                    ParamData res;
                    if (command == "#")
                    {  // no assignment, so just lookup expression value
                        print("Just param: Evaluate param  " + varname);
                        res = ep.EvaluateParam(varname);
                        //get return value if in conditional statement (last statement was IF)
                        if (cs.IFstate == CommandSequence.IFState.Condition)
                        {
                            cs.IFresult = res.boolean;
                            print("IF Conditional = " + res.boolean);
                        }
                        continue;
                    }
                    else
                    {  //assignment.  Get value and assign
                        if (cparams == null || cparams == "")
                        { //gets value from command on next line

                        }

                        print("expression = " + cparams);
                        //now replace $vars with numbers
                        //ep.CreateVar(varname);

                        res = ep.EvaluateParam(cparams);
                        if (command == "=$")
                            res = ep.EvaluateParam(res.str);
                    }
                    string t = res.ToString();
                    Debug.Log("T=" + t);
                    print("RESULT: " + varname + " = " + t);
                    ep.SetVar(varname, res);
                    //                    variables.AddSafe(varname,res);
                    print("ADDED VARIABLE: " + varname + " = " + res);

                }

                //Add additional commands below.
                if (command == "create")
                {
                    string[] splitArray = cparams.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    string objPrefabName = splitArray[0];
                    if (objPrefabName[0] == '$')
                        objPrefabName = ep.EvaluateParam(objPrefabName);
                    print("Create " + objPrefabName);
                    GameObject obj = (GameObject)Instantiate(Resources.Load(objPrefabName));
                    ObjectMessageHandler omh;
                    if (!obj.GetComponent<ObjectMessageHandler>())
                        omh = obj.AddComponent<ObjectMessageHandler>() as ObjectMessageHandler;
                    if (splitArray.Length > 1)
                        obj.name = ep.EvaluateParam(splitArray[1]);

                }
                if (command == "load" || command == "do")
                {
                    string[] sArray = { "" };
                    //                    if (cparams[0]=='$')
                    sArray[0] = ep.EvaluateParam(cparams);
                    //                    else 
                    //                        sArray[0] = cparams;  //convert to string array for loadStreamingAsset
                    print("Load " + sArray[0]);
                    currentNestingLevel++;
                    StartCoroutine(exectueScenarioFiles(sArray));
                    // coroutineDone is never called
                    //bool coroutineDone = false;
                    while (currentNestingLevel != cs.nestingLevel)
                    { //keep doing until coroutine done
                        yield return new WaitForSeconds(1.0f);
                    }

                }
                if (command == "dochoice")
                {    //Sleep for a given time in seconds
                    string paramStr = cparams;
                    print("Do choice " + paramStr);
                    int choicenum = (int)(float)ep.EvaluateParam(cparams);//float.Parse(paramStr);
                    i += choicenum;
                    cs.CHOICEstate = CommandSequence.CHOICEState.Choice;
                    yield return null;
                    continue;
                }


                if (command == "wait")
                {    //Sleep for a given time in seconds
                    string paramStr = cparams;
                    print("Sleep for " + paramStr);
                    float delay = ep.EvaluateParam(cparams);//float.Parse(paramStr);
                    yield return new WaitForSeconds(delay);
                }

                if (command == "goto")
                {     //GOTO a line number in the Scenario file
                    string paramStr = cparams;
                    if (cparams[0] == '$')
                        paramStr = ep.EvaluateParam(cparams);
                    if ((paramStr[0] == '-' || System.Char.IsDigit(paramStr[0])))
                    { //moveTo position
                        i = int.Parse(paramStr) - 2;  //array starts at 0, and i increments, so must minus 2
                    }
                    else
                    {  //is a label
                        i = label[paramStr];  //starts at line after label
                    }
                    yield return null;  //make sure we don't get caught in infinite loop which hangs unity
                    continue;
                }
                if (command == "prompt")
                {   //Prompt dialog message
                    //Prompt = cparams;//(string) ep.EvaluateParam(cparams);
                    Prompt = (string)ep.EvaluateParam(cparams);
                }
                if (command == "label")
                {     //Label a line number in the Scenario file
                    string paramStr = cparams;
                    label.AddSafe(paramStr, i);
                }
                if (command == "prompt")
                {   //Prompt dialog message
                    //Prompt = cparams;//(string) ep.EvaluateParam(cparams);
                    Prompt = (string)ep.EvaluateParam(cparams);
                }
                if (command == "speaker1")
                {   //Prompt dialog message
                    //Prompt = cparams;//(string) ep.EvaluateParam(cparams);
                    Prompt1 = (string)ep.EvaluateParam(cparams);
                }
                if (command == "speaker2")
                {   //Prompt dialog message
                    //Prompt = cparams;//(string) ep.EvaluateParam(cparams);
                    Prompt2 = (string)ep.EvaluateParam(cparams);
                }
                continue;


            }
            else //Not Game Manager command, so send commands to other game objects
            {

                if (objName[0] == '$')
                    objName = ep.EvaluateParam(objName);
                //print("IFstate before " + command + " = " + cs.IFstate);
                bool result = processObjectsCommand(objName, command, cparams);
                print("returned from processObjCom " + command + ", result= " + result);
                //print("cs.IFstate after = "+ cs.IFstate);

                //get return value if in conditional statement (last statement was IF)
                if (cs.IFstate == CommandSequence.IFState.Condition)
                {
                    cs.IFresult = result;
                    print("IF Conditional = " + result);
                }
                //get return value if in conditional statement (last statement was IF)
                if (cs.WAITstate == CommandSequence.WAITState.Condition)
                {
                    bool WAITresult = result;
                    //print("Wait ready = "+ result);
                    if (!WAITresult)
                    { //keep doing until result is ready
                        i--;
                        yield return new WaitForSeconds(0.5f);
                    }
                    else
                        cs.WAITstate = CommandSequence.WAITState.False;
                }
            }
        }
        currentNestingLevel--;
    }

    //Handles states for If-Then-Else-Endif and DoChoice blocks
    bool IFstateSwitch(string command, CommandSequence cs)
    {
        if (command == "]")
        {  //endo of choices
            cs.CHOICEstate = CommandSequence.CHOICEState.False;
            return true;
        }
        if (cs.CHOICEstate == CommandSequence.CHOICEState.NotChoice)
            return true;
        if (cs.CHOICEstate == CommandSequence.CHOICEState.Choice)
            cs.CHOICEstate = CommandSequence.CHOICEState.NotChoice;

        //Handle IF-THEN-ELSE-ENDIF statements
        //cs.IFstate keeps track of where we are in the IF statement
        if (cs.IFstate == CommandSequence.IFState.Condition)
        {  //If we are in conditional part (IF)
            if (command == "then")
            {         //IF switches us to next state
                cs.IFstate = CommandSequence.IFState.Then;
                return true;
            }
        }
        if (cs.IFstate == CommandSequence.IFState.Then)
        {       //If we are in THEN statements
            if (command == "else")
            {         //switch state when ELSE statement
                cs.IFstate = CommandSequence.IFState.Else;
                return true;
            }
        }
        if (command == "endif")
        {            //Leave cs.IFstate when ENDIF is reached
            cs.IFstate = CommandSequence.IFState.False;
            return true;
        }
        if (command == "if")
        {       //IF command
            cs.IFstate = CommandSequence.IFState.Condition;
            return true;
        }

        if (cs.IFstate == CommandSequence.IFState.Then && !cs.IFresult)  //skip THEN if conditiional is false
            return true;
        if (cs.IFstate == CommandSequence.IFState.Else && cs.IFresult)  //skip ELSE if conditional is true
            return true;
        return false;
    }
    char[] separators = new char[] { ' ', '\t' };

    //breaks up current line into command object parameters
    bool GetCommandParts(CommandSequence cs, ref string[] commands, ref string objName, ref string command, ref string cparams)
    {
        command = "#";
        cparams = null;

        GUIlastcommandLine = cs.commandLine;
        cs.commandLine = commands[cs.commandNum];
        cs.commandLine = cs.commandLine.Trim(); //remove extra newlines or whitespace at beginning and end
        print("-----GM:Execute Command[" + cs.commandNum + "] = " + cs.commandLine);
        if (cs.commandLine == "")
            return false;
        print("IFstate = " + cs.IFstate + ", IFResult = " + cs.IFresult);
        GUIcommandLine = cs.commandLine;

        //split up cs.commandLine into objName, command, and cparams
        string[] splitArray = cs.commandLine.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        objName = splitArray[0];  //
        int paramStart = 2;
        if (GMcommands.Contains(objName.ToLower()))
        { //GM command without GM
            command = objName;
            paramStart = 1;
        }
        else
        {  //GM or other object
            if (splitArray.Length > 1)
                command = splitArray[1];
        }
        print("command=" + command);

        //Get the parameters
        if (splitArray.Length > paramStart)
        { //there are parameters too
            //find where parameters start (after command)
            /*
            int start = 0;
            int end = cs.commandLine.Length;
            int count = end - start;
            int at = cs.commandLine.IndexOf(command, start, count);
            if (at == -1) return false;
            start = at+command.Length+1;

            //Trim off end comments like this  #this is a comment
            //print("cs.commandLine cparams starts at "+ start);
            string ptmp = cs.commandLine.Substring(start);*/
            string ptmp = cs.commandLine.StringAfter(command);
            //print("ptmp="+ptmp);
            string[] tmp = ptmp.Split('#');
            cparams = tmp[0].Trim();
        }
        //have to change to lower after params because command is searched to calc params
        command = command.ToLower();//change command to lower case
        return true;
    }
    /*
        //gets the rest of "all" after location of "match" in all.
        public string StringAfter(string all,string match)
        {
            int start = 0;
            int end = all.Length;
            int count = end - start;
            int at = all.IndexOf(match, start, count);
            if (at == -1) return "";
            start = at+match.Length+1;

            //Trim off end comments like this  #this is a comment
            //print("cs.commandLine cparams starts at "+ start);
            return all.Substring(start);
        }
    */
    private GameObject[] m_gameObjects;

    //Find children objects of objName for commands meant for multiple objects
    //Supports ANY and ALL patterns
    //e.g.: Lights/ALL
    //Returns list of children
    GameObject[] FindGameObjects(string objName)
    {

        print("FindGameObjects for " + objName);
        int start = objName.Length - 1;
        int count = objName.Length;
        int at = objName.LastIndexOf("/", start, count);
        print("LastIndexOf / = " + at);
        objName = objName.Remove(at);
        print("Multiple Objects under " + objName + "");

        GameObject go = GameObject.Find(objName);  //   /Room1/Enemy/ANY   or /Room/ANY/Enemy/ANY
        m_gameObjects = new GameObject[go.transform.childCount];
        for (int i = 0; i < go.transform.childCount; i++)
        {
            m_gameObjects[i] = go.transform.GetChild(i).gameObject;
        }
        //TODO 
        return m_gameObjects;
        //        return null;
    }

    //Process commands meant for multiple objects
    //Supports ANY and ALL patterns
    //e.g.: Lights/ALL
    bool processObjectsCommand(string objName, string command, string cparams)
    {
        GameObject[] Objects;
        bool retval;

        string[] splitArray = objName.Split('/');
        int last = splitArray.Length - 1;
        string endObject = splitArray[last];
        if ((endObject == "ALL") || (endObject == "ANY"))
        {
            print(" Sending command " + command + " to all/any");
            //ADD FindGameObjects function to find ALL children objects of objName, 
            //i.e. Room1/Lights/ALL should find:
            //  Room1/Lights/Light1 and Room1/Lights/Light2
            Objects = FindGameObjects(objName);

            if (endObject == "ANY")
            {

                //ADD CODE TO HANDLE "ANY" 
                int randI = UnityEngine.Random.Range(0, Objects.Length);
                print("ANY picked object " + Objects[randI].name);
                retval = processObjectCommand(Objects[randI], command, cparams);
                //retval = true;

            }
            else
            {  //handle ALL
                retval = true;

                foreach (GameObject gameObj in Objects)
                {
                    print("ALL picked object " + gameObj.name);
                    retval = processObjectCommand(gameObj, command, cparams);
                }
            }
        }
        else
        {  //only call for named GameObject
            string fullname = objName;

            //NOTE: if we want to support caching of partial path names, we would use this instead:
            //if (!objName[0]= '/'){ //doesn't not have full path, so check if in dictionary
            if (!objName.Contains("/"))
            { //might not have full path, so check if in dictionary
                if (fullnames.ContainsKey(objName))
                {
                    fullname = fullnames[objName];
                }
            }

            //print("=========================================Object FULLNAME = "+ fullname);
            GameObject go = GameObject.Find(fullname);
            if (!fullname.Contains("/"))
            { //get full name with path and add to dictionary
                fullname = (string.Join("/", go.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray()));
                fullnames[objName] = fullname;
            }

            if (!go)
            {
                print("Object " + objName + " not found.");
                retval = false;
            }
            retval = processObjectCommand(go, command, cparams);
        }
        return retval;
    }

    //Send a GameObject command to appropriate GameObject(s)
    bool processObjectCommand(GameObject go, string command, string cparams)
    {
        //get GameObject name to send command to    
        if (go == null)
        {
            print("GM: ProcessObjectCommand: Object missing for command " + command + " " + cparams);
            return false;
        }
        print("GM: Sending command " + command + " to " + go.name + " with params " + cparams);

        //get our message handler component from the Game Object
        ObjectMessageHandler mhand = go.GetComponent<ObjectMessageHandler>();
        if (!mhand)
        {
            print("Object " + go.name + " missing message handler.");
            return false;
        }


        //evaluate expressions
        //ParamData res;
        // Commenting out ParamData res as it is never called - TAR 1/6/2021
        if (command != null && command[0] == '$')
            command = ep.EvaluateParam(command);
        if (cparams != null && cparams[0] == '$')
            cparams = ep.EvaluateParam(cparams);
        //pass command and parameters to message handler
        bool result = mhand.HandleMessage(command, cparams); //commands return BOOL for IF statements
        return result;
    }

    void Update()
    {

    }

    void OnGUI()
    {
        GUI.contentColor = new Color(1.0f, 1.0f, 1f);

        GUIStyle style = new GUIStyle(GUI.skin.textArea);

        style.fontSize = Screen.height / 40; //change the font size 
        if (GUIcommandLine != null)
        {

            GUI.TextArea(new Rect(10, 10, Screen.width / 2, Screen.height / 5), GUIlastcommandLine + "\n" + GUIcommandLine, style);
            //           GUI.Label(new Rect(10, 10, 100, 20), cs.commandLine );

        }
        GUI.contentColor = new Color(1.0f, 1.0f, 0f);

        //GUIStyle style = new GUIStyle(GUI.skin.textArea);

        int border = 10;
        float dialogWidth = 0.8f;
        style.fontSize = Screen.height / 10; //change the font size 
        if (Prompt1 != null && Prompt1 != "")
        {
            GUI.TextArea(new Rect(border, Screen.height * .4f, Screen.width * dialogWidth, Screen.height / 5), Prompt1, style);
        }
        style.fontSize = Screen.height / 10; //change the font size 
        if (Prompt2 != null && Prompt2 != "")
        {
            GUI.TextArea(new Rect(Screen.width * (1 - dialogWidth) - border, Screen.height * .6f, Screen.width * dialogWidth - border, Screen.height / 5), Prompt2, style);
            //           GUI.Label(new Rect(10, 10, 100, 20), cs.commandLine ); 
        }



        style.fontSize = Screen.height / 10; //change the font size 
        if (Prompt != null && Prompt != "")
        {
            GUI.TextArea(new Rect(0, Screen.height * .8f, Screen.width, Screen.height / 5), Prompt, style);
            //           GUI.Label(new Rect(10, 10, 100, 20), cs.commandLine );
        }
    }

}

/////////////////////////////////////////////////////////////////////////////////////////
// Game Manager command parsing/handling
/////////////////////////////////////////////////////////////////////////////////////////


//AddSafe is a dictionary helper class to add extension that overrides duplcate entries
static class Extensions
{
    //gets the rest of "all" after location of "match" in all.
    public static string StringAfter(this string all, string match)
    {
        int start = 0;
        int end = all.Length;
        int count = end - start;
        int at = all.IndexOf(match, start, count);
        Debug.Log("StringAfter: " + all + "," + match + ", at=" + at);
        if (at == -1) return "";
        start = at + match.Length + 1;

        //Trim off end comments like this  #this is a comment
        //print("cs.commandLine cparams starts at "+ start);
        return all.Substring(start);
    }
    public static void AddSafe(this Dictionary<string, float> dictionary, string key, float value)
    {
        if (!dictionary.ContainsKey(key))
            dictionary.Add(key, value);
        else
        {
            dictionary[key] = value;
        }
    }
    public static void AddSafe(this Dictionary<string, int> dictionary, string key, int value)
    {
        if (!dictionary.ContainsKey(key))
            dictionary.Add(key, value);
        else
        {
            dictionary[key] = value;
        }
    }
}
