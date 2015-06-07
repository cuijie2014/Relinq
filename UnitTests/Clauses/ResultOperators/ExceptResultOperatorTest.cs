// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Development.UnitTesting;
using Remotion.Linq.Development.UnitTesting.Clauses.Expressions;

namespace Remotion.Linq.UnitTests.Clauses.ResultOperators
{
  [TestFixture]
  public class ExceptResultOperatorTest
  {
    private ExceptResultOperator _resultOperator;
    private Expression _source2;

    [SetUp]
    public void SetUp ()
    {
      _source2 = Expression.Constant (new[] { 2 });
      _resultOperator = new ExceptResultOperator (_source2);
    }

    [Test]
    public void GetConstantSource2_ConstantExpression ()
    {
      Assert.That (_resultOperator.GetConstantSource2<int> (), Is.SameAs (((ConstantExpression) _source2).Value));
    }

    [Test]
    public void GetConstantSource2_ReducibleToConstantExpression ()
    {
      var resultOperator = new ExceptResultOperator (new ReducibleExtensionExpression (new ReducibleExtensionExpression (_source2)));
      Assert.That (resultOperator.GetConstantSource2<int> (), Is.SameAs (((ConstantExpression) _source2).Value));
    }

    [Test]
    public void GetConstantSource2_NoConstantExpression ()
    {
      var resultOperator = new ExceptResultOperator (Expression.Parameter (typeof (IEnumerable<string>), "ss"));
      Assert.That (
          () => resultOperator.GetConstantSource2<string>(),
          Throws.ArgumentException
              .With.Message.EqualTo (
#if !NET_3_5
                  "The source2 expression ('ss') is no ConstantExpression, it is a TypedParameterExpression.\r\nParameter name: expression"
#else
                  "The source2 expression ('ss') is no ConstantExpression, it is a ParameterExpression.\r\nParameter name: expression"
#endif
                  ));
    }

    [Test]
    public void GetConstantSource2_ChecksForInfiniteRecursion ()
    {
      var resultOperator = new ExceptResultOperator (new RecursiveReducibleExtensionExpression (_source2.Type));
      Assert.That (
          () => resultOperator.GetConstantSource2<int>(),
#if !NET_3_5
          Throws.ArgumentException.With.Message.EqualTo ("node cannot reduce to itself or null")
#else
          Throws.InvalidOperationException.With.Message.EqualTo ("Reduce cannot return the original expression.")
#endif
          );
    }

    [Test]
    public void Clone ()
    {
      var clonedClauseMapping = new QuerySourceMapping ();
      var cloneContext = new CloneContext (clonedClauseMapping);
      var clone = _resultOperator.Clone (cloneContext);

      Assert.That (clone, Is.InstanceOf (typeof (ExceptResultOperator)));
      Assert.That (((ExceptResultOperator) clone).Source2, Is.SameAs (_source2));
    }

    [Test]
    public void ExecuteInMemory ()
    {
      var items = new[] { 1, 2, 3 };
      var input = new StreamedSequence (items, new StreamedSequenceInfo (typeof (int[]), Expression.Constant (0)));
      var result = _resultOperator.ExecuteInMemory<int> (input);

      Assert.That (result.GetTypedSequence<int>().ToArray(), Is.EquivalentTo (new[] { 1, 3 }));
    }

    [Test]
    public void TransformExpressions ()
    {
      var oldExpression = ExpressionHelper.CreateExpression ();
      var newExpression = ExpressionHelper.CreateExpression ();
      var resultOperator = new ExceptResultOperator (oldExpression);

      resultOperator.TransformExpressions (ex =>
      {
        Assert.That (ex, Is.SameAs (oldExpression));
        return newExpression;
      });

      Assert.That (resultOperator.Source2, Is.SameAs (newExpression));
    }
  }
}
