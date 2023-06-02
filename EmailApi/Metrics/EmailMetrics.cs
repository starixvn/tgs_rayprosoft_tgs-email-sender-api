using System;
using System.Diagnostics.Metrics;

namespace TGS.EmailApi.Metrics
{
    public class EmailMetrics
    {
        internal static readonly Histogram<int> _ProcessEmails;
        private static readonly Meter _meter = new("Tgs.EmailBucket");
        private static readonly MeterListener _Listener;
        private static int _ProcessPlayerCount;

        static EmailMetrics()
        {
            _ProcessEmails = _meter.CreateHistogram<int>("processing.email.count");

            _ = _meter.CreateObservableGauge(
                "email.memory.available",
                () =>
                {
                    var memoryInfo = GC.GetGCMemoryInfo();
                    Console.WriteLine($"email.memory.available => {memoryInfo}");
                    return new Measurement<long>(memoryInfo.TotalAvailableMemoryBytes);
                },
                "Bytes");

            _ = _meter.CreateObservableGauge(
                "email.memory.total",
                () =>
                {
                    var totalMemory = GC.GetTotalMemory(false);
                    Console.WriteLine($"email.memory.total => {totalMemory}");
                    return new Measurement<long>(totalMemory);
                });

            _ = _meter.CreateObservableGauge("processing.emails", () => 
            {
                Console.WriteLine($"processing.emails => {_ProcessPlayerCount}");
                return _ProcessPlayerCount;
            }, "numbers of email");

            _Listener = new MeterListener();

            _Listener.EnableMeasurementEvents(_ProcessEmails);

            _Listener.SetMeasurementEventCallback<int>((instrument, measurement, tags, state) =>
            {
                _ProcessPlayerCount = measurement;
                Console.WriteLine($"SetMeasurementEventCallback => Instrument: {instrument.Name} has recorded the measurement {measurement}");
            });

            _Listener.InstrumentPublished = (instrument, meterListener) =>
            {
                meterListener.EnableMeasurementEvents(instrument);
                Console.WriteLine($"InstrumentPublished => EnableMeasurementEvents {instrument.Name} ");
            };


            _Listener.Start();
        }
    }
}
