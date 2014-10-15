
namespace VCover.Data
{
    public class CurrentUser
    {
        public int UserID { get; private set; }
        public string Username { get; private set; }
        public int CountryID { get; private set; }

        public CurrentUser(int userId, string username, int countryId)
        {
            UserID = userId;
            Username = username;
            CountryID = countryId;
        }
    }
}
