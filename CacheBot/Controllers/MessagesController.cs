using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CacheBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace CacheBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        ///     POST: api/Messages
        ///     receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity.GetActivityType() == ActivityTypes.Message)
                await Conversation.SendAsync(activity, () => new BasicLuisDialog());
            else
                HandleSystemMessage(activity);
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        private static Activity HandleSystemMessage(IActivity message)
        {
            switch (message.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                    break;
                case ActivityTypes.Typing:
                    // Handle knowing tha the user is typing
                    break;
                case ActivityTypes.Ping:
                    break;
                default:
                    return null;
            }

            return null;
        }
    }
}