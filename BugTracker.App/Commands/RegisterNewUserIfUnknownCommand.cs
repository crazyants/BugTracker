﻿using System.Threading.Tasks;

using BugTracker.App.Models;
using BugTracker.Data.Entities;
using BugTracker.Data.Repositories.Abstract;
using BugTracker.Shared.Command.Abstract;
using BugTracker.Shared.Command.Entities;
using BugTracker.Shared.Extensions;

namespace BugTracker.App.Commands
{
    internal class RegisterNewUserIfUnknownCommand : CommandBase<User>
    {
        private readonly IUserAccess userAccess;
        private RegisterUserModel registrationModel;

        public RegisterNewUserIfUnknownCommand(IUserAccess userAccess)
        {
            this.userAccess = userAccess;
        }

        public void Initialize(RegisterUserModel registrationModel)
        {
            this.registrationModel = registrationModel;
        }

        protected override Task<CanExecuteCommandResult> CanExecuteAsync()
        {
            if (this.registrationModel == null)
            {
                return this.CannotExecute("The registrationmodel is null.").ToTaskResult();
            }
            if (string.IsNullOrWhiteSpace(this.registrationModel.Username))
            {
                return this.CannotExecute($"The username '{this.registrationModel.Username}' is null or in a wrong format.").ToTaskResult();
            }
            this.registrationModel.Username = this.registrationModel.Username.Trim();

            return base.CanExecuteAsync();
        }

        protected override Task<CommandResult<User>> ExecuteAsync()
        {
            var usernameToRegister = this.registrationModel.Username;

            var maybeExistingUser = this.userAccess.TryGetByName(usernameToRegister);

            User registeredUser;
            if (maybeExistingUser.HasValue)
            {
                registeredUser = maybeExistingUser.Value;
                return this.SuccessExecution(registeredUser).ToTaskResult();
            }

            registeredUser = this.userAccess.Add(usernameToRegister);

            return this.SuccessExecution(registeredUser).ToTaskResult();
        }
    }
}