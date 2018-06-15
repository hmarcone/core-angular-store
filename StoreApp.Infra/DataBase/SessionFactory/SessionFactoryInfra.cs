﻿using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Cfg;
using StoreApp.Infra.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Infra.DataBase.SessionFactory
{
    internal class SessionFactoryInfra : ISessionFactoryInfra
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly ISession _session;

        public SessionFactoryInfra()
        {
            var assemblyWithMaps = AssemblyLocator.GetByName("StoreApp.Domain.Map.dll");

            string connString = @"Server=127.0.0.1;Port=3306;Database=db_store;Uid=user_test;Pwd=password_test;SslMode=none";

            //Fliently configuration uwing MySql Connector and "StoreApp.Domain.Repository.dll" mapped assembly
            _sessionFactory = Fluently.Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(connString))
                .Mappings(m => m.FluentMappings.AddFromAssembly(assemblyWithMaps))
                .BuildSessionFactory();

            _session = _sessionFactory.OpenSession();
        }

        public ISession GetCurrentSession()
        {
            return _session;
        }

        public void Dispose()
        {
            _sessionFactory.Close();
            _sessionFactory.Dispose();
        }
    }
}