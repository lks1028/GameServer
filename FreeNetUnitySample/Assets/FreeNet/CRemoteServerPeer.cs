﻿using NetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeNetUnity
{
    public class CRemoteServerPeer : IPeer
    {
        public CUserToken token { get; private set; }
        WeakReference freenet_eventmanager;

        public CRemoteServerPeer(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        public void set_eventmanager(CFreeNetEventManager event_manager)
        {
            freenet_eventmanager = new WeakReference(event_manager);
        }

        /// <summary>
		/// 메시지를 수신했을 때 호출된다.
		/// 파라미터로 넘어온 버퍼는 워커 스레드에서 재사용 되므로 복사한 뒤 어플리케이션으로 넘겨준다.
		/// </summary>
		/// <param name="buffer"></param>
        public void on_message(Const<byte[]> buffer)
        {
            // 버퍼를 복사한 뒤 CPacket클래스로 감싼 뒤 넘겨준다.
            // CPacket클래스 내부에서는 참조로만 들고 있는다.
            byte[] app_buffer = new byte[buffer.Value.Length];
            Array.Copy(buffer.Value, app_buffer, buffer.Value.Length);
            CPacket msg = new CPacket(app_buffer, this);
            (freenet_eventmanager.Target as CFreeNetEventManager).enqueue_network_message(msg);
        }

        public void on_removed()
        {
            (freenet_eventmanager.Target as CFreeNetEventManager).enqueue_network_event(NETWORK_EVENT.disconnected);
        }

        public void send(CPacket msg)
        {
            token.send(msg);
        }

        public void disconnect()
        {
            throw new NotImplementedException();
        }

        public void process_user_operation(CPacket msg)
        {
            throw new NotImplementedException();
        }
    }
}