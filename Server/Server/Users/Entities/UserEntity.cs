﻿using Server.Shared;

namespace Server.Users.Entities
{
    public class UserEntity:BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
