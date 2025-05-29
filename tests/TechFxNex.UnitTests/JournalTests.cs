using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using TechFxNet.Application.Queries;
using TechFxNet.Domain.Models;
using TechFxNet.Infrastructure.Repositories;
using TechFxNet.Domain.Dtos;
using TechFxNet.Domain.Entities;

namespace TechFxNex.UnitTests;

[TestFixture]
public class JournalTests
{
    [TestOf(nameof(GetJournalRangeQueryHandler.Handle))]
    public class GetJournalRangeTests
    {
        [Test, CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion) => assertion.Verify(typeof(GetJournalRangeQueryHandler).GetConstructors());

        [Test, CustomAutoData]
        public async Task Get_journal_range_query_should_return_non_empty_result_set(GetJournalRangeQueryHandler sut,
            [Frozen] IJournalRepository repository,
            GetJournalRangeQuery request,
            PaginatedResult<JournalEntity> response)
        {
            // ARRANGE
            Mock.Get(repository)
                .Setup(p => p.GetPagedResultAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<JournalFilterDto>(),It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // ACT
            var result = await sut.Handle(request, default);

            // ASSERTS
            result.Items.Should().NotBeNull();
            result.Items.Should().Contain(dto => dto.Id == response.Items.First().Id);
        }

        [Test, CustomAutoData]
        public async Task Get_journal_range_query_should_handle_command_exception(GetJournalRangeQueryHandler sut,
            [Frozen] IJournalRepository repository,
            GetJournalRangeQuery request)
        {
            // ARRANGE
            Mock.Get(repository)
                .Setup(p => p.GetPagedResultAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<JournalFilterDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // ACT
            var result = () => sut.Handle(request, default);

            // ASSERTS
            await result.Should().ThrowExactlyAsync<Exception>();
        }
    }

    [TestOf(nameof(GetJournalSingleQueryHandler.Handle))]
    public class GetJournalSingleTests
    {
        [Test, CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion) => assertion.Verify(typeof(GetJournalRangeQueryHandler).GetConstructors());

        [Test, CustomAutoData]
        public async Task Get_journal_single_query_should_return_non_empty_result_set(GetJournalSingleQueryHandler sut,
            [Frozen] IJournalRepository repository,
            GetJournalSingleQuery request,
            JournalEntity response)
        {
            // ARRANGE
            Mock.Get(repository)
                .Setup(p => p.GetSingleByEventId(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // ACT
            var result = await sut.Handle(request, default);

            // ASSERTS
            result.Should().NotBeNull();
            result.Text.Should().Be(response.Text);
        }

        [Test, CustomAutoData]
        public async Task Get_journal_single_query_should_handle_command_exception(GetJournalSingleQueryHandler sut,
            [Frozen] IJournalRepository repository,
            GetJournalSingleQuery request)
        {
            // ARRANGE
            Mock.Get(repository)
                .Setup(p => p.GetSingleByEventId(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // ACT
            var result = () => sut.Handle(request, default);

            // ASSERTS
            await result.Should().ThrowExactlyAsync<Exception>();
        }
    }
}