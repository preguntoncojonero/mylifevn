using System.Collections.Generic;
using System.Linq;
using xVal.RuleProviders;
using xVal.Rules;

namespace MyLife.Validation
{
    public static class Validates
    {
        public static RuleSet GetContactRuleSet()
        {
            var rules = new List<KeyValuePair<string, Rule>>
                            {
                                new KeyValuePair<string, Rule>("Name", new RequiredRule{ ErrorMessage = "Bạn hãy nhập họ và tên của bạn"}),
                                new KeyValuePair<string, Rule>("Email", new RequiredRule{ ErrorMessage = "Bạn hãy nhập địa chỉ email của bạn"}),
                                new KeyValuePair<string, Rule>("Email", new RegularExpressionRule(Constants.Regulars.Email) {ErrorMessage = "Địa chỉ email của bạn không hợp lệ"}),
                                new KeyValuePair<string, Rule>("Content", new RequiredRule{ ErrorMessage = "Bạn hãy nhập nội dung phản hồi của bạn"}),
                            };
            return new RuleSet(rules.ToLookup(k => k.Key, v => v.Value));
        }

        public static RuleSet GetResetPasswordRuleSet()
        {
            var rules = new List<KeyValuePair<string, Rule>>
                            {
                                new KeyValuePair<string, Rule>("UserName", new RequiredRule{ ErrorMessage = "Bạn chưa nhập địa chỉ tên đăng nhập của bạn"})
                            };
            return new RuleSet(rules.ToLookup(k => k.Key, v => v.Value));
        }

        public static RuleSet GetLoginRuleSet()
        {
            var rules = new List<KeyValuePair<string, Rule>>
                            {
                                new KeyValuePair<string, Rule>("UserName", new RequiredRule{ ErrorMessage = "Bạn chưa nhập tên đăng nhập của bạn"}),
                                new KeyValuePair<string, Rule>("Password", new RequiredRule{ ErrorMessage = "Bạn chưa nhập mật khẩu"})
                            };
            return new RuleSet(rules.ToLookup(k => k.Key, v => v.Value));
        }

        public static RuleSet GetChangePasswordRuleSet()
        {
            var rules = new List<KeyValuePair<string, Rule>>
                            {
                                new KeyValuePair<string, Rule>("OldPassword", new RequiredRule{ ErrorMessage = "Bạn chưa nhập mật khẩu cũ"}),
                                new KeyValuePair<string, Rule>("NewPassword", new RequiredRule{ ErrorMessage = "Bạn chưa nhập mật khẩu mới"}),
                                new KeyValuePair<string, Rule>("ConfirmNewPassword", new RequiredRule{ ErrorMessage = "Mật khẩu xác nhận không giống nhau"}),
                                new KeyValuePair<string, Rule>("ConfirmNewPassword", new ComparisonRule("NewPassword", ComparisonRule.Operator.Equals){ ErrorMessage = "Mật khẩu xác nhận không giống nhau"})
                            };
            return new RuleSet(rules.ToLookup(k => k.Key, v => v.Value));
        }

        public static RuleSet GetRegisterRuleSet()
        {
            var rules = new List<KeyValuePair<string, Rule>>
                            {
                                new KeyValuePair<string, Rule>("UserName", new RequiredRule{ ErrorMessage = "Bạn chưa nhập tên đăng nhập"}),
                                new KeyValuePair<string, Rule>("UserName", new RegularExpressionRule(Constants.Regulars.User){ ErrorMessage = "Tên đăng nhập chỉ chấp nhận chữ cái, số và dấu _"}),
                                new KeyValuePair<string, Rule>("Password", new RequiredRule{ ErrorMessage = "Bạn chưa nhập mật khẩu"}),
                                new KeyValuePair<string, Rule>("ConfirmPassword", new RequiredRule{ ErrorMessage = "Mật khẩu xác nhận không giống nhau"}),
                                new KeyValuePair<string, Rule>("ConfirmPassword", new ComparisonRule("Password", ComparisonRule.Operator.Equals){ ErrorMessage = "Mật khẩu xác nhận không giống nhau"}),
                                new KeyValuePair<string, Rule>("Email", new RequiredRule{ ErrorMessage = "Bạn chưa nhập địa chỉ email của bạn"}),
                                new KeyValuePair<string, Rule>("Email", new RegularExpressionRule(Constants.Regulars.Email) {ErrorMessage = "Địa chỉ email của bạn không hợp lệ"})
                            };
            return new RuleSet(rules.ToLookup(k => k.Key, v => v.Value));
        }
    }
}