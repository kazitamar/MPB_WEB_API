using System.Data;
using System.Data.SqlClient;

namespace web_api_me
{
    public class MUser:MPB
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsUserAlloedToPost { get; set; }

        public bool IsValidForInsert(ref string msg)
        {
            try
            {
                if (UserId == 0)
                {
                    msg = "User Id is a required field";
                    return false;
                }
                if (UserName == "")
                {
                    msg = "User Name is a required field";
                    return false;
                }
                UserName = UserName.Trim();
                if (UserName.Length > 50)
                {
                    msg = "Your name pass the limit of 50 characters";
                    return false;
                }
                Password = Password.Trim();
                if (Password == "")
                {
                    msg = "Password is a required field";
                    return false;
                }
                if (Password.Length > 20)
                {
                    msg = "Your password pass the limit of 20 characters";
                    return false;
                }
                if (IsUserIdExists(UserId))
                {
                    msg = "User Id already exists";
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MUser.IsValidForInsert", e.Message, UserId);
                return false;
            }
        }

        public string InsertUser()
        {
            try
            {
                SqlCommand command = new SqlCommand("insert into USERS (USER_ID,USER_NAME,PASSWORD,POST_BLOG_ALLOWED) values (@UserId,@UserName,@Password,@IsUserAlloedToPost)");
                command.Parameters.Add("@UserId", SqlDbType.Int);
                command.Parameters["@UserId"].Value = UserId;
                command.Parameters.Add("@UserName", SqlDbType.NVarChar);
                command.Parameters["@UserName"].Value = UserName;
                command.Parameters.Add("@Password", SqlDbType.NVarChar);
                command.Parameters["@Password"].Value = Password;
                command.Parameters.Add("@IsUserAlloedToPost", SqlDbType.Bit);
                command.Parameters["@IsUserAlloedToPost"].Value = IsUserAlloedToPost;


                if (EditData(command) > 0)
                {
                    return "Your User has been created successfully";
                }
                return "Error";
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MUser.InsertUser", e.Message, UserId);
                return "Error";
            }
        }
    }
}