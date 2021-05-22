using FreeNetUnity;
using NetService;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNetworkManager : MonoBehaviour {

	CFreeNetUnityService gameserver;

	void Awake()
    {
		// 네트워크 통신을 위해 CFreeNetUnityService 객체 추가
		gameserver = gameObject.AddComponent<CFreeNetUnityService>();

		// 상태 변화(접속, 끊김 등)를 통보 받을 델리게이트 설정
		gameserver.appcallback_on_status_changed += on_status_changed;

		// 패킷 수신 델리게이트 설정
		gameserver.appcallback_on_message += on_message;
    }

	// Use this for initialization
	void Start () {
		connect();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void connect()
    {
		gameserver.connect("127.0.0.1", 7979);
	}

	/// <summary>  
	/// 네트워크 상태 변경시 호출될 콜백 매소드.  
	/// </summary>  
	/// <param name="server_token"></param>
	private void on_status_changed(NETWORK_EVENT status)
    {
		switch (status)
        {
			// 접속 성공
			case NETWORK_EVENT.connected:
				Debug.Log("on connected");

				CPacket msg = CPacket.create((short)PROTOCOL.CHAT_MSG_REQ);
				msg.push("Hello");
				gameserver.send(msg);

				break;

			// 연결 끊김
			case NETWORK_EVENT.disconnected:
				Debug.Log("disconnectd");

				break;
        }
    }

	private void on_message(CPacket msg)
    {
		// 제일 먼저 프로토콜 아이디를 꺼내온다
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

		// 프로토콜에 따른 분기처리
		switch (protocol_id)
        {
			case PROTOCOL.CHAT_MSG_ACK:
				string text = msg.pop_string();
				GameObject.Find("GameMain").GetComponent<CGameMain>().on_receive_chat_msg(text);

				break;
        }
    }

	public void send(CPacket msg)
    {
		gameserver.send(msg);
    }
}
