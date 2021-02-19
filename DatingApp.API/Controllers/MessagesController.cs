using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DatingApp.API.Interfaces;
using DatingApp.API.DTO;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;

namespace DatingApp.API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository,
                                IMessageRepository messageRepository,
                                IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var userName = User.GetUserName();

            if (userName == createMessageDTO.RecipientUserName.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUsersByUserNameAsync(userName);
            var recipient = await _userRepository.GetUsersByUserNameAsync(createMessageDTO.RecipientUserName);

            if (recipient == null)
                return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDTO.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
                return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to create message");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
            var currentUserName = User.GetUserName();

            return Ok(await _messageRepository.GetMessageThread(currentUserName, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();

            var message = await _messageRepository.GetMessage(id);

            if(message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();

            if(message.Sender.UserName == username)
                message.SenderDeleted = true;

            if(message.Recipient.UserName == username)
                message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted) 
                _messageRepository.DeleteMessage(message);

            if(await _messageRepository.SaveAllAsync())
                return Ok();

            return BadRequest("problem deleting the message");
        }

    }
}