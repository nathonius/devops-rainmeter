using System;
using System.Runtime.InteropServices;
using Rainmeter;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using PluginDevOps.Models;

// Overview: This is a blank canvas on which to build your plugin.

// Note: GetString, ExecuteBang and an unnamed function for use as a section variable
// have been commented out. If you need GetString, ExecuteBang, and/or section variables 
// and you have read what they are used for from the SDK docs, uncomment the function(s)
// and/or add a function name to use for the section variable function(s). 
// Otherwise leave them commented out (or get rid of them)!

namespace PluginDevOps
{
    class Measure
    {
        // Resolved config values
        private string MeasureType { get; set; }
        private string AccessToken { get; set; }
        private string CoreServer { get; set; }
        private string ReleaseServer { get; set; }
        private string ProfileServer { get; set; }
        private string Organization { get; set; }
        private string Project { get; set; }
        private string Repository { get; set; }
        private string ApiVersion { get; set; }
        private string UserId { get; set; }
        private string BuildDefinition { get; set; }
        private string ReleaseDefinition { get; set; }
        private string ReleaseEnvironment { get; set; }
        private int RainmeterUpdate { get; set; }
        private int UpdateRate { get; set; }

        private int _updateCount = -1;

        public string result;
        public API API { get; set; }
        static public implicit operator Measure(IntPtr data)
        {
            return (Measure)GCHandle.FromIntPtr(data).Target;
        }

        public double Update()
        {
            if (_updateCount < 0 || _updateCount >= UpdateRate)
            {
                API.Log(API.LogType.Debug, "Measure.Update()");
                result = string.Empty;
                if (MeasureType.Equals(Constants.MeasureType.PullRequest, StringComparison.OrdinalIgnoreCase))
                {
                    result = GetPRCount();
                }
                else if (MeasureType.Equals(Constants.MeasureType.Build, StringComparison.OrdinalIgnoreCase))
                {
                    result = GetBuildStatus();
                }
                else if (MeasureType.Equals(Constants.MeasureType.Release, StringComparison.OrdinalIgnoreCase))
                {
                    result = GetReleaseStatus();
                }
                else if (MeasureType.Equals(Constants.MeasureType.UserId, StringComparison.OrdinalIgnoreCase))
                {
                    result = GetUserId();
                }
                else
                {
                    result = string.Empty;
                }
                _updateCount = 0;
            }
            else if (!MeasureType.Equals(Constants.MeasureType.UserId, StringComparison.OrdinalIgnoreCase))
            {
                _updateCount++;
            }
            return 0.0;
        }

        public void Reload()
        {
            // Read measure type
            MeasureType = API.ReadString(Constants.Options.MeasureType, "");

            // Get update rate
            UpdateRate = GetConfigValueInt(Constants.Variables.UpdateRate, Constants.Options.UpdateRate, defaultValue: 600, errorOnEmpty: true);

            // Get config values
            // Always
            AccessToken = GetConfigValue(Constants.Variables.AccessToken, Constants.Options.AccessToken, errorOnEmpty: true);
            CoreServer = GetConfigValue(Constants.Variables.CoreServer, Constants.Options.CoreServer, defaultValue: Constants.Defaults.CoreServer, errorOnEmpty: true);
            Organization = GetConfigValue(Constants.Variables.Organization, Constants.Options.Organization, errorOnEmpty: true);
            Project = GetConfigValue(Constants.Variables.Project, Constants.Options.Project, errorOnEmpty: true);
            ApiVersion = GetConfigValue(Constants.Variables.ApiVersion, Constants.Options.ApiVersion, defaultValue: Constants.Defaults.ApiVersion, errorOnEmpty: true);

            // PR Configuration
            if (MeasureType.Equals(Constants.MeasureType.PullRequest, StringComparison.OrdinalIgnoreCase))
            {
                Repository = GetConfigValue(Constants.Variables.Repository, Constants.Options.Repository, errorOnEmpty: true);
                UserId = GetConfigValue(Constants.Variables.UserId, Constants.Options.UserId);
            }
            // Build Configuration
            else if (MeasureType.Equals(Constants.MeasureType.Build, StringComparison.OrdinalIgnoreCase))
            {
                BuildDefinition = GetConfigValue(Constants.Variables.BuildDefinition, Constants.Options.BuildDefinition, errorOnEmpty: true);
                UserId = GetConfigValue(Constants.Variables.UserId, Constants.Options.UserId);
            }
            // Release Configuration
            else if (MeasureType.Equals(Constants.MeasureType.Release, StringComparison.OrdinalIgnoreCase))
            {
                ReleaseServer = GetConfigValue(Constants.Variables.ReleaseServer, Constants.Options.ReleaseServer, defaultValue: Constants.Defaults.ReleaseServer, errorOnEmpty: true);
                ReleaseDefinition = GetConfigValue(Constants.Variables.ReleaseDefinition, Constants.Options.ReleaseDefinition, errorOnEmpty: true);
                ReleaseEnvironment = GetConfigValue(Constants.Variables.ReleaseEnvironment, Constants.Options.ReleaseEnvironment);
            }
            // User ID Configuration
            else if (MeasureType.Equals(Constants.MeasureType.UserId, StringComparison.OrdinalIgnoreCase))
            {
                ProfileServer = GetConfigValue(Constants.Variables.ProfileServer, Constants.Options.ProfileServer, defaultValue: Constants.Defaults.ProfileServer, errorOnEmpty: true);
            }
        }

