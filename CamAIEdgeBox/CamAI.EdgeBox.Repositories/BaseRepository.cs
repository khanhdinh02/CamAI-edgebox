using System.Linq.Expressions;
using CamAI.EdgeBox.Models;
using Microsoft.EntityFrameworkCore;

namespace CamAI.EdgeBox.Repositories;

public class BaseRepository<T>(CamAiEdgeBoxContext context) where T : BaseEntity
{
    protected DbContext Context => context;

    public virtual T Add(T entity)
    {
        var result = Context.Set<T>().Add(entity);
        return result.Entity;
    }

    public virtual int Count(Expression<Func<T, bool>>? expression = null)
    {
        return expression == null ? Context.Set<T>().Count() : Context.Set<T>().Count(expression);
    }

    public virtual T Delete(T entity)
    {
        if (Context.Entry(entity).State == EntityState.Detached)
        {
            Context.Attach(entity);
            Context.Entry(entity).State = EntityState.Deleted;
        }
        return entity;
    }

    public virtual List<T> GetAll()
    {
        return Context.Set<T>().ToList();
    }

    public virtual T? GetById(object key)
    {
        return Context.Set<T>().Find(key);
    }

    public bool IsExisted(object key)
    {
        var data = context.Set<T>().Find(key);
        return data != null;
    }

    public virtual T Update(T entity)
    {
        if (Context.Entry(entity).State == EntityState.Detached)
        {
            Context.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }
        return entity;
    }
}