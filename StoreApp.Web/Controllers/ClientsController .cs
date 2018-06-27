﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Domain.Entity;
using StoreApp.Domain.Repository.Interfaces;
using StoreApp.Infra.DataBase.SessionFactory;
using StoreApp.Infra.DataBase.UnitOfWork;
using StoreApp.Infra.Exceptions;
using StoreApp.Web.Models;
using Microsoft.Extensions.DependencyInjection;

namespace StoreApp.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class ClientsController : Controller
    {
        private IClientRepository _clientRepsitory;

        public ClientsController(IClientRepository clientRespo)
        {
            _clientRepsitory = clientRespo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var clients = _clientRepsitory.FindAllWithContacts();
            return Ok(CreateClients(clients));
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var cli = _clientRepsitory.GetById(id);

            if (cli == null)
                throw new ErrorException(string.Format("Client {0} was not found.", id));

            return Ok(CreateClient(cli));
        }

        [HttpPost]
        public IActionResult Save([FromBody]ClientModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Name))
            {
                throw new ErrorException("Client has missing fields.");
            }

            using (var unit = UnitOfWork.Start(HttpContext.RequestServices.GetService<ISessionFactoryInfra>()))
            {
                var cli = CreateClient(model);
                _clientRepsitory.SaveOrUpdate(cli);
                unit.Commit();
            }

            return Ok();
        }

        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            using (var unit = UnitOfWork.Start(HttpContext.RequestServices.GetService<ISessionFactoryInfra>()))
            {
                var cli = _clientRepsitory.GetById(id);
                if (cli != null)
                {
                    _clientRepsitory.Delete(cli);
                    unit.Commit();
                }
                else
                {
                    throw new ErrorException("Client not found.");
                }

                return Ok();
            }
        }

        private Client CreateClient(ClientModel model)
        {
            Client client;
            if (model.Id != 0)
            {
                client = _clientRepsitory.GetById(model.Id);
                if (client == null)
                {
                    throw new ErrorException(string.Format("Client {0} not found.", model.Id));
                }
            }
            else
            {
                client = new Client();
            }

            client.Name = model.Name;
            client.Number = model.Number;
            client.Address = model.Address;
            client.City = model.City;
            client.State = model.State;
            client.IsActive = model.IsActive;

            client.Contacts.Clear();

            foreach (var contactModel in model.Contacts)
            {
                var contact = new ClientContact();
                contact.Name = contactModel.Name;
                contact.Email = contactModel.Email;
                contact.PhoneNumber = contactModel.PhoneNumber;
                client.Contacts.Add(contact);
            }

            return client;
        }

        #region helpers
        private IList<ClientModel> CreateClients(IList<Client> clients)
        {
            var list = new List<ClientModel>();

            foreach (var product in clients)
            {
                list.Add(CreateClient(product));
            }

            return list;
        }

        private ClientModel CreateClient(Client client)
        {
            var contacts = new List<ClientContactModel>();

            foreach (var item in client.Contacts)
            {
                contacts.Add(new ClientContactModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    PhoneNumber = item.Email
                });
            }

            return new ClientModel
            {
                Id = client.Id,
                Name = client.Name,
                Number = client.Number,
                Address = client.Address,
                City = client.City,
                State = client.State,
                IsActive = client.IsActive,
                Contacts = contacts
            };
        }
        #endregion
    }
}
