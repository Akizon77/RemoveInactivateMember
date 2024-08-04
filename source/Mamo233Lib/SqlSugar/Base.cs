﻿using SqlSugar;
using System.Data;
using System.Linq.Expressions;

namespace Mamo233Lib.SqlSugar;

//code from XinjingdailyBot
public class SqlReopBase
{ }

public abstract class BaseRepository<T>(ISqlSugarClient context) : SimpleClient<T>(context) where T : SqlReopBase, new()
{
    public void InitHeader() => Context.CopyNew().CodeFirst.InitTables<T>();

    #region add
    /// <summary>
    /// 向数据库中添加实体。
    /// </summary>
    /// <inheritdoc/>
    public int Add(T t)
    {
        return Context.CopyNew().Insertable(t).IgnoreColumns(true).ExecuteCommand();
    }
    /// <summary>
    /// 向数据库中插入实体。
    /// </summary>
    /// <inheritdoc/>
    public int Insert(List<T> t)
    {
        return Context.CopyNew().Insertable(t).ExecuteCommand();
    }
    /// <summary>
    /// 向数据库中插入实体。
    /// </summary>
    /// <inheritdoc/>
    public int Insert(T parm, Expression<Func<T, object>>? iClumns = null, bool ignoreNull = true)
    {
        return Context.CopyNew().Insertable(parm).InsertColumns(iClumns).IgnoreColumns(ignoreNullColumn: ignoreNull).ExecuteCommand();
    }
    /// <summary>
    /// 向数据库中插入实体。
    /// </summary>
    /// <inheritdoc/>
    public IInsertable<T> Insertable(T t)
    {
        return Context.CopyNew().Insertable<T>(t);
    }

    #endregion add

    #region update
    /// <summary>
    /// 向数据库中更新实体。
    /// </summary>
    /// <inheritdoc/>
    public IUpdateable<T> Updateable(T entity)
    {
        return Context.CopyNew().Updateable(entity);
    }
    /// <summary>
    /// 向数据库中更新实体。
    /// </summary>
    /// <inheritdoc/>
    public int Update(T entity, bool ignoreNullColumns = false)
    {
        return Context.CopyNew().Updateable(entity).IgnoreColumns(ignoreNullColumns).ExecuteCommand();
    }
    /// <summary>
    /// 向数据库中更新实体。
    /// </summary>
    /// <inheritdoc/>
    public int Update(T entity, Expression<Func<T, object>> expression, bool ignoreAllNull = false)
    {
        return Context.CopyNew().Updateable(entity).UpdateColumns(expression).IgnoreColumns(ignoreAllNull).ExecuteCommand();
    }
    /// <summary>
    /// 向数据库中更新实体。
    /// </summary>
    /// <inheritdoc/>
    public int Update(T entity, Expression<Func<T, object>> expression, Expression<Func<T, bool>> where)
    {
        return Context.CopyNew().Updateable(entity).UpdateColumns(expression).Where(where).ExecuteCommand();
    }
    /// <summary>
    /// 向数据库中更新实体。
    /// </summary>
    /// <inheritdoc/>
    public int Update(SqlSugarClient client, T entity, Expression<Func<T, object>> expression, Expression<Func<T, bool>> where)
    {
        return client.CopyNew().Updateable(entity).UpdateColumns(expression).Where(where).ExecuteCommand();
    }
    /// <summary>
    /// 向数据库中更新实体。
    /// </summary>
    /// <inheritdoc/>
    public int Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns)
    {
        return Context.CopyNew().Updateable<T>().SetColumns(columns).Where(where).RemoveDataCache().ExecuteCommand();
    }

    #endregion update

    /// <inheritdoc/>

    public IStorageable<T> Storageable(T t)
    {
        return Context.CopyNew().Storageable<T>(t);
    }

    /// <inheritdoc/>
    public IStorageable<T> Storageable(List<T> t)
    {
        return Context.CopyNew().Storageable(t);
    }

    #region delete

    /// <inheritdoc/>
    public IDeleteable<T> Deleteable()
    {
        return Context.CopyNew().Deleteable<T>();
    }
    /// <summary>
    /// 向数据库中删除实体。
    /// </summary>
    /// <inheritdoc/>
    public int Delete(object[] obj)
    {
        return Context.CopyNew().Deleteable<T>().In(obj).ExecuteCommand();
    }
    /// <summary>
    /// 向数据库中删除实体。
    /// </summary>
    /// <inheritdoc/>
    public int Delete(object id)
    {
        return Context.CopyNew().Deleteable<T>(id).ExecuteCommand();
    }
    /// <summary>
    /// 向数据库中删除实体。
    /// </summary>
    /// <inheritdoc/>
    public int DeleteTable()
    {
        return Context.CopyNew().Deleteable<T>().ExecuteCommand();
    }

    /// <inheritdoc/>
    public bool Truncate()
    {
        return Context.CopyNew().DbMaintenance.TruncateTable<T>();
    }

    #endregion delete

    #region query

    /// <inheritdoc/>
    public bool Any(Expression<Func<T, bool>> expression)
    {
        return Context.CopyNew().Queryable<T>().Where(expression).Any();
    }

    /// <inheritdoc/>
    public ISugarQueryable<T> Queryable()
    {
        return Context.CopyNew().Queryable<T>();
    }

    /// <inheritdoc/>
    public T GetId(object pkValue)
    {
        return Context.CopyNew().Queryable<T>().InSingle(pkValue);
    }

    #endregion query

    /// <inheritdoc/>
    public DataTable UseStoredProcedureToDataTable(string procedureName, List<SugarParameter> parameters)
    {
        return Context.CopyNew().Ado.UseStoredProcedure().GetDataTable(procedureName, parameters);
    }

    /// <inheritdoc/>
    public (DataTable, List<SugarParameter>) UseStoredProcedureToTuple(string procedureName, List<SugarParameter> parameters)
    {
        var result = (Context.CopyNew().Ado.UseStoredProcedure().GetDataTable(procedureName, parameters), parameters);
        return result;
    }

}