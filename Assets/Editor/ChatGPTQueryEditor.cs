using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ChatGPTQueryEditor : EditorWindow
{
    private string userInput;
    private string chatGptResponse;
    private string apiKey = "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";//"YOUR_API_KEY";
    private string apiEndpoint = "https://api.openai.com/v1/chat/completions";
    private UnityWebRequest request;
    private bool isRequestInProgress;

    [MenuItem("Window/ChatGPT Query Editor")]
    public static void ShowWindow()
    {
        GetWindow<ChatGPTQueryEditor>("ChatGPT Query Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("ChatGPT Query Editor", EditorStyles.boldLabel);

        userInput = EditorGUILayout.TextField("User Input", userInput);

        if (GUILayout.Button("Send Request"))
        {
            SendChatGPTRequest(userInput + " in Unity");
        }

        EditorGUILayout.Space();

        GUILayout.Label("ChatGPT Response", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(chatGptResponse, GUILayout.MinHeight(100f));
    }

    private void Update()
    {
        if (isRequestInProgress && request.isDone)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                ResponsePayload response = JsonUtility.FromJson<ResponsePayload>(request.downloadHandler.text);
                chatGptResponse = response.choices[0].message.content;
                HandleChatGPTResponse(response.choices[0].message.content);
            }
            else
            {
                Debug.LogError("ChatGPT Request Failed: " + request.error);
            }

            isRequestInProgress = false;
            request.Dispose();
            request = null;
        }
    }

    private void SendChatGPTRequest(string message)
    {
        if (isRequestInProgress)
        {
            Debug.LogWarning("ChatGPT request is already in progress.");
            return;
        }

        RequestPayload payload = new RequestPayload
        {
            model = "gpt-3.5-turbo",
            messages = new List<Message>
            {
                new Message { role = "user", content = message }
            },
            temperature = 0.7f
        };

        string payloadJson = JsonUtility.ToJson(payload);

        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payloadJson);

        request = new UnityWebRequest(apiEndpoint, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(payloadBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        EditorApplication.update += SendRequestOnMainThread;
        isRequestInProgress = true;
    }

    private void SendRequestOnMainThread()
    {
        EditorApplication.update -= SendRequestOnMainThread;
        request.SendWebRequest();
    }

    [System.Serializable]
    private class RequestPayload
    {
        public string model;
        public List<Message> messages;
        public float temperature;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    private class ResponsePayload
    {
        public List<Choice> choices;
    }

    [System.Serializable]
    private class Choice
    {
        public float finish_reason;
        public Message message;
    }




    private void HandleChatGPTResponse(string response)
    {
        // Handle the response from ChatGPT and perform object creation, modification, script creation, or modification based on the response

        // Example response handling
        if (response.Contains("create object"))
        {
            CreateObjectFromResponse(response);
        }
        else if (response.Contains("modify object"))
        {
            ModifyObjectFromResponse(response);
        }
        else if (response.Contains("create script"))
        {
            CreateScriptFromResponse(response);
        }
        else if (response.Contains("modify script"))
        {
            ModifyScriptFromResponse(response);
        }
    }

    private void CreateObjectFromResponse(string response)
    {
        // Parse the response and extract relevant object creation information

        // Example: Create a cube GameObject
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0f, 0f, 0f);

        // Example: Modify the cube based on the response
        // ...
    }

    private void ModifyObjectFromResponse(string response)
    {
        // Parse the response and extract relevant object modification information

        // Example: Find the object to modify
        GameObject objectToModify = GameObject.Find("ObjectName");

        if (objectToModify != null)
        {
            // Example: Modify the object based on the response
            // ...
        }
    }

    private void CreateScriptFromResponse(string response)
    {
        // Parse the response and extract relevant script creation information

        // Example: Create a new C# script file
        string scriptName = "MyCustomScript.cs";
        string scriptContent = @"
using UnityEngine;

public class MyCustomScript : MonoBehaviour
{
    private void Start()
    {
        // Add your custom script logic here
        Debug.Log(""Custom script attached!"");
    }
}
";
        CreateScriptFile(scriptName, scriptContent);

        // Example: Modify the script based on the response
        // ...
    }

    private void ModifyScriptFromResponse(string response)
    {
        // Parse the response and extract relevant script modification information

        // Example: Find the script to modify
        UnityEditor.MonoScript scriptToModify = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>("Assets/MyCustomScript.cs");

        if (scriptToModify != null)
        {
            // Example: Modify the script based on the response
            // ...
        }
    }

    // Other functions and code in the ChatGPTEditor script...

    private void CreateScriptFile(string filename, string scriptContent)
    {
        // Specify the file path
        string filePath = "Assets/" + filename;

        // Create the script file
        System.IO.File.WriteAllText(filePath, scriptContent);

        // Refresh the AssetDatabase to make sure Unity recognizes the new script file
        UnityEditor.AssetDatabase.Refresh();
    }

}
