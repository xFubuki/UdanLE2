using BlogDataLibrary.Database;
using BlogDataLibrary.Models;

namespace BlogDataLibrary.Data;

public class SqlData : ISqlData
{
    private const string ConnectionStringName = "SqlDb";
    private readonly ISqlDataAccess _db;

    public SqlData(ISqlDataAccess db)
    {
        _db = db;
    }

    public List<UserModel> GetUsers()
    {
        const string sql = @"
            SELECT Id, UserName, FirstName, LastName, Password
            FROM dbo.Users
            ORDER BY Id;";

        return _db.LoadData<UserModel, object>(sql, new { }, ConnectionStringName, false);
    }

    public UserModel? GetUserById(int id)
    {
        const string sql = @"
            SELECT Id, UserName, FirstName, LastName, Password
            FROM dbo.Users
            WHERE Id = @Id;";

        return _db.LoadData<UserModel, object>(sql, new { Id = id }, ConnectionStringName, false)
            .FirstOrDefault();
    }

    public UserModel? GetUserByUserName(string userName)
    {
        const string sql = @"
            SELECT Id, UserName, FirstName, LastName, Password
            FROM dbo.Users
            WHERE UserName = @UserName;";

        return _db.LoadData<UserModel, object>(
                sql,
                new { UserName = userName },
                ConnectionStringName,
                false)
            .FirstOrDefault();
    }

    public int CreateUser(UserModel user)
    {
        const string sql = @"
            INSERT INTO dbo.Users (UserName, FirstName, LastName, Password)
            VALUES (@UserName, @FirstName, @LastName, @Password);

            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return _db.ExecuteScalar<int, UserModel>(sql, user, ConnectionStringName, false);
    }

    public List<ListPostModel> GetPosts()
    {
        string sql = GetPostSelectSql() + @"
            ORDER BY p.DateCreated DESC, p.Id DESC;";

        return _db.LoadData<ListPostModel, object>(sql, new { }, ConnectionStringName, false);
    }

    public ListPostModel? GetPostById(int id)
    {
        string sql = GetPostSelectSql() + @"
            WHERE p.Id = @Id
            ORDER BY p.DateCreated DESC, p.Id DESC;";

        return _db.LoadData<ListPostModel, object>(sql, new { Id = id }, ConnectionStringName, false)
            .FirstOrDefault();
    }

    public List<ListPostModel> GetPostsByUserId(int userId)
    {
        string sql = GetPostSelectSql() + @"
            WHERE p.UserId = @UserId
            ORDER BY p.DateCreated DESC, p.Id DESC;";

        return _db.LoadData<ListPostModel, object>(
            sql,
            new { UserId = userId },
            ConnectionStringName,
            false);
    }

    public int CreatePost(PostModel post)
    {
        if (post.DateCreated == default)
        {
            post.DateCreated = DateTime.Now;
        }

        const string sql = @"
            INSERT INTO dbo.Posts (UserId, Title, Body, DateCreated)
            VALUES (@UserId, @Title, @Body, @DateCreated);

            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return _db.ExecuteScalar<int, PostModel>(sql, post, ConnectionStringName, false);
    }

    private static string GetPostSelectSql()
    {
        return @"
            SELECT
                p.Id,
                p.Title,
                p.Body,
                p.DateCreated,
                u.UserName,
                u.FirstName,
                u.LastName
            FROM dbo.Posts p
            INNER JOIN dbo.Users u ON p.UserId = u.Id
            ";
    }
}
