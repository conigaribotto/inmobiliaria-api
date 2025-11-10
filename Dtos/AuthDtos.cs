namespace InmobiliariaAPI.Dtos;

public record LoginRequestDto(string Email, string Password);

public record LoginResponseDto(string Token, DateTime ExpiresUtc);
