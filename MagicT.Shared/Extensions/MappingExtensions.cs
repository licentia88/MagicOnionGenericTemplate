using MagicT.Shared.Models.Base;
using MagicT.Shared.Models.MemoryDatabaseModels;
using Mapster;

namespace MagicT.Shared.Extensions;

public static class MappingExtensions
{

    public static TDest MapToMemoryModel<TDest>(this AUTHORIZATIONS_BASE source) where TDest:Authorizations
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<AUTHORIZATIONS_BASE, TDest>()
           .Map(dest => dest.Id, src => src.AB_ROWID)
            .Map(dest => dest.UserRefNo, src => src.AB_USER_REFNO)
            .Map(dest => dest.AuthType, src => src.AB_AUTH_TYPE)
            .Ignore(dest => dest.Description);

        return source.Adapt<TDest>(config);
    }

    public static List<TDest> MapToMemoryModelList<TDest>(this IEnumerable<AUTHORIZATIONS_BASE> sourceList) where TDest : Authorizations, new()
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<AUTHORIZATIONS_BASE, TDest>()
            .Map(dest => dest.Id, src => src.AB_ROWID)
            .Map(dest => dest.UserRefNo, src => src.AB_USER_REFNO)
            .Map(dest => dest.AuthType, src => src.AB_AUTH_TYPE)
            .Ignore(dest => dest.Description);

        return sourceList.Adapt<List<TDest>>(config);
    }


}
