﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using JetBrains.Annotations;
using Orchard.Data;
using Orchard.Logging;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Users.Drivers;
using Orchard.Users.Models;

namespace Orchard.Users.Services {
    [UsedImplicitly]
    public class MembershipService : IMembershipService {
        private readonly IContentManager _contentManager;
        private readonly IRepository<UserRecord> _userRepository;

        public MembershipService(IContentManager contentManager, IRepository<UserRecord> userRepository) {
            _contentManager = contentManager;
            _userRepository = userRepository;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public MembershipSettings GetSettings() {
            var settings = new MembershipSettings();
            // accepting defaults
            return settings;
        }

        public IUser CreateUser(CreateUserParams createUserParams) {
            Logger.Information("CreateUser {0} {1}", createUserParams.Username, createUserParams.Email);

            return _contentManager.Create<User>(UserDriver.ContentType.Name, init =>
            {
                init.Record.UserName = createUserParams.Username;
                init.Record.Email = createUserParams.Email;
                init.Record.NormalizedUserName = createUserParams.Username.ToLower();
                init.Record.HashAlgorithm = "SHA1";
                SetPassword(init.Record, createUserParams.Password);
            });
        }

        public IUser GetUser(string username) {
            var lowerName = username == null ? "" : username.ToLower();

            var userRecord = _userRepository.Get(x => x.NormalizedUserName == lowerName);
            if (userRecord == null) {
                return null;
            }
            return _contentManager.Get<IUser>(userRecord.Id);
        }

        public IUser ValidateUser(string userNameOrEmail, string password) {
            var lowerName = userNameOrEmail == null ? "" : userNameOrEmail.ToLower();

            var userRecord = _userRepository.Get(x => x.NormalizedUserName == lowerName);
            if(userRecord == null)
                userRecord = _userRepository.Get(x => x.Email == lowerName);
            if (userRecord == null || ValidatePassword(userRecord, password) == false)
                return null;

            return _contentManager.Get<IUser>(userRecord.Id);
        }



        public void SetPassword(IUser user, string password) {
            if (!user.Is<User>())
                throw new InvalidCastException();

            var userRecord = user.As<User>().Record;
            SetPassword(userRecord, password);
        }


        void SetPassword(UserRecord record, string password) {
            switch (GetSettings().PasswordFormat) {
                case MembershipPasswordFormat.Clear:
                    SetPasswordClear(record, password);
                    break;
                case MembershipPasswordFormat.Hashed:
                    SetPasswordHashed(record, password);
                    break;
                case MembershipPasswordFormat.Encrypted:
                    SetPasswordEncrypted(record, password);
                    break;
                default:
                    throw new ApplicationException("Unexpected password format value");
            }
        }

        private bool ValidatePassword(UserRecord record, string password) {
            // Note - the password format stored with the record is used
            // otherwise changing the password format on the site would invalidate
            // all logins
            switch (record.PasswordFormat) {
                case MembershipPasswordFormat.Clear:
                    return ValidatePasswordClear(record, password);
                case MembershipPasswordFormat.Hashed:
                    return ValidatePasswordHashed(record, password);
                case MembershipPasswordFormat.Encrypted:
                    return ValidatePasswordEncrypted(record, password);
                default:
                    throw new ApplicationException("Unexpected password format value");
            }
        }

        private static void SetPasswordClear(UserRecord record, string password) {
            record.PasswordFormat = MembershipPasswordFormat.Clear;
            record.Password = password;
            record.PasswordSalt = null;
        }

        private static bool ValidatePasswordClear(UserRecord record, string password) {
            return record.Password == password;
        }

        private static void SetPasswordHashed(UserRecord record, string password) {

            var saltBytes = new byte[0x10];
            var random = new RNGCryptoServiceProvider();
            random.GetBytes(saltBytes);

            var passwordBytes = Encoding.Unicode.GetBytes(password);

            var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();

            var hashAlgorithm = HashAlgorithm.Create(record.HashAlgorithm);
            var hashBytes = hashAlgorithm.ComputeHash(combinedBytes);

            record.PasswordFormat = MembershipPasswordFormat.Hashed;
            record.Password = Convert.ToBase64String(hashBytes);
            record.PasswordSalt = Convert.ToBase64String(saltBytes);
        }

        private static bool ValidatePasswordHashed(UserRecord record, string password) {

            var saltBytes = Convert.FromBase64String(record.PasswordSalt);

            var passwordBytes = Encoding.Unicode.GetBytes(password);

            var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();

            var hashAlgorithm = HashAlgorithm.Create(record.HashAlgorithm);
            var hashBytes = hashAlgorithm.ComputeHash(combinedBytes);

            return record.Password == Convert.ToBase64String(hashBytes);
        }

        private static void SetPasswordEncrypted(UserRecord record, string password) {
            throw new NotImplementedException();
        }

        private static bool ValidatePasswordEncrypted(UserRecord record, string password) {
            throw new NotImplementedException();
        }
    }
}
