// RoomGenerationTest.cs
// Add Room scene to current scene, populate room as needed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Networking;  //Use this if WWW is obsolete in Unity version

public class RoomObject
{
    public enum ObjType { Bed, Wall, Rack, Trolley, Screen };
    private ObjType _objectType;
    public ObjType objectType {
        get { return _objectType; }
        set { _objectType = value; }
    }
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
    private RoomObject _subObject;
    public RoomObject subObject {
        get { return _subObject; }
        set { _subObject = value; }
    }
    private AsyncOperationHandle<GameObject> _handle;
    public AsyncOperationHandle<GameObject> handle {
        get { return _handle; }
        set { _handle = value; }
    }
    public void Spawn(Transform refTransform) {
        handle = Addressables.InstantiateAsync(objectAsset, refTransform.position, refTransform.localRotation);
        handle.Completed += handle => { gameObject = handle.Result; Debug.Log("GOT A GAME OBJECT: " + gameObject.name); };
    }
}

public class RoomGenerationTest : MonoBehaviour
{
    public string[] setupFiles; //contains list of Scenario Setup files 
    public List<RoomObject> roomObjects = new List<RoomObject>();
    private Scene roomScene;
 //   private Dictionary<string, Transform> locationDictionary;

    private static RoomGenerationTest roomGenerator;

    public static RoomGenerationTest instance
    {
        get
        {
            if (!roomGenerator)
            {
                roomGenerator = FindObjectOfType(typeof(RoomGenerationTest)) as RoomGenerationTest;
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

    // Initialize Room Generator
    void Init()
    {
    //       if (locationDictionary == null)
    //       {
    //           locationDictionary = new Dictionary<string, Transform>();
    //       }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(executeSceneSetupFiles(setupFiles));
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
        string roomToLoad = "";
        char[] separators = new char[] { ' ', '\t' };

        for (int i = 0; i < commands.Length; i++)
        {
            //print("LEVEL GEN: command: " + commands[i]);
            string locationName, locationNum, assetName;
            // Trim to remove extra whitespace at start/end
            commands[i].Trim();
            //split up cs.commandLine into location and asset
            string[] splitArray = commands[i].Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            // Don't try to parse a blank line caused by extra carriage return or empty fields
            if (splitArray.Length > 2)
            {
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
                    instance.roomObjects.Add(temp);
                    if (splitArray.Length > 5)
                    {
                        // Process subasset located on this asset
                        string subLocName, subLocNum, subAssetName;
                        subLocName = splitArray[3].TrimEnd(System.Environment.NewLine.ToCharArray());
                        subLocNum = splitArray[4].TrimEnd(System.Environment.NewLine.ToCharArray());
                        subAssetName = splitArray[5].TrimEnd(System.Environment.NewLine.ToCharArray());
                        print("LEVEL GEN: sub object loc = " + subLocName + subLocNum + " | asset = " + subAssetName + " (" + subAssetName.Length + " chars)");
                        if (subAssetName.Length > 0) {
                            RoomObject subTemp = new RoomObject();
                            subTemp.locName = subLocName;
                            subTemp.locNum = int.Parse(subLocNum);
                            subTemp.objectAsset = subAssetName;
                            temp.subObject = subTemp;
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
        for (int i = 0; i < instance.roomObjects.Count; i++)
        {
            print("LEVEL GEN SET THE SCENE: " + instance.roomObjects[i].locName + instance.roomObjects[i].locNum);
            switch (instance.roomObjects[i].locName)
            {
                case "Bed":
                    instance.roomObjects[i].Spawn(roomScript.bedLocs[instance.roomObjects[i].locNum - 1]);
                    break;
                case "Wall":
                    instance.roomObjects[i].Spawn(roomScript.wallLocs[instance.roomObjects[i].locNum - 1]);
                    break;
                case "BedsideHead":
                    instance.roomObjects[i].Spawn(roomScript.bedsideHeadLocs[instance.roomObjects[i].locNum - 1]);
                    break;
                case "BedsideMid":
                    instance.roomObjects[i].Spawn(roomScript.bedsideMidLocs[instance.roomObjects[i].locNum - 1]);
                    break;
                case "BedsideFoot":
                    instance.roomObjects[i].Spawn(roomScript.bedsideFootLocs[instance.roomObjects[i].locNum - 1]);
                    break;
                case "Cart":
                    instance.roomObjects[i].Spawn(roomScript.cartLocs[instance.roomObjects[i].locNum - 1]);
                    break;
                case "Screen":
                    instance.roomObjects[i].Spawn(roomScript.screenLocs[instance.roomObjects[i].locNum - 1]);
                    break;
                default:
                    print("LEVEL GEN: location for " + instance.roomObjects[i].objectAsset + " not found");
                    break;
            }

        }
    }

    public static void SetTheRack(RackScript rackScript)
    {
        for (int i = 0; i < instance.roomObjects.Count; i++)
        {
            if (instance.roomObjects[i].subObject != null)
            {
                print("LEVEL GEN SET THE RACK: Looking for " + rackScript.gameObject.name);
                if (instance.roomObjects[i].gameObject == rackScript.gameObject)
                {
                    print("LEVEL GEN SET THE RACK FOUND: " + instance.roomObjects[i].gameObject.name + "| sub object: " + instance.roomObjects[i].subObject.objectAsset);
                    switch (instance.roomObjects[i].subObject.locName)
                    {
                        case "InfusionBag":
                            instance.roomObjects[i].subObject.Spawn(rackScript.infusionBagLocs[instance.roomObjects[i].subObject.locNum - 1]);
                            break;
                        default:
                            print("LEVEL GEN: location for " + instance.roomObjects[i].subObject.objectAsset + " not found");
                            break;
                    }
                }
            }
        }
    }
}
