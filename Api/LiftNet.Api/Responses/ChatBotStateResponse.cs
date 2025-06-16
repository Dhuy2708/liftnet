using LiftNet.Contract.Enums;

namespace LiftNet.Api.Responses
{
    public class ChatBotStateResponse
    {
        public bool Sucess
        {
            get; set;
        }
        public string? Message
        {
            get; set;
        }
        public UserIntent Intent
        {
            get; set;
        }
    }
}
