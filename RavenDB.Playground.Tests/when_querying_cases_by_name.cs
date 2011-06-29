using Raven.Client.Embedded;
using RavenDB.Playground.Model;
using Xunit;
using System.Linq;
using Shouldly;

namespace RavenDB.Playground.Tests
{
    public class when_querying_cases_by_name
    {
        private readonly EmbeddableDocumentStore store;

        public when_querying_cases_by_name()
        {
            store = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };
            store.Initialize();

            using (var session = store.OpenSession())
            {
                session.Store(new Case { Name = "bcda" });
                session.Store(new Case { Name = "dacb" });
                session.Store(new Case { Name = "daab" });
                session.Store(new Case { Name = "dacb" });
                session.Store(new Case { Name = "aacb" });
                session.Store(new Case { Name = "aaac" });
                session.Store(new Case { Name = "bcbb" });
                session.Store(new Case { Name = "acba" });
                session.Store(new Case { Name = "aaaa" });
                session.Store(new Case { Name = "dada" });
                session.SaveChanges();
            }
        }

        [Fact]
        public void can_query_names_starting_with_da()
        {
            using (var session = store.OpenSession())
            {
                var cases = session.Query<Case>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.Name.StartsWith("da")).ToList();
                
                cases.Count.ShouldBe(4);
                cases.Select(x => x.Name).ShouldBe(new[] { "dacb", "daab", "dacb", "dada" });
            }
        }

        [Fact]
        public void can_query_names_starting_with_dad()
        {
            using (var session = store.OpenSession())
            {
                var cases = session.Query<Case>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.Name.StartsWith("dad")).ToList();

                cases.Count.ShouldBe(1);
                cases.Select(x => x.Name).ShouldBe(new[] { "dada" });
            }
        }

        [Fact]
        public void can_query_names_starting_with_aa()
        {
            using (var session = store.OpenSession())
            {
                var cases = session.Query<Case>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.Name.StartsWith("aa")).ToList();

                cases.Count.ShouldBe(3);
                cases.Select(x => x.Name).ShouldBe(new[] { "aacb", "aaac", "aaaa" });
            }
        }

        [Fact]
        public void can_query_names_starting_with_bc()
        {
            using (var session = store.OpenSession())
            {
                var cases = session.Query<Case>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.Name.StartsWith("bc")).ToList();

                cases.Count.ShouldBe(2);
                cases.Select(x => x.Name).ShouldBe(new[] { "bcda", "bcbb" });
            }
        }
    }
}
