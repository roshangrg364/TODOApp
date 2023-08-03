namespace TodoApp.ViewModel
{
    public class PermissionViewModel
    {
        public string RoleId { get; set; }
        public IList<ModuleWisePermissionViewModel> Permissions { get; set; } = new List<ModuleWisePermissionViewModel>();
    }
    public class ModuleWisePermissionViewModel
    {
        public string Module { get; set; }
        public bool IsAssignedAll => PermissionData.All(a => a.IsAssigned);
        public IList<PermissionValuesViewModel> PermissionData { get; set; } = new List<PermissionValuesViewModel>();
    }
    public class PermissionValuesViewModel
    {
        public bool IsAssigned { get; set; }
        public string Value { get; set; }

    }

    public class PermissionAssignViewModel
    {
        public string  roleId { get; set; }
        public string  permission { get; set; }
    }

    public class ModuleAssignViewModel
    {
        public string roleId { get; set; }
        public string module { get; set; }
    }
}

