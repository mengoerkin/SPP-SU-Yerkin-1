using System;
using System.Collections.Generic;

// USER
// YAGNI: тек қажет қасиеттер ғана
class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}

// USER MANAGER
// KISS: қарапайым List<User>, артық абстракция жоқ
class UserManager
{
    private List<User> users = new List<User>();

    // DRY: қосу және жаңарту үшін ортақ логика
    private User FindByEmail(string email)
    {
        foreach (var user in users)
        {
            if (user.Email == email)
                return user;
        }
        return null;
    }

    public void AddUser(string name, string email, string role)
    {
        if (FindByEmail(email) != null)
            return;

        users.Add(new User
        {
            Name = name,
            Email = email,
            Role = role
        });
    }

    public void RemoveUser(string email)
    {
        User user = FindByEmail(email);
        if (user != null)
            users.Remove(user);
    }

    public void UpdateUser(string email, string name, string role)
    {
        User user = FindByEmail(email);
        if (user == null)
            return;

        user.Name = name;
        user.Role = role;
    }

    // Қарапайым көрсету (тексеру үшін)
    public void PrintUsers()
    {
        foreach (var user in users)
        {
            Console.WriteLine($"{user.Name} | {user.Email} | {user.Role}");
        }
    }
}

// MAIN(TEST)
class Program
{
    static void Main()
    {
        UserManager manager = new UserManager();

        manager.AddUser("Ahmed", "ahmed@gmail.com", "Admin");
        manager.AddUser("Anel", "anel@gmail.com", "User");

        manager.UpdateUser("anel@gmail.com", "Anel K.", "User");
        manager.RemoveUser("ahmed@gmail.com");

        manager.PrintUsers();
    }
}