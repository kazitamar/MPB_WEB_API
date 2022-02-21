using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace web_api_me.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MLikeController : ControllerBase
    {
        [HttpPost(Name = "PostMLike")]
        public HttpResponseMessage Post([FromHeader] string Authorization, int userId, int postId)
        {
            try
            {
                var mLike = new MLike { UserId = userId, PostId = postId };
                if (!mLike.isAuthorize(Authorization))
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                string msg = "";
                if (mLike.IsValidForInsert(ref msg))
                    return mLike.CreateHttpResponseMessage(mLike.InsertLike());
                else
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = (msg != "" ? msg : "General error") };
            }
            catch (Exception e)
            {
                MPB mpv = new MPB();
                mpv.Insert2ErrorTable("PostMLike", e.Message, userId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete(Name = "DeleteMLike")]
        public HttpResponseMessage Delete([FromHeader] string Authorization, int userId, int postId)
        {
            try
            {
                var mLike = new MLike { UserId = userId, PostId = postId };
                if (!mLike.isAuthorize(Authorization))
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                string msg = "";
                if (mLike.IsValidForDelete(ref msg))
                    return mLike.CreateHttpResponseMessage(mLike.Deletelike());
                else
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = (msg != "" ? msg : "General error") };
            }
            catch (Exception e)
            {
                MPB mpv = new MPB();
                mpv.Insert2ErrorTable("DeleteMLike", e.Message, userId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}