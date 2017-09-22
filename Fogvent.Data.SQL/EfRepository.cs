﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fogvent.Data.Common;
using Fogvent.Models.Entities;

namespace Fogvent.Data.SQL
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        #region Fields

        protected readonly DbContext Context;
        private readonly DbSet<TEntity> _entitySet;

        #endregion

        #region Constructor

        public EfRepository(DbContext context)
        {
            if (context == null) throw new Exception("Context cannot be null");

            Context = context;
            Context.Database.CommandTimeout = 180;
            _entitySet = context.Set<TEntity>();
        }

        #endregion

        #region Interface Methods

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, IList<Expression<Func<TEntity,
            object>>> includedProperties = null, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<TEntity> entities = _entitySet;
            //Filtering
            if (filter != null) entities = entities.Where(filter);

            //Sorting
            orderBy?.Invoke(entities);

            //Including
            if (includedProperties!=null)
            {
                foreach (var property in includedProperties)
                    entities = entities.Include(property);
            }
            
            //Paging
            if (pageIndex.HasValue && pageSize.HasValue) entities = entities.Skip(pageSize.Value * pageIndex.Value).Take(pageSize.Value);

            return entities;
        }
        public async Task<ICollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, IList<Expression<Func<TEntity, object>>> includedProperties = null, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<TEntity> entities = _entitySet;
            //Filtering
            if (filter != null) entities = entities.Where(filter);

            //Sorting
            orderBy?.Invoke(entities);

            //Including
            if (includedProperties != null)
            {
                foreach (var property in includedProperties)
                    entities = entities.Include(property);
            }

            //Paging
            if (pageIndex.HasValue && pageSize.HasValue) entities = entities.Skip(pageSize.Value * pageIndex.Value).Take(pageSize.Value);

            return await entities.ToListAsync();
        }

        public TEntity GetById(object id)
        {
            return _entitySet.Find(id);
        }
        public async Task<TEntity> GetAsyncById(object id)
        {
            return await _entitySet.FindAsync(id);
        }
        

        public TEntity Insert(TEntity entity)
        {
            return _entitySet.Add(entity);
        }

        public IEnumerable<TEntity> BulkInsert(IEnumerable<TEntity> entities)
        {
            return _entitySet.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            _entitySet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            var entity = _entitySet.Find(where);
            if (entity != null)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                    _entitySet.Attach(entity);

                _entitySet.Remove(entity);
            }
        }

        public void BulkDelete(IQueryable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                    _entitySet.Attach(entity);
            }

            _entitySet.RemoveRange(entities.AsEnumerable());
        }

        public void Detach(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        #endregion


    }
}