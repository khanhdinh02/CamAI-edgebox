using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services.Utils;

namespace CamAI.EdgeBox.Services;

public static class AuthService
{
    public static bool Login(string username, string password) =>
        username == "admin" && CheckPassword(password);

    public static bool ChangePassword(string oldPassword, string newPassword)
    {
        if (!CheckPassword(oldPassword))
            return false;

        var hashPassword = Hasher.Hash(newPassword);
        AuthRepository.UpdatePassword(hashPassword);
        return true;
    }

    private static bool CheckPassword(string password)
    {
        var hashedPassword = AuthRepository.GetPassword();
        if (hashedPassword == "")
            return password == GlobalData.EdgeBox!.Id.ToString("N");

        return Hasher.Verify(password, hashedPassword);
    }
}
