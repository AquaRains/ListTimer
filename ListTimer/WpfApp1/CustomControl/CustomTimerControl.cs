using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Timer = System.Timers.Timer;

namespace ListTimer.CustomControl
{
    /// <summary>
    /// TimerObject.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CustomTimerControl : UserControl, IDisposable
    {
        private Timer _timer;
        private ItemCollection ParentList { get; set; }

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

        /// <summary>
        /// 이 타이머의 상태
        /// </summary>
        public TimerState State { get; private set; }

        /// <summary>
        /// 진행중이던 타이머의 남은 시간
        /// </summary>
        public TimeSpan DisplayTime { get; set; }
        /// <summary>
        /// 이 타이머가 최초 생성될때 세팅된 초기 남은 시간
        /// </summary>
        public TimeSpan Settime { get; set; }
        /// <summary>
        ///  타이머가 시작 시각 tick값
        /// </summary>
        public long StartTimeTicks { get; private set; }

        /// <summary>
        /// 하 원래 List같은걸로 가져와서 해야하는데..그냥 귀찮으니까 ItemCollection으로 받아서 와야겠다.
        /// 정석대로라면 ControlCollection류를 MinWindow에서 선언한다음,
        /// 그걸 ListControl에 DataSource로 사용하고 , 여기서 그 Collection을 받아와서 사용해야 좀 더 바람직하지 싶다.
        /// </summary>
        public CustomTimerControl(ItemCollection items)
        {
            ParentList = items;
            State = (TimerState.First | TimerState.Cleared);
        }

        public CustomTimerControl(ItemCollection items, TimeSpan time, Timer timer) : this(items)
        {
            InitializeComponent();
            Settime = time;
            this._timer = timer;
            this._timer.Elapsed += Timer_Elapsed;
        }

        public void DoStart() => BtnStart_Click(null, null);

        /// <summary>
        /// ElapsedTime값이 0 이하일때 발생되는 이벤트입니다. 
        /// </summary>
        public event EventHandler OnTimerGoZero;

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            // A 일 경우 StartTime은 미래가 된다 ( 도달할 시간 )
            // A 일 경우 도달한 시간만큼 남은 시간을 Display하면 된다. (목표 시간 - 현재시간)

            if ((State & (TimerState.First | TimerState.Cleared)) != 0)
            {
                StartTimeTicks = DateTime.Now.Ticks + Settime.Ticks;  //A
                State = TimerState.Running;
            }
            //else
            //if ((State & (TimerState.Paused & TimerState.Cleared)) != 0)
            //{
            //    StartTimeTicks = DateTime.Now.Ticks;
            //    State = TimerState.Running;
            //}
            else
            if ((State & TimerState.Paused & ~TimerState.Cleared) != 0)
            {
                StartTimeTicks = DateTime.Now.Ticks + DisplayTime.Ticks; //A

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
            catch (OperationCanceledException)
            {
            }
        }

        private void Timer_ToInvoke()
        {
            //A와 B로직의 차이는 Button Start 부분을 참조할 것

            DisplayTime = TimeSpan.FromTicks(StartTimeTicks - DateTime.Now.Ticks);//A

            if ((State & (TimerState.Running & ~TimerState.Paused)) == 0) return;
            if (DisplayTime.Ticks < 0)
            {
                Logic_stop();
                OnTimerGoZero?.Invoke(this, null);  //이벤트 핸들러 호출

            }
            //일단 남은 시간을 txtbox에 표시
            TimerTime.Text = $"{DisplayTime:hh\\:mm\\:ss\\.fff}";
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
            DisplayTime = Settime;
        }


        /// 지금 현재 문제가 되는 부분이라 임시로 땜빵해서 집어넣어서 테스트중인 부분인데, 솔직히 쓸모가 있는지는모르겠다.
        /// 09-01 수정 : timer의 참조 뿐 아니라 timer가 갖고있던 이벤트핸들러 구독을 함께 취소하니 의존성이 떨어져나갔다.

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Logic_stop();
                    State = TimerState.Cleared;

                    _timer.Elapsed -= Timer_Elapsed;
                    _timer = null;
                }

                _disposedValue = true;
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
