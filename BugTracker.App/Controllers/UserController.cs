﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using BugTracker.App.Commands.Repository.Abstract;
using BugTracker.App.Controllers.Abstract;
using BugTracker.App.Models;
using BugTracker.Data.Repositories.Abstract;
using BugTracker.Shared.Command.Utils.Abstract;

namespace BugTracker.App.Controllers
{
    public class UserController : ApiControllerBase
    {
        private readonly ICommandRepository commandRepository;
        private readonly ICommandExecutor commandExecutor;
        private readonly IUserAccess userAccess;

        public UserController(ICommandRepository commandRepository, ICommandExecutor commandExecutor, IUserAccess userAccess)
        {
            this.commandRepository = commandRepository;
            this.commandExecutor = commandExecutor;
            this.userAccess = userAccess;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Register(RegisterUserModel registrationModel)
        {
            HttpResponseMessage response;
            if (registrationModel == null)
            {
                response = this.CreateErrorResponse("The registrationModel is not set.", HttpStatusCode.BadRequest);
                return response;
            }

            var registerNewUserCommand = this.commandRepository.RegisterNewUser(registrationModel);
            var commandResult = await this.commandExecutor.ExecuteAsync(registerNewUserCommand);

            response = this.CreateResponseFromCommandResult(commandResult);
            return response;
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            HttpResponseMessage response;
            if (id == Guid.Empty)
            {
                response = this.CreateErrorResponse($"The id '{id}' is invalid.", HttpStatusCode.BadRequest);
                return response;
            }

            var maybeUser = this.userAccess.TryGetById(id);

            if (maybeUser.HasNoValue)
            {
                response = this.CreateErrorResponse($"The user with id '{id}' does not exist.", HttpStatusCode.NotFound);
                return response;
            }

            return this.CreateResponse(maybeUser.Value);
        }
    }
}