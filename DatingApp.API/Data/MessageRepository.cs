using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.DTO;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext dataContext, IMapper mapper)
        {
            _mapper = mapper;
            _dataContext = dataContext;

        }
        public void AddMessage(Message message)
        {
            _dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _dataContext.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _dataContext.Messages
                            .Include(u => u.Sender)
                            .Include(u => u.Recipient)
                            .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _dataContext.Messages
                            .OrderByDescending(m => m.MessageSent)
                            .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.UserName 
                                        && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.UserName 
                                        && u.SenderDeleted == false),
                _ => query.Where(u => u.Recipient.UserName == messageParams.UserName 
                                        && u.RecipientDeleted == false && u.DateRead == null)

            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = await _dataContext.Messages
                                    .Include(u => u.Sender).ThenInclude(p => p.Photos)
                                    .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                                    .Where(m => m.Recipient.UserName == currentUserName
                                    && m.RecipientDeleted == false
                                    && m.Sender.UserName == recipientUserName 
                                    || m.Recipient.UserName == recipientUserName
                                    && m.Sender.UserName == currentUserName 
                                    && m.SenderDeleted == false
                                    )
                                    .OrderBy(m => m.MessageSent)
                                    .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && 
                                            m.Recipient.UserName == currentUserName).ToList();

            if(unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
            }

            await _dataContext.SaveChangesAsync();

            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
                                    
        }       

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}