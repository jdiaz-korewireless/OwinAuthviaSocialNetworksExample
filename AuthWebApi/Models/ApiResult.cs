using AuthWebApi.Models.Account;
using Newtonsoft.Json;

namespace AuthWebApi.Models
{
    [JsonObject("result")]
    public class ApiResult
    {
        public const string StatusOk = "ok";
        public const string StatusError = "error";

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("statusmessage")]
        public string StatusMessage { get; set; }

        [JsonIgnore]
        public bool IsSuccess
        {
            get { return Status == StatusOk; }
        }
    }

    public class SuccessResult : ApiResult
    {
        public SuccessResult() :
            base()
        {
            Status = ApiResult.StatusOk;
        }
    }

    public class FailureApiResult : ApiResult
    {
        public FailureApiResult(string error)
        {
            Status = StatusError;
            StatusMessage = error;
        }
    }

    public class UserResult : SuccessResult
    {
        [JsonProperty("user")]
        public UserViewModel User { get; set; }

        public UserResult(UserViewModel user)
        {
            User = user;
        }
    }
}