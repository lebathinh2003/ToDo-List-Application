using AutoMapper;
using Google.Protobuf.Collections;
using UserService.Application.Configs.MapperConverters;


namespace UserService.Application.Configs;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            // Grpc mapping
            config.CreateMap(typeof(List<>), typeof(RepeatedField<>)).ConvertUsing(typeof(ListToRepeatedFieldConverter<,>));
            config.CreateMap(typeof(RepeatedField<>), typeof(List<>)).ConvertUsing(typeof(RepeatedFieldToListConverter<,>));
        });

        return mappingConfig;
    }
}
