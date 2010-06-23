using System;
using NHibernate;

namespace Orchard.Data.Builders {

	
    public class SessionFactoryBuilder : ISessionFactoryBuilder {
        public ISessionFactory BuildSessionFactory(SessionFactoryParameters parameters) {
            AbstractBuilder builder;
            if (isProviderNameMatching(parameters, "SQLite")) {
                builder = new SQLiteBuilder(parameters.DataFolder, parameters.ConnectionString);
            }
						else if (isProviderNameMatching(parameters, "SQLServer")) {
                builder = new SqlServerBuilder(parameters.DataFolder, parameters.ConnectionString);
            }
						else {
							builder = loadBuilderByReflectionOrFail(parameters.Provider);
						}
            return builder.BuildSessionFactory(parameters);
        }

				private AbstractBuilder loadBuilderByReflectionOrFail(string providerTypeName) {
					AbstractBuilder builder = null;
					try {
						var buildertype = Type.GetType(providerTypeName);
						if (buildertype == null) {
							var exceptionMessage = string.Format("type '{0}' cannot be loaded, make sure the qualified name is valid", providerTypeName);
							throw new ArgumentOutOfRangeException("providerTypeName", exceptionMessage);
						}
						var parameterlessconstructor = buildertype.GetConstructor(new Type[] {});
						builder = (AbstractBuilder) parameterlessconstructor.Invoke(new object[] {});
						return builder;
					}
					catch (Exception e) {
						throw new OrchardException(string.Format("wasn't able to load builder '{0}'", providerTypeName), e);
					}
				}

    	private bool isProviderNameMatching(SessionFactoryParameters parameters, string providerNameToMatch) {
    		return string.Equals(parameters.Provider, providerNameToMatch, StringComparison.InvariantCultureIgnoreCase);
    	}
    }
}
