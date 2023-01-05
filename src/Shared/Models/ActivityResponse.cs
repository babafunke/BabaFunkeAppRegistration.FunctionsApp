namespace Shared
{
    public class ActivityResponse
    {
        public ActivityResponse()
        {

        }

        public ActivityResponse(string activityName, string message)
        {
            ActivityName = activityName;
            Message = message;
            ActivityStatus = ActivityStatus.Failed;
        }

        public string ActivityName { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public ActivityStatus ActivityStatus { get; set; }
    }
}