using JetBrains.Annotations;
using Microsoft.CognitiveServices.Speech.Transcription;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;


public class GptTurboScript : MonoBehaviour
{
    enum EGPT
    {
        OPENAI,
        DEEP_SEEK,
        QWEN
    }
    
    EGPT m_eGPT = EGPT.DEEP_SEEK;

    /// <summary>
    /// api URL
    /// </summary>
    //private string m_ApiUrl = "https://api.openai-proxy.com/v1/chat/completions";
    //private string m_ApiUrl = "https://api.openai.com/v1/chat/completions";
    private string m_ApiUrl = "https://api.deepseek.com/chat/completions";
    //private string m_ApiUrl = "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation";
    /// <summary>
    /// gpt-3.5-turbo
    /// </summary>
    //private string m_gptModel = "gpt-3.5-turbo";
    private string m_gptModel = "deepseek-chat";
    //private string m_gptModel = "qwen-max";
    /// <summary>
    /// 缓存对话
    /// </summary>
    [HideInInspector][SerializeField]public List<SendData> m_DataList = new List<SendData>();
    /// <summary>
    /// AI人设
    /// </summary>
    [HideInInspector]public string m_Prompt;

    private void Start()
    {
        // 注册 OnGameSettingChanged 事件
        GameSettingsEvent.OnGameSettingChanged += SetPrompt;
        m_Prompt = GameSettingsEntity.Instance.Persona;
        // 运行时添加人设
        m_DataList.Add(new SendData("system", m_Prompt));
    }

    public void SetPrompt(){
        m_Prompt = GameSettingsEntity.Instance.Persona;
    }

    /// <summary>
    /// 调用接口
    /// </summary>
    /// <param name="_postWord"></param>
    /// <param name="_openAI_Key"></param>
    /// <param name="_callback"></param>
    /// <returns></returns>
    public IEnumerator GetPostData(string _postWord,string _openAI_Key, System.Action<string> _callback)
    {

        //缓存发送的信息列表
        m_DataList.Add(new SendData("user", _postWord));

        using (UnityWebRequest request = new UnityWebRequest(m_ApiUrl, "POST"))
        {
            string _jsonText = "";
            if (m_eGPT == EGPT.QWEN)
            {
                QWenPostData qwenPostData = new QWenPostData
                {
                    model = m_gptModel,
                    input = new QWenSendData
                    {
                        messages = m_DataList
                    }
                };
                _jsonText = JsonUtility.ToJson(qwenPostData);
            }
            else
            {
                PostData _postData = new PostData
                {
                    model = m_gptModel,
                    messages = m_DataList
                };
                _jsonText = JsonUtility.ToJson(_postData);
            }
            
            Debug.Log($"{_jsonText}");
            //string _jsonText = JsonUtility.ToJson(_postData);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", _openAI_Key));
            if (m_eGPT == EGPT.QWEN)
            {
                request.SetRequestHeader("X-DashScope-SSE","enable");
            }


            yield return request.SendWebRequest();
            if (request.responseCode == 200)
            {
                string _msg = request.downloadHandler.text;
                MessageBack _textback = JsonUtility.FromJson<MessageBack>(_msg);
                if (_textback != null && _textback.choices.Count > 0)
                {

                    string _backMsg = _textback.choices[0].message.content;
                    //添加记录
                    m_DataList.Add(new SendData("assistant", _backMsg));
                    _callback(_backMsg);
                }
            }
            else{
                Debug.Log($"ChatGPT_responseCode : {request.responseCode}");
            }
        }


    }

    #region 数据包
    
    [Serializable]
    public class QWenPostData
    {
        public string model;
        public QWenSendData input;
    }
    
    [Serializable]
    public class QWenSendData
    {
        public List<SendData> messages;
    }

    [Serializable]
    public class PostData
    {
        public string model;
        public List<SendData> messages;
    }

    [Serializable]
    public class SendData
    {
        public string role;
        public string content;
        public SendData() { }
        public SendData(string _role,string _content) {
            role = _role;
            content = _content;
        }

    }
    
    
    [Serializable]
    public class MessageBack
    {
        public string id;
        public string created;
        public string model;
        public List<MessageBody> choices;
    }
    [Serializable]
    public class MessageBody
    {
        public Message message;
        public string finish_reason;
        public string index;
    }
    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    #endregion


}

