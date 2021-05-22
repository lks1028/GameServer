using FreeNetUnity;
using NetService;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CFreeNetUnityService : MonoBehaviour {

	private CFreeNetEventManager event_manager;

	// 연결된 게임 서버 객체
	private IPeer gameserver;

	// TCP 통신을 위한 서비스 객체
	private CNetworkService service;

	// 네트워크 상태 변경시 호출되는 델리게이트. 어플리케이션에서 콜백 메소드를 설정하여 사용한다.
	public delegate void StatusChangedHandler(NETWORK_EVENT status);
	public StatusChangedHandler appcallback_on_status_changed;

	// 네트워크 메세지 수신시 호출되는 델리게이트. 어플리케이션에서 콜백 메소드를 설정하여 사용한다.
	public delegate void MessageHandler(CPacket msg);
	public MessageHandler appcallback_on_message;

	void Awake()
    {
		CPacketBufferManager.initialize(10);
		event_manager = new CFreeNetEventManager();
    }

	public void connect(string host, int port)
    {
		// CNetworkService 객체는 메세지의 비동기 송,수신 처리를 수행한다.
		service = new CNetworkService();

		// endpoint 정보를 갖고있는 Connector 생성. 만들어둔 NetworkService 객체를 넣어준다.
		CConnector connector = new CConnector(service);

		// 접속 성공시 호출될 콜백 메소드 지정
		connector.connected_callback += on_connected_gameserver;
		IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
		connector.connect(endpoint);
    }

	/// <summary>  
	/// 접속 성공시 호출될 콜백 매소드.  
	/// </summary>  
	/// <param name="server_token"></param>
	void on_connected_gameserver(CUserToken server_token)
    {
		gameserver = new CRemoteServerPeer(server_token);
		((CRemoteServerPeer)gameserver).set_eventmanager(event_manager);

		// 유니티 어플리케이션으로 이벤트를 넘겨주기 위해 메니저 큐에 삽입
		event_manager.enqueue_network_event(NETWORK_EVENT.connected);
    }

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	/// <summary>  
	/// 네트워크에서 발생하는 모든 이벤트를 클라이언트에게 알려주는 역할을 Update에서 진행한다.  
	/// FreeNet엔진의 메시지 송수신 처리는 워커스레드에서 수행되지만 유니티의 로직 처리는 메인 스레드에서 수행되므로  
	/// 큐잉처리를 통하여 메인 스레드에서 모든 로직 처리가 이루어지도록 구성하였다.  
	/// </summary>  
	void Update () {
		
		// 수신된 메세지에 대한 콜백
		if (event_manager.has_message())
        {
			CPacket msg = event_manager.dequeue_network_message();
			if (appcallback_on_message != null)
            {
				appcallback_on_message(msg);
            }
        }

		// 네트워크 발생 이벤트에 대한 콜백
		if (event_manager.has_event())
        {
			NETWORK_EVENT status = event_manager.dequeue_network_event();
			if (appcallback_on_status_changed != null)
            {
				appcallback_on_status_changed(status);
            }
        }
	}

	public void send(CPacket msg)
    {
		gameserver.send(msg);
    }
}
