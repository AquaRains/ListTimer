﻿using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Timer = System.Timers.Timer;

namespace ListTimer
{
    /// <summary>
    /// TimerObject.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CustomTimerControl : UserControl, IDisposable
    {
        Timer timer;
        ItemCollection ParentList { get; set; }

        /// <summary>
        /// 비트배열로 비트 연산으로 플래그 관리
        /// </summary>
        [Flags]
        public enum TimerState
        {
            None = 0,
            Cleared = 1 << 0,
            First = 1 << 1,
            Paused = 1 << 2,
            Running = 1 << 3,
            All = int.MaxValue
        }

        public TimerState State { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }
        public TimeSpan CurrentRemainedTime { get; set; }
        public TimeSpan SetRemainedTime { get; set; }
        public long StartTimeTicks { get; private set; }

        /// <summary>
        /// 하 원래 List같은걸로 가져와서 해야하는데..그냥 귀찮으니까 ItemCollection으로 받아서 와야겠다.
        /// 정석대로라면 ControlCollection류를 MinWindow에서 선언한다음,
        /// 그걸 ListConstrol에 DataSource로 사용하고 , 여기서 그 Collection을 받아와서 사용해야 좀 더 바람직하지 싶다.
        /// </summary>
        public CustomTimerControl(ItemCollection items)
        {
            ParentList = items;
            State = (TimerState.First | TimerState.Cleared);
        }

        public CustomTimerControl(ItemCollection items, TimeSpan time, Timer timer) : this(items)
        {
            InitializeComponent();
            SetRemainedTime = time;
            this.timer = timer;
            this.timer.Elapsed += Timer_Elapsed;
        }


        public void doStart()
        {
            BtnStart_Click(null, null);
        }
        //ElapsedTime값이 0 이하일때 발생되는 이벤트입니다.
        public event EventHandler OnTimerGoZero;

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if ((State & (TimerState.First | TimerState.Cleared)) != 0)
            {
                StartTimeTicks = System.DateTime.Now.Ticks;
                State = TimerState.Running;
            }
            else
            if ((State & (TimerState.Paused & TimerState.Cleared)) != 0)
            {
                StartTimeTicks = System.DateTime.Now.Ticks;
                State = TimerState.Running;
            }
            else
            if ((State & (TimerState.Paused & ~TimerState.Cleared)) != 0)
            {
                //쉰 시간만큼 지난 시간값을 되돌리기
                // ElapsedTime = TimeSpan.FromTicks(DateTime.Now.Ticks - StartTimeTicks);              //기준시간 다시 변경
                // StartTimeTicks = DateTime.Now.Ticks;
                StartTimeTicks = (DateTime.Now.Ticks - ElapsedTime.Ticks);
                //ElapsedTime = TimeSpan.FromTicks(0);

                State &= ~TimerState.Paused;
                State |= TimerState.Running;
            }

            State &= ~TimerState.Cleared;

            State |= TimerState.Running;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(Timer_ToInvoke);
            }
            catch(OperationCanceledException)
            {
                //취소되어봤자 메인윈도에 해가 될 일은 없다.
                return;
            }
            catch(Exception)
            {
                throw;
            }


        }

        private void Timer_ToInvoke()
        {
            //일단 남은 시간을 txtbox에 표시
            TimerTime.Text = $"{CurrentRemainedTime:hh\\:mm\\:ss\\.fff}";

            if((State & (TimerState.Running & ~TimerState.Paused)) != 0)
            {
                //시작시간은 과거이므로 항상 현재시각보다 작은값이지.
                ElapsedTime = TimeSpan.FromTicks(DateTime.Now.Ticks - StartTimeTicks);
                CurrentRemainedTime = SetRemainedTime - ElapsedTime;
                if(CurrentRemainedTime.Ticks < 0)
                {
                    Logic_stop();
                    OnTimerGoZero?.Invoke(this, null);  //이벤트 핸들러 호출
                }
            }
        }


        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            ParentList.Remove(this);
            State = TimerState.None | TimerState.Cleared | TimerState.Paused;
            Dispose();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            State = TimerState.Paused;
            StartTimeTicks = DateTime.Now.Ticks;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Logic_stop();
        }

        private void Logic_stop()
        {
            State = TimerState.Cleared | TimerState.Paused;
            CurrentRemainedTime = SetRemainedTime;
        }


        /// <summary>
        /// 지금 현재 문제가 되는 부분이라 임시로 땜빵해서 집어넣어서 테스트중인 부분인데, 솔직히 쓸모가 있는지는모르겠다.
        /// 09-01 수정 : timer의 참조 뿐 아니라 timer가 갖고있던 이벤트핸들러 구독을 함께 취소하니 의존성이 떨어져나갔다.
        /// </summary>
        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Logic_stop();
                    State = TimerState.Cleared;

                    timer.Elapsed -= this.Timer_Elapsed;
                    timer = null;

                }

                disposedValue = true;
                GC.ReRegisterForFinalize(this);
                GC.WaitForPendingFinalizers();
            }
        }

        ~CustomTimerControl()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}