﻿namespace IdentityManagerServerApi.Models.DTOs
{
    public record UserSession(string? Id, string? Name, string? Email, string? Role);
}