namespace PluginDevOps
{
    public static class Constants
    {
        public static class MeasureType
        {
            public const string PullRequest = "PullRequest";
            public const string Build = "Build";
            public const string Release = "Release";
            public const string UserId = "UserId";
        }
        public static class Variables
        {
            public const string AccessToken = "#DevOpsAccessToken#";
            public const string CoreServer = "#DevOpsCoreServer#";
            public const string ReleaseServer = "#DevOpsReleaseServer#";
            public const string ProfileServer = "#DevOpsProfileServer#";
            public const string Organization = "#DevOpsOrganization#";
            public const string Project = "#DevOpsProject#";
            public const string Repository = "#DevOpsRespository#";
            public const string ApiVersion = "#DevOpsApiVersion#";
            public const string UserId = "#DevOpsUserId#";
            public const string BuildDefinition = "#DevOpsBuildDefinition#";
            public const string ReleaseDefinition = "#DevOpsReleaseDefinition#";
            public const string ReleaseEnvironment = "#DevOpsReleaseEnvironment#";
            public const string UpdateRate = "#DevOpsUpdateRate#";
        }
        public static class Options
        {
            public const string MeasureType = "Type";
            public const string AccessToken = "AccessToken";
            public const string CoreServer = "CoreServer";
            public const string ReleaseServer = "ReleaseServer";
            public const string ProfileServer = "ProfileServer";
            public const string Organization = "Organization";
            public const string Project = "Project";
            public const string Repository = "Respository";
            public const string ApiVersion = "ApiVersion";
            public const string UserId = "UserId";
            public const string BuildDefinition = "BuildDefinition";
            public const string ReleaseDefinition = "ReleaseDefinition";
            public const string ReleaseEnvironment = "ReleaseEnvironment";
            public const string UpdateRate = "UpdateRate";
        }
        public static class Defaults
        {
            public const string CoreServer = "dev.azure.com";
            public const string ReleaseServer = "vsrm.dev.azure.com";
            public const string ProfileServer = "vssps.dev.azure.com";
            public const string ApiVersion = "5.1";
            public const double UpdateRate = 600;
        }
        public static class BuildStatuses
        {
            public const string All = "all";
            public const string Canceling = "canceling";
            public const string Completed = "completed";
            public const string InProgress = "inProgress";
            public const string None = "none";
            public const string NotStarted = "notStarted";
            public const string Postponed = "postponed";
        }

        public static class BuildResults
        {
            public const string Canceled = "canceled";
            public const string Failed = "failed";
            public const string None = "none";
            public const string PartialSuccess = "partiallySucceeded";
            public const string Succeeded = "succeeded";
        }
    }
}
