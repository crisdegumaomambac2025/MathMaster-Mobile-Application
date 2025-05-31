using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathMaster.Views
{
    public static class SessionManager
    {
        public static int UserId { get; private set; }
        public static string FullName { get; private set; }
        public static string Username { get; private set; }
        public static string Role { get; private set; }
        public static string Email { get; private set; }

        public static bool IsLoggedIn => UserId != 0;

        public static void StartSession(int userId, string firstname, string lastname, string username, string role, string email)
        {
            UserId = userId;
            FullName = $"{firstname} {lastname}";
            Username = username;
            Role = role;
            Email = email;
        }

        public static void EndSession()
        {
            UserId = 0;
            FullName = null;
            Username = null;
            Role = null;
            Email = null;
        }
    }
}