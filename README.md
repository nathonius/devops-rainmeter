# Azure DevOps Rainmeter Plugin
## Configuration

Configuration can be done for the entire skin or for each individual measure. See `@Resources/Config.inc` for skin config. The following options are available:

| Variable Name            | Measure Option Name | Description                                                                                      | Values                                                                       | Default                  | Required For        |
| ------------------------ | ------------------- | ------------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------- | ------------------------ | ------------------- |
| DevOpsCoreServer         | CoreServer          | The url for devops.                                                                              | Any valid URL.                                                               | `dev.azure.com`          | All                 |
| DevOpsReleaseServer      | ReleaseServer       | The url for devops release info.                                                                 | Any valid URL.                                                               | `vsrm.dev.azure.com`     | Release             |
| DevOpsApiVersion         | ApiVersion          | Current API version. Plugin was made for v5.1                                                    | A version string.                                                            | `5.1`                    | All                 |
| DevOpsUpdateRate         | UpdateRate          | Just like WebParser UpdateRate, base skin Update will be multiplied by this value.               | Any integer greater than 0. Set to -1 to update every time the skin updates. | `60` (once every minute) | All                 |
| DevOpsAccessToken        | AccessToken         | A personal access token generated in your profile. See Access Token explanation below.           | A valid PAT.                                                                 |                          | All                 |
| DevOpsOrganization       | Organization        | See URL explanation below.                                                                       | String                                                                       |                          | All                 |
| DevOpsProject            | Project             | See URL explanation below.                                                                       | String                                                                       |                          | All                 |
| DevOpsRespository        | Repository          | See URL explanation below.                                                                       | String                                                                       |                          | Pull Request        |
| DevOpsUserId             | UserId              | The GUID of a user that has PRs assigned, or who created a build. See User ID explanation below. | GUID string                                                                  |                          | Pull Request, Build |
| DevOpsBuildDefinition    | BuildDefinition     | The integer id of a build definition. See URL explanation below.                                 | Integer                                                                      |                          | Build               |
| DevOpsReleaseDefinition  | ReleaseDefinition   | The integer id of a release definition. See URL explanation below.                               | Integer                                                                      |                          | Release             |
| DevOpsReleaseEnvironment | ReleaseEnvironment  | The name of a release environment.                                                               | String                                                                       |                          | Release             |
|                          | Type                | The source to pull data from.                                                                    | `PullRequest`, `Build`, `Release`, or `UserId`                               |                          | All                 |

## Access Tokens

To generate an access token, log in to Dev Ops, navigate to your profile, and select `Personal access tokens`. Here, you can generate a new token. I give mine full access and set the expiration date to be in a year from the date on which it is generated. The generated token should be saved in your `Config.inc` file.

## DevOps URLs

Most of the config values and IDs can be obtained from URLs within DevOps. Here is the basic anatomy of DevOps URLs:

- Navigate to a single repo, and these are the config values: `https://<CoreServer>/<Organization>/<Project>/_git/<Repository>`
- Navigate to a build pipeline page: `https://<CoreServer>/<Organization>/<Project>/_build?definitionId=<BuildDefinition>&_a=summary`
- Navigate to a release page: `https://<CoreServer>/<Organization>/<Project>/_release?_a=releases&view=mine&definitionId=<ReleaseDefinition>`

## User ID

There's a utility measure included to get your user ID. Use `Type=UserId` on the plugin measure and check the skins section of the Rainmeter log. The value of MeasureUserId should be your user guid. This measure will only ever run once, so reload the skin if your access token changes.