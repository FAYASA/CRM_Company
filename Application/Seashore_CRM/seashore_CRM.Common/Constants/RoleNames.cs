namespace seashore_CRM.Common.Constants
{
    public enum RoleType
    {
        Administrator,
        SalesRep,
        //Manager
    }

    public static class RoleNames
    {
        public const string Administrator = "Administrator";
        public const string SalesRep = "SalesRep";
        //public const string Manager = "Manager";
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Suspended
    }
}