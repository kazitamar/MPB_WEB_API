using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace web_api_me.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MUserController : ControllerBase
    {
        [HttpPost(Name = "PostMUser")]
        public HttpResponseMessage Post([FromHeader] string Authorization,int userId, string userName, string password, bool isUserAlloedToPost = false)
        {
            try
            {                
                var mUser = new MUser { UserId = userId , UserName = userName, Password = password, IsUserAlloedToPost = isUserAlloedToPost};

                if (!mUser.isAuthorize(Authorization))
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    
                string msg = "";
                if (mUser.IsValidForInsert(ref msg))
                    return mUser.CreateHttpResponseMessage(mUser.InsertUser());
                else
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = (msg != "" ? msg : "General error") };
            }
            catch (Exception e)
            {
                MPB mpv = new MPB();
                mpv.Insert2ErrorTable("PostMUser", e.Message, userId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}