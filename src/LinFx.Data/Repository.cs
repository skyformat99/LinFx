﻿using LinFx.Domain.Entities;
using LinFx.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace LinFx.Data
{
    public class Repository<TEntity> : RepositoryBase<TEntity> where TEntity : class, IEntity<string>
    {
        public Repository(IDbConnectionFactory factory) : base(factory) { }

        public override Task InsertAsync(TEntity item)
        {
            using (var db = _factory.Open())
            {
                return db.InsertAsync(item);
            }
        }

        public override Task UpdateAsync(TEntity item)
        {
            using (var db = _factory.Open())
            {
                return db.UpdateAsync(item);
            }
        }

        public override Task DeleteAsync(TEntity item)
        {
            using (var db = _factory.Open())
            {
                return db.DeleteAsync(item);
            }
        }

        public override Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public override Task<TEntity> GetAsync(string id)
        {
            using (var db = _factory.Open())
            {
                return db.GetAsync<TEntity>(id);
            }
        }

        public override Task<TEntity> FirstOrDefaultAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
