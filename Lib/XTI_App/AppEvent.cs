using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppEvent
    {
        private readonly AppEventRecord record;

        internal AppEvent(AppEventRecord record)
        {
            this.record = record ?? new AppEventRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public string Caption { get => record.Caption; }
        public string Message { get => record.Message; }
        public string Detail { get => record.Detail; }
        public AppEventSeverity Severity() => AppEventSeverity.Values.Value(record.Severity);

        public AppEventModel ToModel() => new AppEventModel
        {
            ID = ID.Value,
            RequestID = record.RequestID,
            TimeOccurred = record.TimeOccurred,
            Severity = Severity(),
            Caption = Caption,
            Message = Message,
            Detail = Detail
        };

        public override string ToString() => $"{nameof(AppEvent)} {ID.Value}";
    }
}
