using System.Data;
using System.Data.SqlClient;

namespace web_api_me
{
    public class MPost: MPB
    {
        private static readonly int MaxNumWordsAllowedInContent = 1000;
        private static readonly int MaxCharsAllowedInTitle = 50;
        private static readonly string[] ForbiddenWords = new[]{"zzz", "aaa"};
        public int PostId { get; set; }
        public string? PostContent { get; set; }
        public string? Title { get; set; }
        public int UserId { get; set; }
        public int NumLikes { get; set; }

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
                if (PostContent == null)
                {
                    msg = "Post Content is a required field";
                    return false;
                }
                PostContent = PostContent.Trim();
                if (PostContent.Split(" ").Length > MaxNumWordsAllowedInContent)
                {
                    msg = "Your post Content pass the limit of " + MaxNumWordsAllowedInContent + " words";
                    return false;
                }
                if (Title == null)
                {
                    msg = "Title is a required field";
                    return false;
                }
                Title = Title.Trim();
                if (Title.Length > MaxCharsAllowedInTitle)
                {
                    msg = "Your post title pass the limit of  " + MaxCharsAllowedInTitle + "  characters";
                    return false;
                }
                //My arbitrary rule:
                foreach (var forbiddenWord in ForbiddenWords)
                {
                    if (PostContent.Contains(forbiddenWord))
                    {
                        msg = "Your post Content contains a forbidden word: " + forbiddenWord;
                        return false;
                    }
                }

                SqlCommand command = new SqlCommand("SELECT 1 FROM USERS where USER_ID = @UserId and POST_BLOG_ALLOWED = 1");
                command.Parameters.Add("@UserId", SqlDbType.Int);
                command.Parameters["@UserId"].Value = UserId;
                DataTable dt = GetRawData(command);
                if (dt.Rows.Count == 0)
                {
                    msg = "You are not allowed to publish a post";
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPost.IsValidForInsert", e.Message, UserId);
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
                if (SelectUserIdByPostId() == 0)
                {
                    msg = "Post Id Does not exists";
                    return false;
                }
                if (SelectUserIdByPostId() != UserId)
                {
                    msg = "Post Id Does not belong to you";
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPost.IsValidForDelete", e.Message, UserId);
                return false;
            }
        }

        public bool IsValidForUpdate(ref string msg)
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
                if (PostContent == null && Title == null)
                {
                    msg = "Post Content or Title must be full";
                    return false;
                }
                if (PostContent != null)
                {
                    PostContent = PostContent.Trim();
                    if (PostContent.Split(" ").Length > MaxNumWordsAllowedInContent - 1)
                    {
                        msg = "Your post Content pass the limit of " + MaxNumWordsAllowedInContent + " words";
                        return false;
                    }
                    //My arbitrary rule:
                    foreach (var forbiddenWord in ForbiddenWords)
                    {
                        if (PostContent.Contains(forbiddenWord))
                        {
                            msg = "Your post Content contains a forbidden word: " + forbiddenWord;
                            return false;
                        }
                    }
                }
                if (Title != null)
                {
                    Title = Title.Trim();
                    if (Title.Length > MaxCharsAllowedInTitle)
                    {
                        msg = "Your post title pass the limit of  " + MaxCharsAllowedInTitle + "  characters";
                        return false;
                    }
                }
                if (SelectUserIdByPostId() == 0)
                {
                    msg = "Post Id Does not exists";
                    return false;
                }
                if (SelectUserIdByPostId() != UserId)
                {
                    msg = "Post Id Does not belong to you";
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPost.IsValidForUpdate", e.Message, UserId);
                return false;
            }
        }
        
        public int SelectUserIdByPostId()
        {
            try
            {
                SqlCommand command = new SqlCommand("select CRT_USER_ID from POSTS where POST_ID = @PostId");
                command.Parameters.Add("@PostId", SqlDbType.Int);
                command.Parameters["@PostId"].Value = PostId;
                DataTable dt = GetRawData(command);
                if (dt.Rows.Count > 0)
                    return (int)dt.Rows[0].ItemArray[0];
                return 0;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPost.SelectUserIdByPostId", e.Message, UserId);
                return 0;
            }
        }

        public string SelectPost()
        {
            try
            {
                SqlCommand command = new SqlCommand("select p.POST_ID,USER_NAME,POST_TITLE,POST_CONTENT,INS_DATE,UPD_DATE,ISNULL(n.NUM_LIKES,0) as NUM_LIKES " +
                                                   " from POSTS p " +
                                                   " left join USERS u on u.USER_ID = p.CRT_USER_ID" +
                                                   " left join NUM_LIKES_PER_POST n on p.POST_ID = n.POST_ID ");
                if (PostId > 0)
                {
                    command.CommandText += " where p.POST_ID = @PostId";
                    command.Parameters.Add("@PostId", SqlDbType.Int);
                    command.Parameters["@PostId"].Value = PostId;
                }
                return GetData(command);
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPost.SelectPost", e.Message, UserId);
                return "Error";
            }
        }

        public string InsertPost()
        {
            try
            {
                SqlCommand command = new SqlCommand("insert into POSTS (POST_ID,POST_CONTENT,INS_DATE,CRT_USER_ID,POST_TITLE) values (NEXT VALUE FOR POSTS_SEQ,@PostContent,GETDATE(),@UserId,@Title)");
                command.Parameters.Add("@PostContent", SqlDbType.NText);
                command.Parameters["@PostContent"].Value = PostContent;
                command.Parameters.Add("@UserId", SqlDbType.Int);
                command.Parameters["@UserId"].Value = UserId;
                command.Parameters.Add("@Title", SqlDbType.NVarChar);
                command.Parameters["@Title"].Value = Title;

                if (EditData(command) > 0)
                {
                    DataTable dt = GetRawData(new SqlCommand("SELECT current_value FROM sys.sequences WHERE name = 'POSTS_SEQ'"));
                    if (dt.Rows.Count > 0)
                        return "Your post has been created successfully. Post Id for Update/Delete: " + dt.Rows[0].ItemArray[0].ToString();
                    return "Your post has been created successfully";
                }
                return "Error";
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPost.InsertPost", e.Message, UserId);
                return "Error";
            }
        }

        public string EditPost()
        {
            try
            {
                SqlCommand command = new SqlCommand("update POSTS set UPD_DATE = GETDATE()");
                if (Title != null)
                {
                    command.CommandText += " ,POST_TITLE=@Title";
                    command.Parameters.Add("@Title", SqlDbType.NVarChar);
                    command.Parameters["@Title"].Value = Title;
                }
                if (PostContent != null)
                {
                    command.CommandText += " ,POST_CONTENT=@PostContent";
                    command.Parameters.Add("@PostContent", SqlDbType.NText);
                    command.Parameters["@PostContent"].Value = PostContent;
                }
                command.CommandText += " where POST_ID = @PostId";
                command.Parameters.Add("@PostId", SqlDbType.Int);
                command.Parameters["@PostId"].Value = PostId;
                if (EditData(command) > 0)
                    return "Your post has been updated successfully";
                return "Error";
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPost.EditPost", e.Message, UserId);
                return "Error";
            }
        }

        public string DeletePost()
        {
            try
            {
                SqlCommand command = new SqlCommand("delete POSTS where POST_ID = @PostId");
                command.Parameters.Add("@PostId", SqlDbType.Int);
                command.Parameters["@PostId"].Value = PostId;
                if (EditData(command) > 0)
                    return "Your post has been deleted successfully";
                return "Error";
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPost.DeletePost", e.Message, UserId);
                return "Error";
            }
        }
    }
}