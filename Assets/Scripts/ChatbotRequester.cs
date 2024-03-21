using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;


public class ChatbotRequester : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI txtChatHistory;
    [SerializeField]
    TMPro.TMP_InputField inptQuestion;
    [SerializeField]
    string ApiPath = "http://localhost:5000/predict";
    void Start()
    {
        //StartCoroutine(PostRequest("http://localhost:5000/predict", "{\"input\":\"Hello!\"}"));
        txtChatHistory.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendRequest();
        }
    }

    IEnumerator PostRequest(string url, string json)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            ReceiveResponse(request.downloadHandler.text);
        }
    }

    public void SendRequest()
    {
        string strRequest = inptQuestion.text;
        RequestMsg msg = new RequestMsg { input = strRequest};
        txtChatHistory.text += SwitchHyperTagMessage(strRequest, "User", "black");
        StartCoroutine(PostRequest(ApiPath, JsonUtility.ToJson(msg)));
    }
    
    void ReceiveResponse(string strData)
    {
        ResponseMsg msg = JsonUtility.FromJson<ResponseMsg>(strData);
        string response = msg.response;
        txtChatHistory.text += SwitchHyperTagMessage(response, "Agent", "blue");
        inptQuestion.text = "";
    }

    string SwitchHyperTagMessage(string strMsg, string userName, string color)
    {
        return $"<size=20><b>{userName}</b></size>\n<color={color}>{strMsg}</color>\n\n";
    }
}

[System.Serializable]
public class RequestMsg
{
    public string input;
}

[System.Serializable]
public class ResponseMsg
{
    public string response;
}
