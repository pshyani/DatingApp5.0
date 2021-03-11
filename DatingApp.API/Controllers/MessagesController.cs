using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Interfaces;
using DatingApp.API.DTO;
using AutoMapper;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;

namespace DatingApp.API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MessagesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var username = User.GetUserName();

            if (username == createMessageDTO.RecipientUserName.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _unitOfWork.UserRepository.GetUsersByUserNameAsync(username);
            var recipient = await _unitOfWork.UserRepository.GetUsersByUserNameAsync(createMessageDTO.RecipientUserName);

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

            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.Complete())
                return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to create message");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();

            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();

            if (message.Sender.UserName == username)
                message.SenderDeleted = true;

            if (message.Recipient.UserName == username)
                message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                _unitOfWork.MessageRepository.DeleteMessage(message);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("problem deleting the message");
        }

    }
}