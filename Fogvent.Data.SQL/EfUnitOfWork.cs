﻿using System;
using System.Data.Entity;
using Fogvent.Data.Common;
using Fogvent.Models.Entities;

namespace Fogvent.Data.SQL
{
    public class EfUnitOfWork : IUnitOfWork
    {
        #region Fields

        private readonly DbContext _context;

        #endregion

        #region Constructor
        public EfUnitOfWork()
        {
            DbContext context = new AppContext();
            if (context == null) throw new Exception("Context cannot be null");

            _context = context;
        }

        #endregion

        #region Interface Implementation
        public IRepository<TEntity> RepositoryFor<TEntity>() where TEntity : EntityBase
        {
            return new EfRepository<TEntity>();
        }

        public int SaveChanges()
        {
            foreach (var entry in _context.ChangeTracker.Entries<EntityBase>())
            {
                var entity = entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedOn = DateTime.Now;
                    entity.ModifiedOn = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                    entity.ModifiedOn = DateTime.Now;
            }
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        #endregion
    }
}
