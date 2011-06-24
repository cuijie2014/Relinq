﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;

namespace Remotion.Linq.UnitTests.Linq.Core
{
  public class StubQueryExecutor : IQueryExecutor
  {
    public T ExecuteScalar<T> (QueryModel queryModel)
    {
      throw new NotImplementedException ("ExecuteScalar<" + typeof (T).Name + "> (" + queryModel + ")");
    }

    public T ExecuteSingle<T> (QueryModel queryModel, bool returnDefaultWhenEmpty)
    {
      throw new NotImplementedException ("ExecuteSingle<" + typeof (T).Name + "> (" + queryModel + ", " + returnDefaultWhenEmpty + ")");
    }

    public IEnumerable<T> ExecuteCollection<T> (QueryModel queryModel)
    {
      throw new NotImplementedException ("ExecuteCollection<" + typeof (T).Name + "> (" + queryModel + ")");
    }
  }
}