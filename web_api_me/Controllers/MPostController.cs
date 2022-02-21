using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace web_api_me.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class MPostController : ControllerBase
    {
        [HttpGet(Name = "GetMPost")]
        public HttpResponseMessage Get([FromHeader] string Authorization, int postId)
        {
            try
            {
                var mPost = new MPost { PostId = postId };
                if (!mPost.isAuthorize(Authorization))
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return mPost.CreateHttpResponseMessage(mPost.SelectPost());
            }
            catch (Exception e)
            {
                MPB mpv = new MPB();
                mpv.Insert2ErrorTable("GetMPost", e.Message,0);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost(Name = "PostMPost")]
        public HttpResponseMessage Post([FromHeader] string Authorization, int userId, string? PostContent, string? title)
        {
            try
            {
                var mPost = new MPost { UserId = userId, PostContent = PostContent, Title = title };
                if (!mPost.isAuthorize(Authorization))
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized); 
                string msg = "";
                if (mPost.IsValidForInsert(ref msg))
                    return mPost.CreateHttpResponseMessage(mPost.InsertPost());
                else
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = (msg != "" ? msg : "General error") };
            }
            catch (Exception e)
            {
                MPB mpv = new MPB();
                mpv.Insert2ErrorTable("PostMPost", e.Message, userId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete(Name = "DeleteMPost")]
        public HttpResponseMessage Delete([FromHeader] string Authorization, int userId, int PostId)
        {
            try
            {
                var mPost = new MPost { UserId = userId, PostId = PostId };
                if (!mPost.isAuthorize(Authorization))
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                string msg = "";
                if (mPost.IsValidForDelete(ref msg))
                    return mPost.CreateHttpResponseMessage(mPost.DeletePost());
                else
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = (msg != "" ? msg : "General error") };
            }
            catch (Exception e)
            {
                MPB mpv = new MPB();
                mpv.Insert2ErrorTable("DeleteMPost", e.Message, userId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut(Name = "PutMPost")]
        public HttpResponseMessage Put([FromHeader] string Authorization, int userId, int postId, string? PostContent, string? title)
        {
            try
            {
                var mPost = new MPost { PostId = postId, UserId = userId, PostContent = PostContent, Title = title };
                if (!mPost.isAuthorize(Authorization))
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                string msg = "";
                if (mPost.IsValidForUpdate(ref msg))
                    return mPost.CreateHttpResponseMessage(mPost.EditPost());
                else
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = (msg != "" ? msg : "General error") };
            }
            catch (Exception e)
            {
                MPB mpv = new MPB();
                mpv.Insert2ErrorTable("PutMPost", e.Message, userId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}