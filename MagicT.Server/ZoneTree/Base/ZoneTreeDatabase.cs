using MagicT.Server.ZoneTree.Serializers;
using Tenray.ZoneTree;

namespace MagicT.Server.ZoneTree.Base;

public abstract class ZoneTreeDatabase<TKey,TModel> where TModel:class
{
    public IZoneTree<TKey,TModel> Database { get; set; }

    public IZoneTreeIterator<TKey,TModel> Iterator  { get; set; }

    public ZoneTreeDatabase(string path)
    {
        Database = new ZoneTreeFactory<TKey, TModel>()
            .SetValueSerializer(new ZoneTreeSerializer<TModel>())
            .SetDataDirectory(path)
            .OpenOrCreate();


        Iterator = Database.CreateIterator();
    }

 
    public virtual void Insert(TKey key, TModel model)
    {
        Database.Upsert(key, model);
    }

    public virtual void AddOrUpdate(TKey key,TModel model)
    {
        Database.TryAtomicAddOrUpdate(key, model,(ref TModel value) =>
        {
            value = model;
            return true;
        });
    }
    public virtual void Update(TKey key, TModel model)
    {
        Database.Upsert(key,model);
    }
    
    public virtual void Delete(TKey key)
    {
        Database.TryDelete(key);
    }
    
    public virtual TModel Find(TKey key)
    {
        Database.TryGet(key, out var model);

        return model;
    }

    public virtual IEnumerable<KeyValuePair<TKey,TModel>> FindAll()
    {
        while (Iterator.Next())
        {
            yield return Iterator.Current;
        }
    }

    public virtual IEnumerable<KeyValuePair<TKey, TModel>> FindBy(Func<TModel,bool> predicate)
    {
        while (Iterator.Next())
        {
            if (predicate(Iterator.Current.Value))
                yield return Iterator.Current;

        }
    }

    public virtual void DeleteBy(Func<TModel, bool> predicate)
    {
        foreach(var item in FindBy(predicate))
            Delete(item.Key);

    }

    public virtual void DeleteAll()
    {
        foreach (var kValueTuple in FindAll())
        {
            Delete(kValueTuple.Key);
        }
    }

}
