using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Impl;
using NHibernate.Linq;
using Orchard.ContentManagement.Records;
using Orchard.Data;
using Orchard.Utility.Extensions;

namespace Orchard.ContentManagement {
    public class DefaultContentQuery : IContentQuery {
        private readonly ISessionLocator _sessionLocator;
        private ISession _session;
        private ICriteria _itemVersionCriteria;
        private VersionOptions _versionOptions;

        public DefaultContentQuery(IContentManager contentManager, ISessionLocator sessionLocator) {
            _sessionLocator = sessionLocator;
            ContentManager = contentManager;
        }

        public IContentManager ContentManager { get; private set; }

        ISession BindSession() {
            if (_session == null)
                _session = _sessionLocator.For(typeof(ContentItemVersionRecord));
            return _session;
        }

        ICriteria BindCriteriaByPath(ICriteria criteria, string path) {
            return criteria.GetCriteriaByPath(path) ?? criteria.CreateCriteria(path);
        }

        ICriteria BindTypeCriteria() {
            // ([ContentItemVersionRecord] >join> [ContentItemRecord]) >join> [ContentType]

            return BindCriteriaByPath(BindItemCriteria(), "ContentType");
        }

        ICriteria BindItemCriteria() {
            // [ContentItemVersionRecord] >join> [ContentItemRecord]

            return BindCriteriaByPath(BindItemVersionCriteria(), "ContentItemRecord");
        }

        ICriteria BindItemVersionCriteria() {
            if (_itemVersionCriteria == null) {
                _itemVersionCriteria = BindSession().CreateCriteria<ContentItemVersionRecord>();
            }
            return _itemVersionCriteria;
        }

        ICriteria BindPartCriteria<TRecord>() where TRecord : ContentPartRecord {
            if (typeof(TRecord).IsSubclassOf(typeof(ContentPartVersionRecord))) {
                return BindCriteriaByPath(BindItemVersionCriteria(), typeof(TRecord).Name);
            }
            return BindCriteriaByPath(BindItemCriteria(), typeof(TRecord).Name);
        }


        private void ForType(params string[] contentTypeNames) {
            if (contentTypeNames != null && contentTypeNames.Length != 0)
                BindTypeCriteria().Add(Restrictions.InG("Name", contentTypeNames));
        }

        public void ForVersion(VersionOptions options) {
            _versionOptions = options;
        }

        private void Where<TRecord>() where TRecord : ContentPartRecord {
            // this simply demands an inner join
            BindPartCriteria<TRecord>();
        }

        private void Where<TRecord>(Expression<Func<TRecord, bool>> predicate) where TRecord : ContentPartRecord {

            // build a linq to nhibernate expression
            var options = new QueryOptions();
            var queryProvider = new NHibernateQueryProvider(BindSession(), options);
            var queryable = new Query<TRecord>(queryProvider, options).Where(predicate);

            // translate it into the nhibernate ICriteria implementation
            var criteria = (CriteriaImpl)queryProvider.TranslateExpression(queryable.Expression);

            // attach the criterion from the predicate to this query's criteria for the record
            var recordCriteria = BindPartCriteria<TRecord>();
            foreach (var expressionEntry in criteria.IterateExpressionEntries()) {
                recordCriteria.Add(expressionEntry.Criterion);
            }
        }

        private void OrderBy<TRecord, TKey>(Expression<Func<TRecord, TKey>> keySelector) where TRecord : ContentPartRecord {
            // build a linq to nhibernate expression
            var options = new QueryOptions();
            var queryProvider = new NHibernateQueryProvider(BindSession(), options);
            var queryable = new Query<TRecord>(queryProvider, options).OrderBy(keySelector);

            // translate it into the nhibernate ordering
            var criteria = (CriteriaImpl)queryProvider.TranslateExpression(queryable.Expression);

            // attaching orderings to the query's criteria
            var recordCriteria = BindPartCriteria<TRecord>();
            foreach (var ordering in criteria.IterateOrderings()) {
                recordCriteria.AddOrder(ordering.Order);
            }
        }

        private void OrderByDescending<TRecord, TKey>(Expression<Func<TRecord, TKey>> keySelector) where TRecord : ContentPartRecord {
            // build a linq to nhibernate expression
            var options = new QueryOptions();
            var queryProvider = new NHibernateQueryProvider(BindSession(), options);
            var queryable = new Query<TRecord>(queryProvider, options).OrderByDescending(keySelector);

            // translate it into the nhibernate ICriteria implementation
            var criteria = (CriteriaImpl)queryProvider.TranslateExpression(queryable.Expression);

            // attaching orderings to the query's criteria
            var recordCriteria = BindPartCriteria<TRecord>();
            foreach (var ordering in criteria.IterateOrderings()) {
                recordCriteria.AddOrder(ordering.Order);
            }
        }


