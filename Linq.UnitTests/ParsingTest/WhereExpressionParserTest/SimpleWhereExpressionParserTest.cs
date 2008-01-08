using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.Linq.Parsing;

namespace Rubicon.Data.Linq.UnitTests.ParsingTest.WhereExpressionParserTest
{
  [TestFixture]
  public class SimpleWhereExpressionParserTest
  {
    private IQueryable<Student> _querySource;
    private MethodCallExpression _expression;
    private ExpressionTreeNavigator _navigator;
    private WhereExpressionParser _parser;
    private BodyHelper _bodyWhereHelper;

    [SetUp]
    public void SetUp ()
    {
      _querySource = ExpressionHelper.CreateQuerySource ();
      _expression = TestQueryGenerator.CreateSimpleWhereQuery_WhereExpression (_querySource);
      _navigator  = new ExpressionTreeNavigator(_expression);
      _parser = new WhereExpressionParser (_expression, _expression, true);
      _bodyWhereHelper = new BodyHelper (_parser.FromLetWhereExpressions);
    }

    [Test]
    public void ParsesFromExpressions()
    {
      Assert.IsNotNull (_bodyWhereHelper.FromExpressions);
      Assert.That (_bodyWhereHelper.FromExpressions, Is.EqualTo (new object[] { _expression.Arguments[0] }));
      Assert.IsInstanceOfType (typeof (ConstantExpression), _bodyWhereHelper.FromExpressions[0]);
      Assert.AreSame (_querySource, ((ConstantExpression) _bodyWhereHelper.FromExpressions[0]).Value);
    }

    [Test]
    public void ParsesFromIdentifiers ()
    {
      Assert.IsNotNull (_bodyWhereHelper.FromIdentifiers);
      Assert.That (_bodyWhereHelper.FromIdentifiers,
                   Is.EqualTo (new object[] { _navigator.Arguments[1].Operand.Parameters[0].Expression }));
      Assert.IsInstanceOfType (typeof (ParameterExpression), _bodyWhereHelper.FromIdentifiers[0]);
      Assert.AreEqual ("s", ((ParameterExpression) _bodyWhereHelper.FromIdentifiers[0]).Name);
    }

    [Test]
    public void ParsesBoolExpressions ()
    {
      Assert.IsNotNull (_bodyWhereHelper.WhereExpressions);
      Assert.That (_bodyWhereHelper.WhereExpressions, Is.EqualTo (new object[] { _navigator.Arguments[1].Operand.Expression }));
      Assert.IsInstanceOfType (typeof (LambdaExpression), _bodyWhereHelper.WhereExpressions[0]);
    }

    [Test]
    public void ParsesProjectionExpressions ()
    {
      Assert.IsNotNull (_parser.ProjectionExpressions);
      Assert.AreEqual (1, _parser.ProjectionExpressions.Count);
      Assert.IsNull (_parser.ProjectionExpressions[0]);
    }

    [Test]
    public void ParsesProjectionExpressions_NotTopLevel ()
    {
      WhereExpressionParser parser = new WhereExpressionParser (_expression, _expression, false);
      Assert.IsNotNull (parser.ProjectionExpressions);
      Assert.AreEqual (0, parser.ProjectionExpressions.Count);
    }


  }
}