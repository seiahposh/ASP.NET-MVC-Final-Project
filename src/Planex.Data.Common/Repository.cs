﻿namespace Planex.Data.Common
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    using Planex.Data.Common.Models;

    // TODO: Why BaseModel<int> instead BaseModel<TKey>?
    public class Repository<T, TKey> : IRepository<T, TKey>
        where T : BaseModel<TKey>, IHavePrimaryKey<TKey> where TKey : struct
    {
        public Repository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException(
                    "An instance of DbContext is required to use this repository.", 
                    nameof(context));
            }

            this.Context = context;
            this.DbSet = this.Context.Set<T>();
        }

        private DbContext Context { get; }

        private IDbSet<T> DbSet { get; }

        public void Add(T entity)
        {
            this.DbSet.Add(entity);
            this.Save();
        }

        public IQueryable<T> All()
        {
            return this.DbSet.Where(x => !x.IsDeleted);
        }

        public IQueryable<T> AllWithDeleted()
        {
            return this.DbSet;
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.Now;
            this.Save();
        }

        public virtual void Delete(TKey id)
        {
            var entity = this.GetById(id);

            if (entity != null)
            {
                this.Delete(entity);
            }

            this.Save();
        }

        public T GetById(TKey id)
        {
            return this.DbSet.Find(id);
        }

        public void HardDelete(T entity)
        {
            this.DbSet.Remove(entity);
            this.Save();
        }

        public void Save()
        {
            this.Context.SaveChanges();
        }

        public void Update(T entity)
        {
            DbEntityEntry entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
            this.Save();
        }
    }
}