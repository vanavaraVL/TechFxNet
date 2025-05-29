using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using TechFxNet.Application.Queries;
using TechFxNet.Domain.Entities;
using TechFxNet.Domain.Models;
using TechFxNet.Infrastructure.Repositories;

namespace TechFxNex.UnitTests;

[TestFixture]
public class TreeNodeTests
{
    [TestOf(nameof(GetTreeQueryHandler.Handle))]
    public class GetTreeQueryTests
    {
        [Test, CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion) => assertion.Verify(typeof(GetTreeQueryHandler).GetConstructors());

        [Test, CustomAutoData]
        public async Task Get_tree_should_build_tree_view(GetTreeQueryHandler sut,
            [Frozen] ITreeNodeRepository repository,
            GetTreeQuery request,
            PaginatedResult<JournalEntity> response)
        {
            // ARRANGE
            var nodes = new List<NodeEntity>()
            {
                new NodeEntity()
                {
                    Id = 1,
                    ParentNodeId = null,
                    NodeName = "Root",
                    ChildNodes = new List<NodeEntity>()
                    {
                        new NodeEntity()
                        {
                            Id = 2,
                            ParentNodeId = 1,
                            NodeName = "Child"
                        },
                        new NodeEntity()
                        {
                            Id = 3,
                            ParentNodeId = 1,
                            NodeName = "Child-2"
                        },
                    }
                }
            };

            var treeEntity = new TreeEntity()
            {
                Id = 1,
                TreeName = "Tree",
                Nodes = nodes
            };


            Mock.Get(repository)
                .Setup(p => p.GetTreeByName(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(treeEntity);

            Mock.Get(repository)
                .Setup(p => p.GetTreeNodes(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(nodes);

            // ACT
            var result = await sut.Handle(request, default);

            // ASSERTS
            result.Name.Should().Be(treeEntity.TreeName);
            result.Children.Count.Should().Be(1);

            result.Children.First()!.Children.Count.Should().Be(2);
        }
    }
}