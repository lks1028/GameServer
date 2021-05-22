using FreeNetUnity;
using NetService;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameMain : MonoBehaviour {

	private string input_text;
	private List<string> received_texts;
	private CNetworkManager network_manager;

	Vector2 currentScrollPos = new Vector2();

	void Awake()
    {
		input_text = "";
		received_texts = new List<string>();
		network_manager = GameObject.Find("NetworkManager").GetComponent<CNetworkManager>();
    }

	public void on_receive_chat_msg(string text)
    {
		received_texts.Add(text);
		currentScrollPos.y = float.PositiveInfinity;
    }

	private void OnGUI()
    {
        // Received text.  
        GUILayout.BeginVertical();
        currentScrollPos = GUILayout.BeginScrollView(currentScrollPos,
            GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width),
            GUILayout.MaxHeight(Screen.height - 100), GUILayout.MinHeight(Screen.height - 100));

        foreach (string text in this.received_texts)
        {
            GUILayout.BeginHorizontal();
            GUI.skin.label.wordWrap = true;
            GUILayout.Label(text);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();


        // Input.  
        GUILayout.BeginHorizontal();
        this.input_text = GUILayout.TextField(this.input_text, GUILayout.MaxWidth(Screen.width - 100), GUILayout.MinWidth(Screen.width - 100),
            GUILayout.MaxHeight(50), GUILayout.MinHeight(50));

        if (GUILayout.Button("Send", GUILayout.MaxWidth(100), GUILayout.MinWidth(100), GUILayout.MaxHeight(50), GUILayout.MinHeight(50)))
        {
            CPacket msg = CPacket.create((short)PROTOCOL.CHAT_MSG_REQ);
            msg.push(this.input_text);
            this.network_manager.send(msg);

            this.input_text = "";
        }
        GUILayout.EndHorizontal();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
