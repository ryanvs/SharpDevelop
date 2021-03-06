﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class IsMvcControllerFolderSelectedConditionEvaluator : IsMvcFolderSelectedConditionEvaluator
	{
		protected override string MvcParentFolderName {
			get { return "Controllers"; }
		}
	}
}
