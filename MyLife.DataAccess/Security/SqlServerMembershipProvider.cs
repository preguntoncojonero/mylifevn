using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;
using MyLife.Web;
using MyLife.Web.Security;

namespace MyLife.DataAccess.Security
{
    public class SqlServerMembershipProvider : MembershipProvider
    {
        private const int PASSWORD_SIZE = 14;
        private bool enablePasswordReset;
        private bool enablePasswordRetrieval;
        private string hashAlgorithmType;
        private int minRequiredNonalphanumericCharacters;
        private int minRequiredPasswordLength;
        private MembershipPasswordFormat passwordFormat;
        private string passwordStrengthRegularExpression;
        private bool requiresQuestionAndAnswer;
        private bool requiresUniqueEmail;

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonalphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return requiresUniqueEmail; }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            SecUtility.CheckParameter(ref username, true, true, true, 255, "username");
            SecUtility.CheckParameter(ref oldPassword, true, true, false, 128, "oldPassword");
            SecUtility.CheckParameter(ref newPassword, true, true, false, 128, "newPassword");

            var context = new MyLifeEntities();
            var user = context.tblUsers.Where(item => item.UserName == username).FirstOrDefault();
            if (user == null)
            {
                return false;
            }

            var encodedOldPassword =
                EncodePassword(oldPassword.ToLowerInvariant(), user.PasswordFormat, user.PasswordSalt);

            if (!user.Password.Equals(encodedOldPassword))
            {
                return false;
            }

            if (newPassword.Length < MinRequiredPasswordLength)
            {
                throw new ArgumentException(
                    string.Format("The length of parameter '{0}' needs to be greater or equal to '{1}'.", "newPassword",
                                  MinRequiredPasswordLength));
            }

            var count = 0;
            for (var i = 0; i < newPassword.Length; i++)
            {
                if (!char.IsLetterOrDigit(newPassword, i))
                {
                    count++;
                }
            }

            if (count < MinRequiredNonAlphanumericCharacters)
            {
                throw new ArgumentException(
                    string.Format("Non alpha numeric characters in '{0}' needs to be greater than or equal to '{1}'.",
                                  "newPassword",
                                  MinRequiredNonAlphanumericCharacters.ToString(CultureInfo.InvariantCulture)));
            }

