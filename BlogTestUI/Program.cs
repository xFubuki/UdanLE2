using BlogDataLibrary.Data;
using BlogDataLibrary.Database;
using BlogDataLibrary.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogTestUI;

internal class Program
{
    private static UserModel? _currentUser;

    private static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        ServiceProvider serviceProvider = new ServiceCollection()
            .AddSingleton(configuration)
            .AddTransient<ISqlDataAccess, SqlDataAccess>()
            .AddTransient<ISqlData, SqlData>()
            .BuildServiceProvider();

        ISqlData data = serviceProvider.GetRequiredService<ISqlData>();

        RunMenu(data);
    }

    private static void RunMenu(ISqlData data)
    {
        bool keepRunning = true;

        while (keepRunning)
        {
            Console.WriteLine();
            Console.WriteLine("1. Register user");
            Console.WriteLine("2. Log in");
            Console.WriteLine("3. Add post");
            Console.WriteLine("4. List all posts");
            Console.WriteLine("5. Show post details");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    RegisterUser(data);
                    break;
                case 2:
                    LogIn(data);
                    break;
                case 3:
                    AddPost(data);
                    break;
                case 4:
                    ListAllPosts(data);
                    break;
                case 5:
                    ShowPostDetails(data);
                    break;
                case 6:
                    keepRunning = false;
                    break;
                default:
                    Console.WriteLine("Please choose a valid menu option.");
                    break;
            }
        }
    }

    private static void RegisterUser(ISqlData data)
    {
        string userName = PromptRequired("Username", 16);
        string firstName = PromptRequired("First name", 50);
        string lastName = PromptRequired("Last name", 50);
        string password = PromptRequired("Password", 16);

        if (data.GetUserByUserName(userName) is not null)
        {
            Console.WriteLine("That username is already taken.");
            return;
        }

        UserModel user = new()
        {
            UserName = userName,
            FirstName = firstName,
            LastName = lastName,
            Password = password
        };

        int id = data.CreateUser(user);
        Console.WriteLine($"User created with ID: {id}");
    }

    private static void LogIn(ISqlData data)
    {
        Console.Write("Username: ");
        string userName = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? string.Empty;

        UserModel? user = data.GetUserByUserName(userName);

        // Plaintext passwords are insecure and must be replaced with password hashing in production.
        if (user is null || user.Password != password)
        {
            Console.WriteLine("Invalid username or password.");
            return;
        }

        _currentUser = user;
        Console.WriteLine($"Logged in as {user.FirstName} {user.LastName} (@{user.UserName}).");
    }

    private static void AddPost(ISqlData data)
    {
        if (_currentUser is null)
        {
            Console.WriteLine("You must log in before adding a post.");
            return;
        }

        string title = PromptRequired("Title", 150);
        string body = PromptRequired("Body");

        PostModel post = new()
        {
            UserId = _currentUser.Id,
            Title = title,
            Body = body
        };

        int id = data.CreatePost(post);
        Console.WriteLine($"Post created with ID: {id}");
    }

    private static void ListAllPosts(ISqlData data)
    {
        List<ListPostModel> posts = data.GetPosts();

        if (posts.Count == 0)
        {
            Console.WriteLine("No posts found.");
            return;
        }

        foreach (ListPostModel post in posts)
        {
            Console.WriteLine($"ID: {post.Id}");
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Author: {post.FirstName} {post.LastName} (@{post.UserName})");
            Console.WriteLine($"Created: {post.DateCreated:yyyy-MM-dd HH:mm}");
            Console.WriteLine("--------------------------------------------------");
        }
    }

    private static void ShowPostDetails(ISqlData data)
    {
        Console.Write("Post ID: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Please enter a valid post ID.");
            return;
        }

        ListPostModel? post = data.GetPostById(id);

        if (post is null)
        {
            Console.WriteLine("Post not found.");
            return;
        }

        Console.WriteLine($"ID: {post.Id}");
        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Body: {post.Body}");
        Console.WriteLine($"Author: {post.FirstName} {post.LastName}");
        Console.WriteLine($"Username: {post.UserName}");
        Console.WriteLine($"Date created: {post.DateCreated:yyyy-MM-dd HH:mm}");
    }

    private static string PromptRequired(string label, int? maxLength = null)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            string value = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(value))
            {
                Console.WriteLine($"{label} is required.");
                continue;
            }

            if (maxLength.HasValue && value.Length > maxLength.Value)
            {
                Console.WriteLine($"{label} must be {maxLength.Value} characters or fewer.");
                continue;
            }

            return value;
        }
    }
}