        private string GetPRCount()
        {
            string prCount = "";

            // Create request
            var request = PrepareRequest($"https://{CoreServer}/{Organization}/{Project}/_apis/git/repositories/{Repository}/pullrequests?api-version={ApiVersion}&searchCriteria.reviewerId={UserId}");

            // Get response
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string responseJson = reader.ReadToEnd();
                    var prResponse = JsonConvert.DeserializeObject<PRResponse>(responseJson);
                    prCount = prResponse.Count;
                }
            }
            return prCount;
        }

        private string GetBuildStatus()
        {
            string status = "";

            // Create request
            var request = PrepareRequest($"https://{CoreServer}/{Organization}/{Project}/_apis/build/builds?api-version={ApiVersion}&requestedFor={UserId}&definitions={BuildDefinition}&$top=1");

            // Get response
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string responseJson = reader.ReadToEnd();
                    var buildStatusResponse = JsonConvert.DeserializeObject<BuildStatusResponse>(responseJson);
                    var build = buildStatusResponse.Builds[0];
                    if (build.Status.Equals(Constants.BuildStatuses.Completed))
                    {
                        status = build.Result;
                    }
                    else
                    {
                        status = build.Status;
                    }
                }
            }
            return status;
        }

        private string GetReleaseStatus()
        {
            int releaseId = 0;
            string status = "";

            // First get last release ID
            // Create request
            var listRequest = PrepareRequest($"https://{ReleaseServer}/{Organization}/{Project}/_apis/release/releases?api-version={ApiVersion}&$top=1&definitionId={ReleaseDefinition}");

            // Get response
            var listResponse = (HttpWebResponse)listRequest.GetResponse();
            if (listResponse.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = listResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string responseJson = reader.ReadToEnd();
                    var releaseListResponse = JsonConvert.DeserializeObject<ReleaseListResponse>(responseJson);
                    var release = releaseListResponse.Releases[0];
                    releaseId = release.Id;
                }
            }

            API.Log(API.LogType.Debug, $"GetReleaseStatus() : Got release ID {releaseId}");

            // Now get real release
            // Prepare request
            var releaseRequest = PrepareRequest($"https://{ReleaseServer}/{Organization}/{Project}/_apis/release/releases/{releaseId}?$expand=none&api-version={ApiVersion}");

            // Get response
            var releaseResponse = (HttpWebResponse)releaseRequest.GetResponse();
            if (releaseResponse.StatusCode == HttpStatusCode.OK)
            {
                API.Log(API.LogType.Debug, $"GetReleaseStatus() : Got good response.");
                using (Stream responseStream = releaseResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string responseJson = reader.ReadToEnd();
                    API.Log(API.LogType.Debug, $"GetReleaseStatus() : Read response.");
                    var response = JsonConvert.DeserializeObject<ReleaseResponse>(responseJson);
                    API.Log(API.LogType.Debug, $"GetReleaseStatus() : Converted response.");
                    foreach (var env in response.Environments)
                    {
                        if (env.Name.Equals(ReleaseEnvironment, StringComparison.OrdinalIgnoreCase))
                        {
                            API.Log(API.LogType.Debug, $"GetReleaseStatus() : Found env {env.Name} with status {env.Status}");
                            status = env.Status;
                        }
                    }
                }
            }
            return status;
        }

        public string GetUserId()
        {
            string userId = "";

            // Create request
            var request = PrepareRequest($"https://{ProfileServer}/{Organization}/_apis/profile/profiles/me?api-version={ApiVersion}");

            // Get response
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string responseJson = reader.ReadToEnd();
                    var userIdResponse = JsonConvert.DeserializeObject<UserIdResponse>(responseJson);
                    userId = userIdResponse.UserId;
                }
            }
            return userId;
        }

        /// <summary>
        /// Checks variables, measure options for config values
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="optionName"></param>
        /// <param name="errorOnEmpty"></param>
        /// <returns></returns>
        private string GetConfigValue(string variableName, string optionName, string defaultValue = "", bool errorOnEmpty = false)
        {
            // Read from options first, as it is most specific
            string result = API.ReadString(optionName, string.Empty);
            if (string.IsNullOrEmpty(result))
            {
                // No option provided, check variables
                result = API.ReplaceVariables(variableName);
                if (string.IsNullOrEmpty(result) || result.Equals(variableName))
                {
                    // No variable, set to default
                    result = defaultValue;
                }
            }
            if (errorOnEmpty && string.IsNullOrEmpty(result))
            {
                API.Log(API.LogType.Error, "No config value for " + optionName);
            }
            return result;
        }

        /// <summary>
        /// Checks variables, measure options for config values
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="optionName"></param>
        /// <param name="errorOnEmpty"></param>
        /// <returns></returns>
        private int GetConfigValueInt(string variableName, string optionName, int defaultValue = -1, bool errorOnEmpty = false)
        {
            // Read from options first, as it is most specific
            var result = API.ReadInt(optionName, -1);
            if (result == -1)
            {
                // No option provided, check variables
                var stringResult = API.ReplaceVariables(variableName);
                if (string.IsNullOrEmpty(stringResult) || stringResult.Equals(variableName))
                {
                    // No variable, set to default
                    result = defaultValue;
                }
                else
                {
                    // Found variable value
                    result = int.Parse(stringResult);
                }
            }
            if (errorOnEmpty && result == -1)
            {
                API.Log(API.LogType.Error, "No config value for " + optionName);
            }
            return result;
        }

        private WebRequest PrepareRequest(string uri)
        {
            API.Log(API.LogType.Debug, $"Preparing request for: {uri}");
            var request = WebRequest.Create(uri);
            request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + AccessToken);
            request.ContentType = "application/json";
            return request;
        }

        public IntPtr buffer = IntPtr.Zero;
    }

    public class Plugin
    {
        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            Measure measure = new Measure();
            data = GCHandle.ToIntPtr(GCHandle.Alloc(measure));
            API api = rm;
            measure.API = api;
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            Measure measure = (Measure)data;
            if (measure.buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(measure.buffer);
            }
            GCHandle.FromIntPtr(data).Free();
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = data;
            measure.Reload();
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)data;

            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)data;
            if (measure.buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(measure.buffer);
                measure.buffer = IntPtr.Zero;
            }

            measure.buffer = Marshal.StringToHGlobalUni(measure.result);

            return measure.buffer;
        }

        //[DllExport]
        //public static void ExecuteBang(IntPtr data, [MarshalAs(UnmanagedType.LPWStr)]String args)
        //{
        //    Measure measure = (Measure)data;
        //}

        //[DllExport]
        //public static IntPtr (IntPtr data, int argc,
        //    [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)] string[] argv)
        //{
        //    Measure measure = (Measure)data;
        //    if (measure.buffer != IntPtr.Zero)
        //    {
        //        Marshal.FreeHGlobal(measure.buffer);
        //        measure.buffer = IntPtr.Zero;
        //    }
        //
        //    measure.buffer = Marshal.StringToHGlobalUni("");
        //
        //    return measure.buffer;
        //}
    }
}

