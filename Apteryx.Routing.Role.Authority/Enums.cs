using System.ComponentModel;

namespace Apteryx.Routing.Role.Authority
{
    public enum AddTypes
    {
        程序 = 101,
        人工
    }

    public enum CaptchaType
    {
        [Description("登录验证码")]
        Login,       // 登录验证码
        [Description("注册验证码")]
        Register,    // 注册验证码
        [Description("密码重置验证码")]
        PasswordReset // 密码重置验证码
    }
}
