# About

This is a simple extension for IQueryable<T> that allow the use of strongly typed specification objects. This can be useful
when reusing and combining predicates in ORMs such as EnitityFramework.

# Example

Generally, you should inherit from `Rh.Specifiable.Specification.ExpressionSpecification<T>`. E.g.

``` csharp
public class BlogPostIsPublished : ExpressionSpecification<BlogPost>
{
    protected override Expression<Func<BlogPost, bool>> Expression { get; }

    public IsPublic()
    {
        Expression = b => b.VisibilityId == PostingVisibilityId.Public;
    }
}
```
Then you can use it directly in a `Where` call:

``` csharp
myDbContext.BlogPosts.Where(new BlogPostIsPublished());
```

There are also the `AndSpecification` and `OrSpecification` base classes which allow for strongly typed combinations of smaller
specifications.

# `AsSpecifiable`

In order to use a specification inside an expression tree, e.g. in a `Select` call, you first need to call `AsSpecifiable()` on the
source `IQueryable<T>`. That returns a `VisitableQuery` which visits the expression and replaces the `Specification`s with their
corresponding `Expression`. You only need to do this once per queryable but it doesn't matter if you call it multiple times.

``` csharp
// AsSpecifiable enables specifications in the Select Expression tree.
myDbContext.BlogPosts.AsSpecifiable().Select(post => new BlogPostModel 
    { 
        Id = post.Id, 
        Content = post.Content, 
        Comments = post.Comments.AsQueryable().Where(new CommentIsPublic()) 
    }); 
```
