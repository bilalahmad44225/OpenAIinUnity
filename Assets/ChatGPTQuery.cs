using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;


public class ChatGPTQuery : MonoBehaviour
{
    public InputField userInputField;
    public Text chatGptResponseText;
    public string apiKey = "YOUR_API_KEY";
    public string apiEndpoint = "https://api.openai.com/v1/chat/completions";

    private UnityWebRequest request;

    public void SendChatGPTRequest()
    {
        string userInput = userInputField.text;

        if (string.IsNullOrEmpty(userInput))
        {
            Debug.LogWarning("User input is empty.");
            chatGptResponseText.text= "User input is empty.";
            return;
        }

        RequestPayload payload = new RequestPayload
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new Message { role = "system", content = "You are a helpful assistant." },
                new Message { role = "user", content = userInput }
            }
        };

        string payloadJson = JsonUtility.ToJson(payload);

        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payloadJson);

        request = new UnityWebRequest(apiEndpoint, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(payloadBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        StartCoroutine(SendRequestCoroutine());
        chatGptResponseText.text = "ChatGPT Request Sent.\nWaiting for response. ";
    }

    private IEnumerator SendRequestCoroutine()
    {
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ResponsePayload response = JsonUtility.FromJson<ResponsePayload>(request.downloadHandler.text);
            chatGptResponseText.text = response.choices[0].message.content;
        }
        else
        {
            Debug.LogError("ChatGPT Request Failed: " + request.error);
            chatGptResponseText.text = "ChatGPT Request Failed: " + request.error;
        }

        request.Dispose();
        request = null;
    }

    [System.Serializable]
    private class RequestPayload
    {
        public string model;
        public Message[] messages;
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
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public float finish_reason;
        public Message message;
    }
}
