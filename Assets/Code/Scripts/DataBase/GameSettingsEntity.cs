using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class GameSettingsEntity : Singleton<GameSettingsEntity>
{
    // 目光跟随模式
    public int LookTargetMode { get; set; } // 0:鼠标 1:摄像头
    // 语言-声源
    public string Speaker { get; set; }
    // ChatGPT API
    public string ChatGPTAPI { get; set; }
    // Azure API
    public string AzureAPI { get; set; }
    // APISpace API
    public string APISpaceAPI { get; set; }
    // 人设
    public string Persona { get; set; }

    private GameSettingsEntity()
    {
        // 从json文件中读取游戏设置
        string readData;
        string fileUrl = Application.persistentDataPath + "\\GameSettings.json";
        if(File.Exists(fileUrl)){
            using (StreamReader sr = File.OpenText(fileUrl))
            {
                readData = sr.ReadToEnd();
                sr.Close();
            }
            // Debug.Log(readData);

            string[] keys = { "LookTargetMode", "Speaker", "ChatGPTAPI", "AzureAPI", "APISpaceAPI", "Persona" };
            int[] start_position = new int[6];
            for (int i = 0; i < keys.Length; i++)
            {
                start_position[i] = readData.IndexOf(keys[i]) + keys[i].Length + 2;
            }
            this.LookTargetMode = int.Parse(readData.Substring(start_position[0], 1));
            this.Speaker = readData.Substring(start_position[1] + 1, readData.IndexOf("\"", start_position[1] + 1) - start_position[1] - 1);
            this.ChatGPTAPI = readData.Substring(start_position[2] + 1, readData.IndexOf("\"", start_position[2] + 1) - start_position[2] - 1);
            this.AzureAPI = readData.Substring(start_position[3] + 1, readData.IndexOf("\"", start_position[3] + 1) - start_position[3] - 1);
            this.APISpaceAPI = readData.Substring(start_position[4] + 1, readData.IndexOf("\"", start_position[4] + 1) - start_position[4] - 1);
            this.Persona = readData.Substring(start_position[5] + 1, readData.IndexOf("\"", start_position[5] + 1) - start_position[5] - 1);
            // Debug.Log(this.LookTargetMode + "&" + this.Speaker + "&" + this.ChatGPTAPI + "&" + this.AzureAPI + "&" + this.APISpaceAPI + ".");
            
            //设置默认值
            this.LookTargetMode = 0;
            this.Speaker = "zh-CN-XiaoxiaoNeural";
            this.ChatGPTAPI = "sk-f3a5090a7c6e42be9f3184a075d7d974";
            this.AzureAPI = "894042222b1a41599b6f649698ed5ba2";
            this.APISpaceAPI = "ltd9jcxfh9jmvqhpqgam8i5sifv6ptly";
            //this.Persona = "请你扮演我的学妹，名字是桃花元子，按照傲娇学妹的说话习惯回答";
            this.Persona = "请你扮演一名经验丰富的英语老师，与中国学生进行对话，句子要简短。对于学生的每一句话都要判断是否有错误有的话给学生指出来，如果没有则直接回答学生的问题。所有的回复使用英语回答";
        }
        else{
            this.LookTargetMode = 0;
            this.Speaker = "zh-CN-XiaoxiaoNeural";
            this.ChatGPTAPI = "sk-f3a5090a7c6e42be9f3184a075d7d974";
            this.AzureAPI = "894042222b1a41599b6f649698ed5ba2";
            this.APISpaceAPI = "ltd9jcxfh9jmvqhpqgam8i5sifv6ptly";
            //this.Persona = "请你扮演我的学妹，名字是桃花元子，按照傲娇学妹的说话习惯回答";
            this.Persona = "请你扮演一名经验丰富的英语老师，与中国学生进行对话，句子要简短。对于学生的每一句话都要判断是否有错误有的话给学生指出来，如果没有则直接回答学生的问题。所有的回复使用英语回答";
                          // "Please act as an experienced English teacher and have an oral English conversation with a Chinese student." +
                          // "For each sentence, first judge whether there is any grammatical error, if there is, point out the grammatical error and then give the answer of the sentence.Response in English";
            string js = JsonConvert.SerializeObject(this);
            using (StreamWriter sw = new StreamWriter(fileUrl))
            {
                sw.Write(js);
                sw.Close();
                sw.Dispose();
            }
        }
    }
}
