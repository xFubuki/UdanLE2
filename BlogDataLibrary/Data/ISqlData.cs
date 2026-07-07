using BlogDataLibrary.Models;

namespace BlogDataLibrary.Data;

public interface ISqlData
{
    List<UserModel> GetUsers();
    UserModel? GetUserById(int id);
    UserModel? GetUserByUserName(string userName);
    int CreateUser(UserModel user);

    List<ListPostModel> GetPosts();
    ListPostModel? GetPostById(int id);
    List<ListPostModel> GetPostsByUserId(int userId);
    int CreatePost(PostModel post);
}
