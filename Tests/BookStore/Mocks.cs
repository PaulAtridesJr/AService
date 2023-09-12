using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using AService.Items;
using AService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Moq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AService.Tests.BookStore
{
	internal static class Mocks
	{
		internal static BookStoreContext CreateBookStoreContextMock(List<Book> initialItems) 
		{
			Mock<BookStoreContext> result = new Mock<BookStoreContext>(new DbContextOptions<BookStoreContext>());

			if (initialItems != null)
			{
				IQueryable<Book> data = initialItems.AsQueryable();

				DbSet<Book> mockDbSet = GetQueryableMockDbSet(data);

				result.Setup(p => p.Books).Returns(mockDbSet);
			}
			
			return result.Object;
		}

		private static DbSet<T> GetQueryableMockDbSet<T>(IQueryable<T> data) where T : class
		{			
			var mockSet = new Mock<DbSet<T>>();
			mockSet.As<IDbAsyncEnumerable<T>>()
				.Setup(m => m.GetAsyncEnumerator())
				.Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));
			mockSet.As<IAsyncEnumerable<T>>()
				.Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
				.Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
			mockSet.As<IQueryable<T>>()
				.Setup(m => m.Provider)
				.Returns(new TestDbAsyncQueryProvider<T>(data.Provider));

			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

			return mockSet.Object;
		}

		internal static IOptions<ServiceOptions> GetServiceOptionsMock() 
		{
			Mock<IOptions<ServiceOptions>> result = new();
			return result.Object;
		}

		internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
		{
			private readonly IQueryProvider _inner;

			internal TestDbAsyncQueryProvider(IQueryProvider inner)
			{
				_inner = inner;
			}

			public IQueryable CreateQuery(Expression expression)
			{
				return new TestDbAsyncEnumerable<TEntity>(expression);
			}

			public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
			{
				return new TestDbAsyncEnumerable<TElement>(expression);
			}

			public object Execute(Expression expression)
			{
				return _inner.Execute(expression);
			}

			public TResult Execute<TResult>(Expression expression)
			{
				return _inner.Execute<TResult>(expression);
			}

			public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
			{
				return Task.FromResult(Execute(expression));
			}

			public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
			{
				return Task.FromResult(Execute<TResult>(expression));
			}
		}

		internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
		{
			public TestAsyncEnumerable(IEnumerable<T> enumerable)
				: base(enumerable)
			{ }

			public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			{
				return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
			}
		}

		internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
		{
			private readonly IEnumerator<T> _inner;

			public TestAsyncEnumerator(IEnumerator<T> inner)
			{
				_inner = inner;
			}

			public T Current => _inner.Current;

			public ValueTask DisposeAsync()
			{
				_inner.Dispose();

				return ValueTask.CompletedTask;
			}

			public ValueTask<bool> MoveNextAsync()
			{
				return ValueTask.FromResult(_inner.MoveNext());
			}
		}

		internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
		{
			public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
				: base(enumerable)
			{ }

			public TestDbAsyncEnumerable(Expression expression)
				: base(expression)
			{ }

			public IDbAsyncEnumerator<T> GetAsyncEnumerator()
			{
				return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
			}

			IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
			{
				return GetAsyncEnumerator();
			}

			IQueryProvider IQueryable.Provider
			{
				get { return new TestDbAsyncQueryProvider<T>(this); }
			}
		}

		internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
		{
			private readonly IEnumerator<T> _inner;

			public TestDbAsyncEnumerator(IEnumerator<T> inner)
			{
				_inner = inner;
			}

			public void Dispose()
			{
				_inner.Dispose();
			}

			public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
			{
				return Task.FromResult(_inner.MoveNext());
			}

			public T Current
			{
				get { return _inner.Current; }
			}

			object IDbAsyncEnumerator.Current
			{
				get { return Current; }
			}
		}
	}
}
