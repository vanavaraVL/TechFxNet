using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using TechFxNet.Infrastructure.Repositories;
using TechFxNet.Application.Mappers;

namespace TechFxNex.UnitTests;

[AttributeUsage(AttributeTargets.Method)]
public class CustomAutoDataAttribute : AutoDataAttribute
{
    public CustomAutoDataAttribute() : base(FixtureHelpers.CreateFixture)
    {
    }
}

internal static class FixtureHelpers
{
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true, GenerateDelegates = true });

        fixture.Customizations.Add(new TypeRelay(typeof(IReadOnlySet<>), typeof(HashSet<>)));

        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        fixture.Freeze<IJournalRepository>();
        fixture.Freeze<ITreeNodeRepository>();

        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new JournalMapperProfile());
            mc.AddProfile(new TreeNodeMapperProfile());
        });

        fixture.Register<IMapper>(() => new Mapper(mappingConfig));

        return fixture;
    }
}