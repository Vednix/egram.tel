﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Persistance;
using Tel.Egram.TdLib;

namespace Tel.Egram.Authentication
{
    public class Authenticator : IAuthenticator
    {
        private readonly TdAgent _agent;
        private readonly IStorage _storage;

        public Authenticator(
            TdAgent agent,
            IStorage storage
            )
        {
            _agent = agent;
            _storage = storage;
        }

        public IObservable<TdApi.AuthorizationState> ObserveState()
        {
            return _agent.Updates.OfType<TdApi.Update.UpdateAuthorizationState>()
                .Select(update => update.AuthorizationState);
        }
        
        public IObservable<Unit> SetupParameters()
        {   
            return _agent.Execute(new TdApi.SetTdlibParameters
                {
                    Parameters = new TdApi.TdlibParameters
                    {
                        UseTestDc = false,
                        DatabaseDirectory = _storage.TdLibDirectory,
                        FilesDirectory = _storage.TdLibDirectory,
                        UseFileDatabase = true,
                        UseChatInfoDatabase = true,
                        UseMessageDatabase = true,
                        UseSecretChats = true,
                        ApiId = 111112,
                        ApiHash = new Guid(new byte[]{ 142, 34, 97, 121, 94, 51, 206, 139, 4, 159, 245, 26, 236, 242, 11, 171 }).ToString("N"),
                        SystemLanguageCode = "en",
                        DeviceModel = "Mac",
                        SystemVersion = "0.1",
                        ApplicationVersion = "0.1",
                        EnableStorageOptimizer = true,
                        IgnoreFileNames = false
                    }
                })
                .Select(_ => Unit.Default);
        }

        public IObservable<Unit> CheckEncryptionKey()
        {
            return _agent.Execute(new TdApi.CheckDatabaseEncryptionKey())
                .Select(_ => Unit.Default);
        }

        public IObservable<Unit> SetPhoneNumber(string phoneNumber)
        {
            return _agent.Execute(new TdApi.SetAuthenticationPhoneNumber
                {
                    PhoneNumber = phoneNumber
                })
                .Select(_ => Unit.Default);
        }

        public IObservable<Unit> CheckCode(string code, string firstName, string lastName)
        {
            return _agent.Execute(new TdApi.CheckAuthenticationCode
                {
                    Code = code,
                    FirstName = firstName,
                    LastName = lastName
                })
                .Select(_ => Unit.Default);
        }

        public IObservable<Unit> CheckPassword(string password)
        {
            return _agent.Execute(new TdApi.CheckAuthenticationPassword
                {
                    Password = password
                })
                .Select(_ => Unit.Default);
        }
    }
}