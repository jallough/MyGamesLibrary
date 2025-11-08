using AutoMapper;
using Server.Games.Entities;
using Server.Models;
using Server.Users.Entities;

namespace Server.Shared
{
    public static class Mappings
    {
        public static void InitMapper(this IServiceCollection services)
        {
            services.AddSingleton(InitMapper());
        }
        public static IMapper InitMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserEntity,UserDto>()
                .ReverseMap()
                .ForMember(dto => dto.RefreshTokenExpiryTime, opt => opt.Ignore())
                .ForMember(dto => dto.RefreshToken, opt => opt.Ignore());
                cfg.CreateMap<GamesEntity, GamesEntity>()
                .ForMember(g=>g.CreatedAt, opt=>opt.Ignore());
                cfg.CreateMap<UserEntity, UserEntity>()
                .ForMember(u=>u.CreatedAt, opt => opt.Ignore());
                cfg.CreateMap<GamesUserRelationEntity, GamesUserRelationEntity>()
                .ForMember(gur => gur.Game, opt => opt.Ignore())
                .ForMember(gur=>gur.CreatedAt, opt=>opt.Ignore());

            });
            config.CompileMappings();
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }
    }
}
