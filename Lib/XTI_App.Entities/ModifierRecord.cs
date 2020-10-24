namespace XTI_App.Entities
{
    public sealed class ModifierRecord
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public int CategoryID { get; set; }
        public string ModKey { get; set; } = "xti_notfound";
        public string TargetKey { get; set; } = "xti_notfound";
        public string DisplayText { get; set; }
    }
}
