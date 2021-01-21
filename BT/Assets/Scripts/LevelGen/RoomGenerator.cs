/** RoomGenerator.cs
 * <summary>
 * Adds Room scene to current scene and populates room as directed by room setup file.
 * </summary>
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Networking;  //Use this if WWW is obsolete in Unity version

/** <summary>
 * Class used for each object and sub-object in a room (furniture, medical equipment, etc.).
 * </summary>
 */
public class RoomObject
{
/*
    public enum ObjType { Bed, Wall, Rack, Trolley, Screen };
    private ObjType _objectType;
    public ObjType objectType {
        get { return _objectType; }
        set { _objectType = value; }
    }
*/

    private string _locName;
    public string locName {
        get { return _locName; }
        set { _locName = value; }
    }
    private int _locNum;
    public int locNum { 
        get { return _locNum; }
        set { _locNum = value; }
    }
    private Transform _locTransform;
    public Transform locTransform {
        get { return _locTransform; }
        set { _locTransform = value; }
    }
    private string _objectAsset;
    public string objectAsset {
        get { return _objectAsset; }
        set { _objectAsset = value; }
    }
    private GameObject _gameObject;
    public GameObject gameObject {
        get { return _gameObject; }
        set { _gameObject = value; }
    }

    private Dictionary<string, RoomObject> _subObjects = new Dictionary<string, RoomObject>();
    public RoomObject GetSubObject(string key)
    {
        RoomObject result = null;
        if (_subObjects.ContainsKey(key))
        {
            result = _subObjects[key];
        }
        return result;
    }
    public void SetSubObject(string key, RoomObject value)
    {
        if (_subObjects.ContainsKey(key))
        {
            _subObjects[key] = value;
        }
        else {
            _subObjects.Add(key, value);
        }
    }
    public Dictionary<string, RoomObject> GetAllSubObjects()
    {
        return _subObjects;
    }
    public int CountSubObjects()
    {
        return _subObjects.Count;
    }

    private AsyncOperationHandle<GameObject> _handle;
    public AsyncOperationHandle<GameObject> handle {
        get { return _handle; }
        set { _handle = value; }
    }

    /** <summary>
     * Spawn is used to handle instantiation of this object.
     * </summary>
     * <param name="refTransform">Reference to existing transform with same position/rotation where new object should instantiate.</param>
     * <param name="myParent">(optional) Parent object that spawning object should be made a child of. Default is null.</param>
     */
    public void Spawn(Transform refTransform, Transform myParent = null) {
        // If parent is null, InstantiateAsync() will default to instantiating unparented
        handle = Addressables.InstantiateAsync(objectAsset, refTransform.position, refTransform.rotation, myParent);
        handle.Completed += handle => { gameObject = handle.Result; Debug.Log("GOT A GAME OBJECT: " + gameObject.name); };
    }
}

public class RoomGenerator : MonoBehaviour
{
    public string[] setupFiles; /**< Array of level setup filenames*/
    public Dictionary<string, RoomObject> roomObjects = new Dictionary<string, RoomObject>(); /**< Keeps track of objects in the room*/
    private Scene roomScene; /**< Reference to room scene being added to the current scene*/

    private static RoomGenerator roomGenerator;

    public static RoomGenerator instance
    {
        get
        {
            if (!roomGenerator)
            {
                roomGenerator = FindObjectOfType(typeof(RoomGenerator)) as RoomGenerator;
                if (!roomGenerator)
                {
                    Debug.LogError("There must be one active Room Generation script on a GameObject in the scene.");
                }
                else
                {
                    roomGenerator.Init();
                }
            }
            return roomGenerator;
        }
    }

