using System.Diagnostics;

public class PerformanceIndicator
{
    Stopwatch _stopWatch;

    public PerformanceIndicator()
    {
        _stopWatch = new Stopwatch();
    }

    public void Begin()
    {
        // 시간측정 시작
        _stopWatch.Start();
    }

    // 1s = 1,000ms = 1,000,000,000ns
    public void End()
    {
        // 측정 종료
        _stopWatch.Stop();
        // 측정 결과 출력
        UnityEngine.Debug.Log($"time: {_stopWatch.ElapsedTicks / 1000000.0f} ms");
        // 스톱워치 초기화
        _stopWatch.Reset();
    }
}