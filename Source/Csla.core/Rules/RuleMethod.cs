﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Establishes a relationship between a business/validation
  /// rule and a specific property of a business class.
  /// </summary>
  public class RuleMethod
  {
    /// <summary>
    /// Gets the rule object.
    /// </summary>
    public IBusinessRule Rule { get; private set; }
    /// <summary>
    /// Gets the primary property to which this rule is attached.
    /// </summary>
    public Csla.Core.IPropertyInfo PrimaryProperty { get; private set; }
    /// <summary>
    /// Gets the rule priority.
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="rule">Instance of rule.</param>
    /// <param name="property">Property to which the rule is attached.</param>
    /// <param name="priority"></param>
    public RuleMethod(IBusinessRule rule, Core.IPropertyInfo property)
    {
      Rule = rule;
      PrimaryProperty = property;
      Priority = 0;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="rule">Instance of rule.</param>
    /// <param name="property">Property to which the rule is attached.</param>
    /// <param name="priority">Rule priority.</param>
    public RuleMethod(IBusinessRule rule, Core.IPropertyInfo property, int priority)
      : this(rule, property)
    {
      Priority = priority;
    }

    /// <summary>
    /// Invokes the business rule, waiting for the rule
    /// to complete before proceeding.
    /// </summary>
    /// <param name="target">Reference to the business
    /// object against which this rule will run.</param>
    public void Invoke(object target)
    {
      var lck = new System.Threading.ManualResetEventSlim(false);
      var context = new RuleContext((r) =>
        {
          // process results
          lck.Set();
        })
      {
        PrimaryProperty = this.PrimaryProperty,
        RuleDefinition = this,
      };
      Rule.Rule(context);
      lck.Wait();
    }

    /// <summary>
    /// Invokes the business rule, not waiting for
    /// the rule to complete before proceeding (if the
    /// rule is async).
    /// </summary>
    /// <param name="target">Reference to the business
    /// object against which this rule will run.</param>
    /// <param name="callback">Callback to be invoked when
    /// the rule completes.</param>
    public void BeginInvoke(object target, Action<List<RuleResult>> callback)
    {
      var context = new RuleContext((r) =>
      {
        // process results
        callback(r.Results);
      })
      {
        PrimaryProperty = this.PrimaryProperty,
        RuleDefinition = this,
      };
      Rule.Rule(context);
    }
  }
}