    /** <summary>Initialize Room Generator</summary>*/
    void Init()
    {
        // Nothing needed here so far
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(executeSceneSetupFiles(setupFiles));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            foreach (KeyValuePair<string, RoomObject> entry in instance.roomObjects)
            {
                Debug.Log("KEY: " + entry.Key + " | VALUE: " + entry.Value.gameObject.name);
                if (entry.Value.CountSubObjects() > 0)
                {
                    foreach (KeyValuePair<string, RoomObject> subEntry in entry.Value.GetAllSubObjects())
                    {
                        Debug.Log("== SUB OBJECT KEY: " + subEntry.Key + " | VALUE: " + subEntry.Value.gameObject.name);
                        foreach (KeyValuePair<string, RoomObject> subSubEntry in subEntry.Value.GetAllSubObjects())
                        {
                            Debug.Log("====== SUB SUB OBJECT KEY: " + subSubEntry.Key + " | VALUE: " + subSubEntry.Value.gameObject.name);
                        }

                    }
                }
            }
        }
    }

    //loads local or remote file
    IEnumerator executeSceneSetupFiles(string[] fileNames)
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
            yield return StartCoroutine(ParseCommands(linesInFile));
        }
    }

    private IEnumerator ParseCommands(string[] commands)
    {
        string roomToLoad = ""; /**< Name of new room scene in string format.*/
        char[] separators = new char[] { ' ', '\t' };

        for (int i = 0; i < commands.Length; i++)
        {
            print("LEVEL GEN: command: " + commands[i] + "|");
            string locationName, locationNum, assetName;
            // Trim to remove extra whitespace at start/end
            commands[i] = commands[i].TrimEnd('\r');
            //commands[i].Replace(System.Environment.NewLine, "");
            print("LEVEL GEN: command: " + commands[i] + "|");

            //split up cs.commandLine into location and asset
            string[] splitArray = commands[i].Split(separators, System.StringSplitOptions.None);
            // Don't try to parse a blank line caused by extra carriage return or empty fields
            if (splitArray.Length > 2)
            {
                string tempString = "";
                foreach (string s in splitArray) {
                    tempString += s;
                    tempString += "|";
                }
                Debug.Log(tempString);

                locationName = splitArray[0].TrimEnd(System.Environment.NewLine.ToCharArray());
                locationNum = splitArray[1].TrimEnd(System.Environment.NewLine.ToCharArray());
                assetName = splitArray[2].TrimEnd(System.Environment.NewLine.ToCharArray());
                print("LEVEL GEN: loc = " + locationName + locationNum + " | asset = " + assetName + " (" + assetName.Length + " chars)");

                if (locationName.ToLower() == "room")
                {
                    // Load this room once all commands are processed, get rid of carriage return/line feed
                    roomToLoad = assetName.TrimEnd(System.Environment.NewLine.ToCharArray());
                }
                else if (assetName.Length > 0)
                {
                    // Room content asset - load this after room is completely loaded and SetTheScene() is called
                    RoomObject temp = new RoomObject();
                    temp.locName = locationName;
                    temp.locNum = int.Parse(locationNum);
                    temp.objectAsset = assetName;
                    instance.roomObjects.Add(string.Concat(temp.locName, temp.locNum), temp);
                    if (splitArray.Length > 5)
                    {
                        // Process subassets located on this asset
                        for (int j = 3; j < splitArray.Length; j += 3)
                        {
                            string subLocName, subLocNum, subAssetName;
                            subLocName = splitArray[j].TrimEnd(System.Environment.NewLine.ToCharArray());
                            subLocNum = splitArray[j+1].TrimEnd(System.Environment.NewLine.ToCharArray());
                            subAssetName = splitArray[j+2].TrimEnd(System.Environment.NewLine.ToCharArray());
                            print("LEVEL GEN: sub object loc = " + subLocName + subLocNum + " | asset = " + subAssetName + " (" + subAssetName.Length + " chars)");
                            if (subAssetName.Length > 0)
                            {
                                RoomObject subTemp = new RoomObject();
                                subTemp.locName = subLocName;
                                subTemp.locNum = int.Parse(subLocNum);
                                subTemp.objectAsset = subAssetName;
                                temp.SetSubObject(string.Concat(subLocName, subLocNum), subTemp);
                            }
                        }
                    }
                }
                else
                {
                    print("LEVEL GEN: EMPTY ASSET FIELD - IGNORING");
                }
            } else
            {
                print("LEVEL GEN: INCOMPLETE COMMAND DETECTED - IGNORING");
            }
        }

        // Ready to load the room asset
        var parameters = new LoadSceneParameters(LoadSceneMode.Additive);
        if (roomToLoad != "") {
            roomScene = SceneManager.LoadScene(roomToLoad, parameters);
        }
        else {
            print("LEVEL GEN: NO ROOM SCENE GIVEN IN SETUP FILE");
            // Load a default scene if other room fails
            roomScene = SceneManager.LoadScene("RoomMedExam_base", parameters);
        }

        yield return null;
    }

    public static void SetTheScene(RoomScript roomScript)
    {
        foreach (KeyValuePair<string, RoomObject> entry in instance.roomObjects)
        {
            print("LEVEL GEN SET THE SCENE: " + entry.Value.locName + entry.Value.locNum);
            switch (entry.Value.locName)
            {
                case "Bed":
                    entry.Value.Spawn(roomScript.bedLocs[entry.Value.locNum - 1]);
                    break;
                case "Wall":
                    entry.Value.Spawn(roomScript.wallLocs[entry.Value.locNum - 1]);
                    break;
                case "BedsideHead":
                    entry.Value.Spawn(roomScript.bedsideHeadLocs[entry.Value.locNum - 1]);
                    break;
                case "BedsideMid":
                    entry.Value.Spawn(roomScript.bedsideMidLocs[entry.Value.locNum - 1]);
                    break;
                case "BedsideFoot":
                    entry.Value.Spawn(roomScript.bedsideFootLocs[entry.Value.locNum - 1]);
                    break;
                case "Cart":
                    entry.Value.Spawn(roomScript.cartLocs[entry.Value.locNum - 1]);
                    break;
                case "Screen":
                    entry.Value.Spawn(roomScript.screenLocs[entry.Value.locNum - 1]);
                    break;
                default:
                    // Find an object in the world with the given name as spawn location
                    GameObject destination = GameObject.Find(entry.Value.locName);
                    if (destination != null)
                    {
                        entry.Value.Spawn(destination.transform);
                    }
                    else
                    {
                        print("LEVEL GEN: location for " + entry.Value.objectAsset + " not found");
                    }
                    break;
            }

        }
    }

    public static void SpawnSubObjects(SubObjectParent subObjectParent)
    {
        foreach (KeyValuePair<string, RoomObject> entry in instance.roomObjects)
        {
            if (entry.Value.CountSubObjects() > 0)
            {
                print("--LEVEL GEN SPAWNING SUB OBJECTS: Looking for " + subObjectParent.gameObject.name);
                if (entry.Value.gameObject == subObjectParent.gameObject)
                {
                    foreach (KeyValuePair<string, RoomObject> subEntry in entry.Value.GetAllSubObjects())
                    {
                        print("--LEVEL GEN SPAWNABLE SUB OBJECT FOUND FOR: " + entry.Value.gameObject.name + "| sub object: " + subEntry.Value.objectAsset);
                        subEntry.Value.Spawn(subObjectParent.subObjectLocs[subEntry.Value.locNum - 1], entry.Value.gameObject.transform);
                    }
                }
            }
        }
    }
}
