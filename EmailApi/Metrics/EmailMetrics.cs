using System;
using System.Diagnostics.Metrics;

namespace TGS.EmailApi.Metrics
{
    public class EmailMetrics
    {
        internal static readonly Histogram<int> _ProcessEmails;
        private static readonly Meter _EmailBucketMeter = new("Tgs.EmailBucket");
        private static readonly MeterListener _Listener;
        private static int _ProcessPlayerCount;

        static EmailMetrics()
        {
            _ProcessEmails = _EmailBucketMeter.CreateHistogram<int>("processing.email.count");

            _ = _EmailBucketMeter.CreateObservableGauge(
                "memory.available",
                () =>
                {
                    var memoryInfo = GC.GetGCMemoryInfo();

                    return new Measurement<long>(memoryInfo.TotalAvailableMemoryBytes);
                },
                "Bytes");

            _ = _EmailBucketMeter.CreateObservableGauge(
                "email.memory.total",
                () => new Measurement<long>(GC.GetTotalMemory(false)));

            _ = _EmailBucketMeter.CreateObservableGauge("processing.emails", () => _ProcessPlayerCount, "numbers of email");

            _Listener = new MeterListener();

            _Listener.EnableMeasurementEvents(_ProcessEmails);

            _Listener.SetMeasurementEventCallback<int>(
                (instrument, measurement, tags, _) => _ProcessPlayerCount = measurement);

            _Listener.Start();
        }
    }
}