            if (PasswordStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(newPassword, PasswordStrengthRegularExpression))
                {
                    throw new ArgumentException(
                        string.Format(
                            "The parameter '{0}' does not match the regular expression specified in config file.",
                            "newPassword"));
                }
            }

            var encodedNewPassword =
                EncodePassword(newPassword.ToLowerInvariant(), user.PasswordFormat, user.PasswordSalt);
            if (encodedNewPassword.Length > 128)
            {
                throw new ArgumentException("The password is too long: it must not exceed 128 chars after encrypting.");
            }

            var e = new ValidatePasswordEventArgs(username, newPassword, false);
            OnValidatingPassword(e);

            if (e.Cancel)
            {
                if (e.FailureInformation != null)
                {
                    throw e.FailureInformation;
                }
                throw new ArgumentException("The custom password validation failed.");
            }

            user.Password = encodedNewPassword;
            user.LastPasswordChangedDate = DateTime.UtcNow;

            try
            {
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                             string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email,
                                                  string passwordQuestion, string passwordAnswer, bool isApproved,
                                                  object providerUserKey, out MembershipCreateStatus status)
        {
            if (!SecUtility.ValidateParameter(ref password, true, true, false, 128))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            var salt = GenerateSalt();
            var encodedPassword = EncodePassword(password.ToLowerInvariant(), (int) PasswordFormat, salt);
            if (encodedPassword.Length > 128)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            string encodedPasswordAnswer;

            if (!string.IsNullOrEmpty(passwordAnswer))
            {
                passwordAnswer = passwordAnswer.Trim();
                if (passwordAnswer.Length > 128)
                {
                    status = MembershipCreateStatus.InvalidAnswer;
                    return null;
                }
                encodedPasswordAnswer = EncodePassword(passwordAnswer.ToLowerInvariant(), (int) PasswordFormat, salt);
            }
            else
            {
                encodedPasswordAnswer = passwordAnswer;
            }

            if (!SecUtility.ValidateParameter(ref encodedPasswordAnswer, RequiresQuestionAndAnswer, true, false, 128))
            {
                status = MembershipCreateStatus.InvalidAnswer;
                return null;
            }

            if (!SecUtility.ValidateParameter(ref username, true, true, true, 255))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }

            if (!SecUtility.ValidateParameter(ref email, RequiresUniqueEmail, RequiresUniqueEmail, false, 255))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            if (!SecUtility.ValidateParameter(ref passwordQuestion, RequiresQuestionAndAnswer, true, false, 255))
            {
                status = MembershipCreateStatus.InvalidQuestion;
                return null;
            }

            if (password.Length < MinRequiredPasswordLength)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            var count = 0;

            for (var i = 0; i < password.Length; i++)
            {
                if (!char.IsLetterOrDigit(password, i))
                {
                    count++;
                }
            }

            if (count < MinRequiredNonAlphanumericCharacters)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (PasswordStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(password, PasswordStrengthRegularExpression))
                {
                    status = MembershipCreateStatus.InvalidPassword;
                    return null;
                }
            }

            var e = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(e);

            if (e.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            var context = new MyLifeEntities();
            var user = context.tblUsers.Where(item => item.UserName == username).FirstOrDefault();
            if (user != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            if (RequiresUniqueEmail)
            {
                user = context.tblUsers.Where(item => item.Email == email).FirstOrDefault();
                if (user != null)
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
            }

            user = new tblUsers
                       {
                           UserName = username.ToLowerInvariant(),
                           Password = encodedPassword,
                           PasswordFormat = ((int) PasswordFormat),
                           PasswordSalt = salt,
                           Email = email.ToLowerInvariant(),
                           IsApproved = isApproved,
                           IsLockedOut = false,
                           CreatedDate = DateTime.UtcNow,
                           LastLoginDate = Constants.DateTime.MinSqlDate,
                           LastPasswordChangedDate = Constants.DateTime.MinSqlDate,
                           LastLockoutDate = Constants.DateTime.MinSqlDate
                       };

            try
            {
                context.AddTotblUsers(user);
                context.SaveChanges();
                status = MembershipCreateStatus.Success;
                return Convert(user);
            }
            catch
            {
                status = MembershipCreateStatus.UserRejected;
            }
            return null;
        }

        protected static string GenerateSalt()
        {
            var buf = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(buf);
            return System.Convert.ToBase64String(buf);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            var context = new MyLifeEntities();
            var user = context.tblUsers.Where(item => item.UserName == username).FirstOrDefault();
            if (user != null)
            {
                context.DeleteObject(user);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            var context = new MyLifeEntities();
            totalRecords = context.tblUsers.Where(item => item.Email.Contains(emailToMatch)).Count();
            var list =
                context.tblUsers.Where(item => item.Email.Contains(emailToMatch)).OrderBy(item => item.Email).Skip(
                    pageIndex*pageSize).Take(pageSize).ToList();
            return Convert(list);
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {
            var context = new MyLifeEntities();
            totalRecords = context.tblUsers.Where(item => item.UserName.Contains(usernameToMatch)).Count();
            var list =
                context.tblUsers.Where(item => item.UserName.Contains(usernameToMatch)).OrderBy(item => item.UserName).
                    Skip(
                    pageIndex*pageSize).Take(pageSize).ToList();
            return Convert(list);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var context = new MyLifeEntities();
            totalRecords = context.tblUsers.Count();
            var list = context.tblUsers.OrderBy(item => item.UserName).Skip(pageIndex*pageSize).Take(pageSize).ToList();
            return Convert(list);
        }

        private MembershipUserCollection Convert(IEnumerable<tblUsers> list)
        {
            var retval = new MembershipUserCollection();
            foreach (var obj in list)
            {
                retval.Add(Convert(obj));
            }
            return retval;
        }

        private MembershipUser Convert(tblUsers obj)
        {
            if (obj == null)
            {
                return null;
            }
            return new MembershipUser(Name, obj.UserName, obj.Id, obj.Email, null, null,
                                      obj.IsApproved, obj.IsLockedOut, obj.CreatedDate, obj.LastLoginDate,
                                      obj.LastLoginDate, obj.LastPasswordChangedDate, obj.LastLockoutDate);
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "SqlServerMembershipProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "DbEntry membership provider.");
            }

            base.Initialize(name, config);

            enablePasswordRetrieval = SecUtility.GetBooleanValue(config, "enablePasswordRetrieval", false);
            enablePasswordReset = SecUtility.GetBooleanValue(config, "enablePasswordReset", true);
            requiresQuestionAndAnswer = SecUtility.GetBooleanValue(config, "requiresQuestionAndAnswer", false);
            requiresUniqueEmail = SecUtility.GetBooleanValue(config, "requiresUniqueEmail", true);
            minRequiredPasswordLength = SecUtility.GetIntValue(config, "minRequiredPasswordLength", 3, false, 128);
            minRequiredNonalphanumericCharacters =
                SecUtility.GetIntValue(config, "minRequiredNonalphanumericCharacters", 0, true, 128);
            hashAlgorithmType = config["hashAlgorithmType"];
            if (String.IsNullOrEmpty(hashAlgorithmType))
            {
                hashAlgorithmType = "SHA1";
            }

            passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"];
            if (passwordStrengthRegularExpression != null)
            {
                passwordStrengthRegularExpression = passwordStrengthRegularExpression.Trim();
                if (passwordStrengthRegularExpression.Length != 0)
                {
                    try
                    {
                        new Regex(passwordStrengthRegularExpression);
                    }
                    catch (ArgumentException e)
                    {
                        throw new ProviderException(e.Message, e);
                    }
                }
            }
            else
            {
                passwordStrengthRegularExpression = string.Empty;
            }

            var strTemp = config["passwordFormat"] ?? "Hashed";

            switch (strTemp)
            {
                case "Clear":
                    passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                case "Encrypted":
                    passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Hashed":
                    passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                default:
                    throw new ProviderException("Bad password format");
            }

            if (passwordFormat == MembershipPasswordFormat.Hashed && enablePasswordRetrieval)
                throw new ProviderException("Provider cannot retrieve hashed password");

            config.Remove("cacheProvider");
            config.Remove("enablePasswordRetrieval");
            config.Remove("enablePasswordReset");
            config.Remove("requiresQuestionAndAnswer");
            config.Remove("applicationName");
            config.Remove("requiresUniqueEmail");
            config.Remove("maxInvalidPasswordAttempts");
            config.Remove("passwordAttemptWindow");
            config.Remove("passwordFormat");
            config.Remove("name");
            config.Remove("description");
            config.Remove("minRequiredPasswordLength");
            config.Remove("minRequiredNonalphanumericCharacters");
            config.Remove("passwordStrengthRegularExpression");
            config.Remove("hashAlgorithmType");
            if (config.Count > 0)
            {
                var attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                    throw new ProviderException("Provider unrecognized attribute: " + attribUnrecognized);
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            var utc = DateTime.UtcNow.AddMinutes(-Membership.UserIsOnlineTimeWindow);
            var context = new MyLifeEntities();
            return context.tblUsers.Where(item => item.LastLoginDate >= utc).Count();
        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new NotSupportedException(
                    "This Membership Provider has not been configured to support password retrieval.");
            }

            SecUtility.CheckParameter(ref username, true, true, true, 255, "username");
            var context = new MyLifeEntities();
            var user = context.tblUsers.Where(item => item.UserName == username).FirstOrDefault();
            if (user == null)
            {
                return null;
            }

            return UnEncodePassword(user.Password, user.PasswordFormat);
        }

        protected string UnEncodePassword(string pass, int format)
        {
            switch (format)
            {
                case 0:
                    return pass;

                case 1:
                    throw new ProviderException("Hashed passwords cannot be decoded.");
            }
            var encodedPassword = System.Convert.FromBase64String(pass);
            var bytes = DecryptPassword(encodedPassword);
            if (bytes == null)
            {
                return null;
            }
            return Encoding.Unicode.GetString(bytes, 0x10, bytes.Length - 0x10);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            SecUtility.CheckParameter(ref username, true, false, true, 255, "username");
            var context = new MyLifeEntities();
            var user = context.tblUsers.Where(item => item.UserName == username).FirstOrDefault();
            if (user == null)
            {
                return null;
            }

            return Convert(user);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
            {
                throw new ArgumentNullException("providerUserKey");
            }

            if (!(providerUserKey is long))
            {
                throw new ArgumentException(
                    "The provider user key supplied is invalid.  It must be of type System.Int64.");
            }

            var context = new MyLifeEntities();
            var id = (long) providerUserKey;
            var user = context.tblUsers.Where(item => item.Id == id).FirstOrDefault();
            return Convert(user);
        }

        public override string GetUserNameByEmail(string email)
        {
            var context = new MyLifeEntities();
            var username =
                context.tblUsers.Where(item => item.Email == email).Select(item => item.UserName).FirstOrDefault();
            return username;
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException(
                    "This provider is not configured to allow password resets. To enable password reset, set enablePasswordReset to \"true\" in the configuration file.");
            }

            SecUtility.CheckParameter(ref username, true, true, true, 255, "username");
            var context = new MyLifeEntities();
            var user = context.tblUsers.Where(item => item.UserName == username).FirstOrDefault();
            if (user.IsLockedOut || !user.IsApproved)
                return null;

            var newPassword = GeneratePassword().ToLowerInvariant();
            var newEncodedPassword =
                EncodePassword(newPassword, user.PasswordFormat, user.PasswordSalt);
            var e = new ValidatePasswordEventArgs(username, newPassword, false);
            OnValidatingPassword(e);
            if (e.Cancel)
            {
                if (e.FailureInformation != null)
                {
                    throw e.FailureInformation;
                }
                throw new ProviderException("The custom password validation failed.");
            }

            user.Password = newEncodedPassword;
            user.LastPasswordChangedDate = DateTime.UtcNow;

            try
            {
                context.SaveChanges();
                return newPassword;
            }
            catch
            {
                return null;
            }
        }

        public virtual string GeneratePassword()
        {
            return Membership.GeneratePassword(
                MinRequiredPasswordLength < PASSWORD_SIZE ? PASSWORD_SIZE : MinRequiredPasswordLength,
                MinRequiredNonAlphanumericCharacters);
        }

        public override bool UnlockUser(string username)
        {
            SecUtility.CheckParameter(ref username, true, true, true, 255, "username");
            var context = new MyLifeEntities();
            var user = context.tblUsers.Where(item => item.UserName == username).FirstOrDefault();
            user.IsLockedOut = false;

            try
            {
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            var temp = user.Email;
            SecUtility.CheckParameter(ref temp, true, true, false, 255, "Email");
            user.Email = temp;
            var context = new MyLifeEntities();
            var id = (int) user.ProviderUserKey;
            var obj = context.tblUsers.Where(item => item.Id == id).FirstOrDefault();
            if (requiresUniqueEmail)
            {
                obj = context.tblUsers.Where(item => item.Email == user.Email).FirstOrDefault();
                if (obj != null && !obj.UserName.Equals(user.UserName, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException(Messages.DuplicateEmail);
                }
            }
            obj.Email = user.Email.ToLowerInvariant();
            obj.IsApproved = user.IsApproved;
            obj.LastLoginDate = user.LastLoginDate.ToUniversalTime();
            context.SaveChanges();
        }

        public override bool ValidateUser(string username, string password)
        {
            if (!SecUtility.ValidateParameter(ref username, true, true, true, 255))
                return false;

            if (!SecUtility.ValidateParameter(ref password, true, true, false, 128))
                return false;

            var context = new MyLifeEntities();
            var user = context.tblUsers.Where(item => item.UserName == username).FirstOrDefault();
            if (user == null)
            {
                return false;
            }

            if (!user.IsApproved)
                return false;

            if (user.IsLockedOut)
                return false;

            var encodedPassword = EncodePassword(password.ToLowerInvariant(), user.PasswordFormat, user.PasswordSalt);
            var isPasswordCorrect = user.Password.Equals(encodedPassword);
            var dt = DateTime.UtcNow;

            if (isPasswordCorrect)
                user.LastLoginDate = dt;

            context.SaveChanges();

            return isPasswordCorrect;
        }

        protected string EncodePassword(string pass, int format, string salt)
        {
            if (format == 0)
                return pass;

            var bIn = Encoding.Unicode.GetBytes(pass);
            var bSalt = System.Convert.FromBase64String(salt);
            var bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet;

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
            if (format == 1)
            {
                var s = HashAlgorithm.Create(Membership.HashAlgorithmType);
                bRet = s.ComputeHash(bAll);
            }
            else
            {
                bRet = EncryptPassword(bAll);
            }

            return System.Convert.ToBase64String(bRet);
        }

        public MembershipUser GetUserByEmail(string email)
        {
            var context = new MyLifeEntities();
            var user = context.tblUsers.Where(item => item.Email == email).FirstOrDefault();
            return user == null ? null : Convert(user);
        }
    }
}