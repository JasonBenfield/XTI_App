namespace XTI_App
{
    public sealed class ResourceGroupRoleModel
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool IsAllowed { get; set; }
    }
}
