using System.Linq;
using UnityEngine;

public class GameManager : MyGameManager
{
    // GameManager singleton
    public static GameManager instance = null;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        ep = new ExpressionParser();
    }

    public static void StartScenario()
    {
        Debug.Log("GAME MANAGER: Got Start Scenario message");
        GameManager.instance.ExecuteScenario();
    }

    private void ExecuteScenario()
    {
        StartCoroutine(exectueScenarioFiles(CommandFiles));
    }

    //Process commands meant for multiple objects
    //Supports ANY and ALL patterns
    //e.g.: Lights/ALL
    protected override bool processObjectsCommand(string objName, string command, string cparams)
    {
        GameObject[] Objects;
        bool retval;

        string[] splitArray = objName.Split('/');
        int last = splitArray.Length - 1;
        string endObject = splitArray[last];
        if ((endObject == "ALL") || (endObject == "ANY"))
        {
            Debug.Log("GAME MANAGER: Sending command " + command + " to all/any");
            //ADD FindGameObjects function to find ALL children objects of objName, 
            //i.e. Room1/Lights/ALL should find:
            //  Room1/Lights/Light1 and Room1/Lights/Light2
            Objects = FindGameObjects(objName);

            if (endObject == "ANY")
            {

                //ADD CODE TO HANDLE "ANY" 
                int randI = UnityEngine.Random.Range(0, Objects.Length);
                Debug.Log("GAME MANAGER: ANY picked object " + Objects[randI].name);
                retval = processObjectCommand(Objects[randI], command, cparams);
                //retval = true;

            }
            else
            {  //handle ALL
                retval = true;

                foreach (GameObject gameObj in Objects)
                {
                    Debug.Log("GAME MANAGER: ALL picked object " + gameObj.name);
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

            print("=========================================Object FULLNAME = " + fullname);
            GameObject go = null;

            // First, check for object name under level generator room object dictionary
            if (RoomGenerator.instance.roomObjects.ContainsKey(splitArray[0]))
            {
                if (objName.Contains("/"))
                {
                    // hierarchical name, search for child under parent array
                    // if fails, returns null
                    go = RoomGenerator.instance.roomObjects[splitArray[0]].GetSubObject(splitArray[1]).gameObject;
                }
                else {
                    // non-hierarchical name
                    go = RoomGenerator.instance.roomObjects[objName].gameObject;
                }
            }
            
            if (go == null)
            {
                go = GameObject.Find(objName);  //   /Room1/Enemy/ANY   or /Room/ANY/Enemy/ANY
            }

            if (!fullname.Contains("/"))
            { //get full name with path and add to dictionary
                fullname = (string.Join("/", go.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray()));
                fullnames[objName] = fullname;
            }

            if (!go)
            {
                Debug.Log("GAME MANAGER: Object " + objName + " not found.");
                retval = false;
            }
            retval = processObjectCommand(go, command, cparams);
        }
        return retval;
    }

    //Find children objects of objName for commands meant for multiple objects
    //Supports ANY and ALL patterns
    //e.g.: Lights/ALL
    //Returns list of children
    protected override GameObject[] FindGameObjects(string objName)
    {

        Debug.Log("GAME MANAGER: FindGameObjects for " + objName);
        int start = objName.Length - 1;
        int count = objName.Length;
        int at = objName.LastIndexOf("/", start, count);
        Debug.Log("GAME MANAGER: LastIndexOf / = " + at);
        objName = objName.Remove(at);
        Debug.Log("GAME MANAGER: Multiple Objects under " + objName + "");

        GameObject go;

        // First, check for object name under level generator room object dictionary
        if (RoomGenerator.instance.roomObjects.ContainsKey(objName))
        {
            go = RoomGenerator.instance.roomObjects[objName].gameObject;
        } else {
            go = GameObject.Find(objName);  //   /Room1/Enemy/ANY   or /Room/ANY/Enemy/ANY
        }

        m_gameObjects = new GameObject[go.transform.childCount];
        for (int i = 0; i < go.transform.childCount; i++)
        {
            m_gameObjects[i] = go.transform.GetChild(i).gameObject;
        }
        //TODO 
        return m_gameObjects;
    }

    public static GameObject FindThisObject(string objName)
    {
        Debug.Log("GAME MANAGER: NEED TO FIND GAME OBJECT FOR: " + objName);
        GameObject go = null;
        string[] splitArray = objName.Split('/');

        foreach (string name in splitArray) {
            Debug.Log("GAME MANAGER: TRYING TO FIND A: " + name);
        }

        // First, check for object name under level generator room object dictionary
        if (RoomGenerator.instance.roomObjects.ContainsKey(splitArray[0]))
        {
            if (objName.Contains("/"))
            {
                // hierarchical name, search for child under parent array
                // if fails, returns null
                go = RoomGenerator.instance.roomObjects[splitArray[0]].GetSubObject(splitArray[1]).gameObject;
            }
            else
            {
                // non-hierarchical name
                go = RoomGenerator.instance.roomObjects[objName].gameObject;
            }
        }

        if (go == null)
        {
            go = GameObject.Find(objName);
        }

        if (go == null) {
            Debug.Log("GAME MANAGER: =-=-= GAME OBJECT NOT FOUND =-=-=: " + objName);
        }
        return go;
    }
}
