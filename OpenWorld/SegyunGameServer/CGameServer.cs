using NetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SegyunGameServer
{
    class CGameServer
    {
        private object operation_lock;
        private Queue<CPacket> user_operations;

        // 로직은 하나의 스레드로만 처리
        private Thread logic_thread;
        private AutoResetEvent loop_event;


        //----------------------------------------------------------------
        // 게임 로직 처리 관련 변수들.
        //----------------------------------------------------------------
        // 게임방을 관리하는 매니저.
        public CGameRoomManager room_manager { get; set; }

        // 매칭 대기 리스트.
        private List<CGameUser> matching_waiting_users;

        public CGameServer()
        {
            operation_lock = new object();
            loop_event = new AutoResetEvent(false);
            user_operations = new Queue<CPacket>();

            // 게임 로직 관련
            room_manager = new CGameRoomManager();
            matching_waiting_users = new List<CGameUser>();

            logic_thread = new Thread(gameloop);
            logic_thread.Start();
        }

        /// <summary>
		/// 게임 로직을 수행하는 루프.
		/// 유저 패킷 처리를 담당한다.
		/// </summary>
		void gameloop()
        {
            while (true)
            {
                CPacket packet = null;
                lock (operation_lock)
                {
                    if (user_operations.Count > 0)
                    {
                        packet = user_operations.Dequeue();
                    }
                }

                if (packet != null)
                {
                    // 패킷 처리
                    process_receive(packet);
                }

                // 더이상 처리할 패킷이 없으면 스레드 대기
                if (user_operations.Count > 0)
                {
                    loop_event.WaitOne();
                }
            }
        }

        public void enqueue_packet(CPacket packet, CGameUser user)
        {
            lock (operation_lock)
            {
                user_operations.Enqueue(packet);
                loop_event.Set();
            }
        }

        private void process_receive(CPacket msg)
        {
            // user msg filter 체크
            msg.owner.process_user_operation(msg);
        }

        /// <summary>
        /// 유저로부터 매칭 요청이 왔을 때 호출됨.
        /// </summary>
        /// <param name="user">매칭을 신청한 유저 객체</param>
        public void matching_req(CGameUser user)
        {
            // 매칭 대기 리스트에 추가
            matching_waiting_users.Add(user);

            // 2명이 모이면 매칭 성공
            if (matching_waiting_users.Count == 2)
            {
                // 게임방 생성
                room_manager.create_room(matching_waiting_users[0], matching_waiting_users[1]);

                // 매칭 대기 리스트 삭제
                matching_waiting_users.Clear();
            }
        }

        public void user_disconnected(CGameUser user)
        {
            if (matching_waiting_users.Contains(user))
            {
                matching_waiting_users.Remove(user);
            }
        }
    }
}
