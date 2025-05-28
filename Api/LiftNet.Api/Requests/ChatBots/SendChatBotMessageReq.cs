namespace LiftNet.Api.Requests.ChatBots
{
    public class SendChatBotMessageReq
    {
        public string ConversationId
        {
            get; set;
        }
        public string Message
        {
            get; set;
        }
    }
}
