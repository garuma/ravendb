using Raven.Bundles.Authorization.Model;
using Raven.Bundles.Tests.Versioning;
using Raven.Client.Authorization;
using Raven.Client.Extensions;
using Xunit;

namespace Raven.Bundles.Tests.Authorization.Bugs
{
    public class WhenUsingMultiTenancy : AuthorizationTest
    {
        [Fact]
        public void BugWhenSavingDocumentOnDatabase()
        {
            string database = "test_auth";
            store.DatabaseCommands.EnsureDatabaseExists(database);

            var company = new Company
            {
                Name = "Hibernating Rhinos"
            };
            using (var s = store.OpenSession(database))
            {
                s.Store(new AuthorizationUser
                {
                    Id = UserId,
                    Name = "Ayende Rahien",
                });

                s.Store(company);

                s.SetAuthorizationFor(company, new DocumentAuthorization
                {
                    Permissions =
                        {
                            new DocumentPermission
                            {
                                User = UserId,
                                Allow = true,
                                Operation = "Company/Bid"
                            }
                        }
                });

                s.SaveChanges();
            }

            using (var s = store.OpenSession(database))
            {
                s.SecureFor(UserId, "Company/Bid");

                Assert.NotNull(s.Load<Company>(company.Id));
            }
        }
    }
}