        private IEnumerable<ContentItem> Slice(int skip, int count) {
            var criteria = BindItemVersionCriteria();
            if (_versionOptions == null) {
                criteria.Add(Restrictions.Eq("Published", true));
            }
            else if (_versionOptions.IsPublished) {
                criteria.Add(Restrictions.Eq("Published", true));
            }
            else if (_versionOptions.IsLatest) {
                criteria.Add(Restrictions.Eq("Latest", true));
            }
            else if (_versionOptions.IsDraft) {
                criteria.Add(Restrictions.And(
                                 Restrictions.Eq("Latest", true),
                                 Restrictions.Eq("Published", false)));
            }
            else if (_versionOptions.IsAllVersions) {
                // no-op... all versions will be returned by default
            }
            else {
                throw new ApplicationException("Invalid VersionOptions for content query");
            }

            // TODO: put 'removed false' filter in place

            if (skip != 0) {
                criteria = criteria.SetFirstResult(skip);
            }
            if (count != 0) {
                criteria = criteria.SetMaxResults(count);
            }
            return criteria
                .List<ContentItemVersionRecord>()
                .Select(x => ContentManager.Get(x.Id, VersionOptions.VersionRecord(x.Id)))
                .ToReadOnlyCollection();
        }

        IContentQuery<TPart> IContentQuery.ForPart<TPart>() {
            return new ContentQuery<TPart>(this);
        }

        class ContentQuery<T> : IContentQuery<T> where T : IContent {
            protected readonly DefaultContentQuery _query;

            public ContentQuery(DefaultContentQuery query) {
                _query = query;
            }

            public IContentManager ContentManager {
                get { return _query.ContentManager; }
            }

            IContentQuery<TPart> IContentQuery.ForPart<TPart>() {
                return new ContentQuery<TPart>(_query);
            }

            IContentQuery<T> IContentQuery<T>.ForType(params string[] contentTypes) {
                _query.ForType(contentTypes);
                return this;
            }

            IContentQuery<T> IContentQuery<T>.ForVersion(VersionOptions options) {
                _query.ForVersion(options);
                return this;
            }

            IEnumerable<T> IContentQuery<T>.List() {
                return _query.Slice(0, 0).AsPart<T>();
            }

            IEnumerable<T> IContentQuery<T>.Slice(int skip, int count) {
                return _query.Slice(skip, count).AsPart<T>();
            }

            IContentQuery<T, TRecord> IContentQuery<T>.Join<TRecord>() {
                _query.Where<TRecord>();
                return new ContentQuery<T, TRecord>(_query);
            }

            IContentQuery<T, TRecord> IContentQuery<T>.Where<TRecord>(Expression<Func<TRecord, bool>> predicate) {
                _query.Where(predicate);
                return new ContentQuery<T, TRecord>(_query);
            }

            IContentQuery<T, TRecord> IContentQuery<T>.OrderBy<TRecord, TKey>(Expression<Func<TRecord, TKey>> keySelector) {
                _query.OrderBy(keySelector);
                return new ContentQuery<T, TRecord>(_query);
            }

            IContentQuery<T, TRecord> IContentQuery<T>.OrderByDescending<TRecord, TKey>(Expression<Func<TRecord, TKey>> keySelector) {
                _query.OrderByDescending(keySelector);
                return new ContentQuery<T, TRecord>(_query);
            }
        }


        class ContentQuery<T, TR> : ContentQuery<T>, IContentQuery<T, TR>
            where T : IContent
            where TR : ContentPartRecord {
            public ContentQuery(DefaultContentQuery query)
                : base(query) {
            }

            IContentQuery<T, TR> IContentQuery<T, TR>.ForVersion(VersionOptions options) {
                _query.ForVersion(options);
                return this;
            }

            IContentQuery<T, TR> IContentQuery<T, TR>.Where(Expression<Func<TR, bool>> predicate) {
                _query.Where(predicate);
                return this;
            }

            IContentQuery<T, TR> IContentQuery<T, TR>.OrderBy<TKey>(Expression<Func<TR, TKey>> keySelector) {
                _query.OrderBy(keySelector);
                return this;
            }

            IContentQuery<T, TR> IContentQuery<T, TR>.OrderByDescending<TKey>(Expression<Func<TR, TKey>> keySelector) {
                _query.OrderByDescending(keySelector);
                return this;
            }
        }

    }
}