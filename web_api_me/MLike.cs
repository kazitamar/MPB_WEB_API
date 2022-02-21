using System.Data;
using System.Data.SqlClient;

namespace web_api_me
{
    public class MLike:MPB
    {
        public int UserId { get; set; }
        public int PostId { get; set; }

        public bool IsValidForInsert(ref string msg)
        {
            try
            {
                if (UserId == 0)
                {
                    msg = "User Id is a required field";
                    return false;
                }
                if (!IsUserIdExists(UserId))
                {
                    msg = "User Id does not exists";
                    return false;
                }
                if (PostId == 0)
                {
                    msg = "Post Id is a required field";
                    return false;
                }
                if (!IsPostIdExists())
                {
                    msg = "Post Id does not exists";
                    return false;
                }
                if (IsUserLikedPost())
                {
                    msg = "You already liked this post";
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MLike.IsValidForInsert", e.Message, UserId);
                return false;
            }
        }

        public bool IsValidForDelete(ref string msg)
        {
            try
            {
                if (UserId == 0)
                {
                    msg = "User Id is a required field";
                    return false;
                }
                if (!IsUserIdExists(UserId))
                {
                    msg = "User Id does not exists";
                    return false;
                }
                if (PostId == 0)
                {
                    msg = "Post Id is a required field";
                    return false;
                }
                if (!IsPostIdExists())
                {
                    msg = "Post Id does not exists";
                    return false;
                }
                if (!IsUserLikedPost())
                {
                    msg = "You did not like this post";
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MLike.IsValidForDelete", e.Message, UserId);
                return false;
            }
        }

        public bool IsUserLikedPost()
        {
            try
            {
                SqlCommand command = new SqlCommand("select 1 from POST_LIKES where POST_ID = @PostId and USER_ID = @UserId");
                command.Parameters.Add("@PostId", SqlDbType.Int);
                command.Parameters["@PostId"].Value = PostId;
                command.Parameters.Add("@UserId", SqlDbType.Int);
                command.Parameters["@UserId"].Value = UserId;
                DataTable dt = GetRawData(command);
                if (dt.Rows.Count > 0)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MLike.IsUserLikedPost", e.Message, UserId);
                return false;
            }
        }

        public bool IsPostIdExists()
        {
            try
            {
                SqlCommand command = new SqlCommand("select 1 from POSTS where POST_ID = @PostId");
                command.Parameters.Add("@PostId", SqlDbType.Int);
                command.Parameters["@PostId"].Value = PostId;
                DataTable dt = GetRawData(command);
                if (dt.Rows.Count > 0)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MLike.IsPostIdExists", e.Message, UserId);
                throw e;
            }
        }

        public string InsertLike()
        {
            try
            {
                SqlCommand command = new SqlCommand("insert into POST_LIKES (POST_ID,USER_ID) values (@PostId,@UserId)");
                command.Parameters.Add("@UserId", SqlDbType.Int);
                command.Parameters["@UserId"].Value = UserId;
                command.Parameters.Add("@PostId", SqlDbType.Int);
                command.Parameters["@PostId"].Value = PostId;

                if (EditData(command) > 0)
                {
                    return "Your like has been created successfully";
                }
                return "Error";
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MLike.InsertLike", e.Message, UserId);
                return "Error";
            }
        }

        public string Deletelike()
        {
            try
            {
                SqlCommand command = new SqlCommand("delete POST_LIKES where POST_ID = @PostId and USER_ID=@UserId");
                command.Parameters.Add("@PostId", SqlDbType.Int);
                command.Parameters["@PostId"].Value = PostId;
                command.Parameters.Add("@UserId", SqlDbType.Int);
                command.Parameters["@UserId"].Value = UserId;
                if (EditData(command) > 0)
                    return "Your like has been deleted successfully";
                return "Error";
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MLike.Deletelike", e.Message, UserId);
                return "Error";
            }
        }
    }
}