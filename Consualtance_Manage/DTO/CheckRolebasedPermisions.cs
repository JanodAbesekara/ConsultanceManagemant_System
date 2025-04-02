namespace Consualtance_Manage.DTO
{
    public static class CheckRoleBasedPermissions
    {
        public static List<string> GetPermissionsByRole(string role)
        {
            var permissions = new List<string>();

            switch (role)
            {
                case "Admin":
                    permissions.Add(AdminPermissionsDTO.PatientCreate);
                    permissions.Add(AdminPermissionsDTO.PatientDelete);
                    permissions.Add(AdminPermissionsDTO.PatientUpdate);
                    break;
                case "Doctor":
                    permissions.Add(AdminPermissionsDTO.PatientUpdate);
                    break;
                case "Nurse":
                    permissions.Add(AdminPermissionsDTO.PatientCreate);
                    break;
            }

            return permissions;
        }
    }
}