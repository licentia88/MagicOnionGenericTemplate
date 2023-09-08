namespace MagicT.Server.ZoneTree.Base;

public abstract class ZoneTreeIntDatabase<TModel> : ZoneTreeDatabase<long, TModel> where TModel : class
{
    public long Index;

    protected ZoneTreeIntDatabase(string path) : base(path)
    {
        Index = Database.Count();
    }

    public virtual void Add(TModel model)
    {
        var index = Interlocked.Increment(ref Index);
        Database.TryAtomicAdd(index, model);

    }

    public override void Update(long key, TModel model)
    {
        Database.TryAtomicUpdate(key, model);
    }

}