using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNet.Mvc.ApplicationModels;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Sandbox.Web
{
    public interface IApplicationModelConventionBuilder : IControllerModelConventionBuilder
    {
        IApplicationModelConventionBuilder Apply(IApplicationModelConvention convention);

        IApplicationModelConventionBuilder Apply(Action<ApplicationModel> action);

        IControllerModelConventionBuilder ForControllers(Func<ControllerModel, bool> predicate);
    }

    public interface IControllerModelConventionBuilder : IActionModelConventionBuilder
    {
        IControllerModelConventionBuilder Apply(IControllerModelConvention convention);

        IControllerModelConventionBuilder Apply(Action<ControllerModel> action);

        IActionModelConventionBuilder ForActions(Func<ActionModel, bool> predicate);
    }

    public interface IActionModelConventionBuilder : IParameterModelConventionBuilder
    {
        IActionModelConventionBuilder Apply(IActionModelConvention convention);

        IActionModelConventionBuilder Apply(Action<ActionModel> action);

        IParameterModelConventionBuilder ForParameters(Func<ParameterModel, bool> predicate);
    }

    public interface IParameterModelConventionBuilder
    {
        IParameterModelConventionBuilder Apply(IParameterModelConvention convention);

        IParameterModelConventionBuilder Apply(Action<ParameterModel> action);
    }

    public static class ConventionBuilderExtensions
    {
        public static IApplicationModelConventionBuilder Build(this IList<IApplicationModelConvention> conventions)
        {
            return new ApplicationModelConventionBuilder(conventions);
        }

        #region Controller Filtering

        public static IControllerModelConventionBuilder ForAllControllers(this IApplicationModelConventionBuilder builder)
        {
            return builder.ForControllers(predicate: null);
        }

        public static IControllerModelConventionBuilder ForControllerType<T>(this IApplicationModelConventionBuilder builder)
        {
            return builder.ForControllerType(typeof(T));
        }

        public static IControllerModelConventionBuilder ForControllerType(this IApplicationModelConventionBuilder builder, Type type)
        {
            return builder.ForControllers(c => c.ControllerType == type.GetTypeInfo());
        }

        public static IControllerModelConventionBuilder ForControllersDerivedFromType<T>(this IApplicationModelConventionBuilder builder)
        {
            return builder.ForControllersDerivedFromType(typeof(T));
        }

        public static IControllerModelConventionBuilder ForControllersDerivedFromType(this IApplicationModelConventionBuilder builder, Type type)
        {
            return builder.ForControllers(c => type.GetTypeInfo().IsAssignableFrom(c.ControllerType));
        }

        public static IControllerModelConventionBuilder ForControllersNamed(this IApplicationModelConventionBuilder builder, string name)
        {
            return builder.ForControllers(c => c.ControllerName == name);
        }

        public static IControllerModelConventionBuilder ForControllersInNamespace(this IApplicationModelConventionBuilder builder, string @namespace)
        {
            return builder.ForControllers(c => c.ControllerType.Namespace == @namespace);
        }

        #endregion

        #region Action Filtering

        public static IActionModelConventionBuilder ForAllActions(this IControllerModelConventionBuilder builder)
        {
            return builder.ForActions(predicate: null);
        }

        public static IActionModelConventionBuilder ForActionsNamed(this IControllerModelConventionBuilder builder, string name)
        {
            return builder.ForActions(a => a.ActionName == name);
        }

        public static IActionModelConventionBuilder ForAction<TController>(this IControllerModelConventionBuilder builder, Expression<Action<TController>> expression)
        {
            if (expression.Body.NodeType != ExpressionType.Call)
            {
                throw null;
            }

            var call = (MethodCallExpression)expression.Body;
            var method = call.Method;

            return builder.ForActions(a => a.ActionMethod == method);
        }

        #endregion

        #region Routing

        public static IActionModelConventionBuilder UseRoute(this IActionModelConventionBuilder builder, string route)
        {
            return builder.Apply(a => a.AttributeRouteModel.Template = route);
        }

        public static IActionModelConventionBuilder AddRouteParameters(this IActionModelConventionBuilder builder, params string[] whitelist)
        {
            return builder.Apply((action) =>
            {
                var route = action.AttributeRouteModel.Template;
                if (route == null)
                {
                    return;
                }

                route = route + "/" + string.Join("/", action.Parameters.Where(p => p.BindingInfo.BindingSource == BindingSource.Path || whitelist?.Contains(p.ParameterName) == true));
                action.AttributeRouteModel.Template = route;
            });
        }

        public static IActionModelConventionBuilder InferHttpVerbFromPrefix(this IActionModelConventionBuilder builder)
        {
            var verbs = new HashSet<string>()
            {
                "GET",
                "PUT",
                "POST",
                "HEAD",
                "DELETE",
            };

            return builder.Apply((action) =>
            {
                if (action.HttpMethods.Any())
                {
                    return;
                }

                foreach (var verb in verbs)
                {
                    if (action.ActionName.StartsWith(verb, StringComparison.OrdinalIgnoreCase))
                    {
                        action.ActionName = action.ActionName.Substring(verb.Length);
                        action.HttpMethods.Add(verb);
                        return;
                    }
                }
            });
        }

        #endregion

        #region Binding

        public static IParameterModelConventionBuilder SetBindingSource(this IParameterModelConventionBuilder builder, BindingSource source)
        {
            return builder.Apply(p => p.BindingInfo.BindingSource = source);
        }

        #endregion

        private class ApplicationModelConventionBuilder : IApplicationModelConventionBuilder
        {
            private readonly IList<IApplicationModelConvention> _conventions;

            public ApplicationModelConventionBuilder(IList<IApplicationModelConvention> conventions)
            {
                _conventions = conventions;
            }

            public IApplicationModelConventionBuilder Apply(Action<ApplicationModel> action)
            {
                return Apply(new ApplicationModelConvention(action));
            }

            public IApplicationModelConventionBuilder Apply(IApplicationModelConvention convention)
            {
                _conventions.Add(convention);
                return this;
            }

            public IControllerModelConventionBuilder Apply(IControllerModelConvention convention)
            {
                _conventions.Add(new ApplicationModelConvention(controllerPredicate: null, controllerConvention: convention));
                return this;
            }

            public IControllerModelConventionBuilder Apply(Action<ControllerModel> action)
            {
                return Apply(new ControllerModelConvention(action));
            }

            public IActionModelConventionBuilder Apply(IActionModelConvention convention)
            {
                _conventions.Add(new ApplicationModelConvention(controllerPredicate: null, controllerConvention: new ControllerModelConvention(actionPredicate: null, actionConvention: convention)));
                return this;
            }

            public IActionModelConventionBuilder Apply(Action<ActionModel> action)
            {
                return Apply(new ActionModelConvention(action));
            }

            public IParameterModelConventionBuilder Apply(IParameterModelConvention convention)
            {
                _conventions.Add(new ApplicationModelConvention(controllerPredicate: null, controllerConvention: new ControllerModelConvention(actionPredicate: null, actionConvention: new ActionModelConvention(parameterPredicate: null, parameterConvention: convention))));
                return this;
            }

            public IParameterModelConventionBuilder Apply(Action<ParameterModel> action)
            {
                return Apply(new ParameterModelConvention(action));
            }

            public IControllerModelConventionBuilder ForControllers(Func<ControllerModel, bool> predicate)
            {
                return new ControllerModelConventionBuilder(_conventions, predicate);
            }

            public IActionModelConventionBuilder ForActions(Func<ActionModel, bool> predicate)
            {
                return new ActionModelConventionBuilder(_conventions, controllerPredicate: null, actionPredicate: predicate);
            }

            public IParameterModelConventionBuilder ForParameters(Func<ParameterModel, bool> predicate)
            {
                return new ParameterModelConventionBuilder(_conventions, controllerPredicate: null, actionPredicate: null, parameterPredicate: predicate);
            }
        }

        private class ControllerModelConventionBuilder : IControllerModelConventionBuilder
        {
            private readonly IList<IApplicationModelConvention> _conventions;
            private readonly Func<ControllerModel, bool> _controllerPredicate;

            public ControllerModelConventionBuilder(IList<IApplicationModelConvention> conventions, Func<ControllerModel, bool> controllerPredicate)
            {
                _conventions = conventions;
                _controllerPredicate = controllerPredicate;
            }

            public IControllerModelConventionBuilder Apply(IControllerModelConvention convention)
            {
                _conventions.Add(new ApplicationModelConvention(_controllerPredicate, convention));
                return this;
            }

            public IControllerModelConventionBuilder Apply(Action<ControllerModel> action)
            {
                return Apply(new ControllerModelConvention(action));
            }


            public IActionModelConventionBuilder Apply(IActionModelConvention convention)
            {
                _conventions.Add(new ApplicationModelConvention(controllerPredicate: _controllerPredicate, controllerConvention: new ControllerModelConvention(actionPredicate: null, actionConvention: convention)));
                return this;
            }

            public IActionModelConventionBuilder Apply(Action<ActionModel> action)
            {
                return Apply(new ActionModelConvention(action));
            }

            public IParameterModelConventionBuilder Apply(IParameterModelConvention convention)
            {
                _conventions.Add(new ApplicationModelConvention(controllerPredicate: _controllerPredicate, controllerConvention: new ControllerModelConvention(actionPredicate: null, actionConvention: new ActionModelConvention(parameterPredicate: null, parameterConvention: convention))));
                return this;
            }

            public IParameterModelConventionBuilder Apply(Action<ParameterModel> action)
            {
                return Apply(new ParameterModelConvention(action));
            }

            public IActionModelConventionBuilder ForActions(Func<ActionModel, bool> predicate)
            {
                return new ActionModelConventionBuilder(_conventions, _controllerPredicate, predicate);
            }

            public IParameterModelConventionBuilder ForParameters(Func<ParameterModel, bool> predicate)
            {
                return new ParameterModelConventionBuilder(_conventions, controllerPredicate: _controllerPredicate, actionPredicate: null, parameterPredicate: predicate);
            }
        }

        private class ActionModelConventionBuilder : IActionModelConventionBuilder
        {
            private readonly IList<IApplicationModelConvention> _conventions;
            private readonly Func<ControllerModel, bool> _controllerPredicate;
            private readonly Func<ActionModel, bool> _actionPredicate;

            public ActionModelConventionBuilder(IList<IApplicationModelConvention> conventions, Func<ControllerModel, bool> controllerPredicate, Func<ActionModel, bool> actionPredicate)
            {
                _conventions = conventions;
                _controllerPredicate = controllerPredicate;
                _actionPredicate = actionPredicate;
            }

            public IActionModelConventionBuilder Apply(IActionModelConvention convention)
            {
                _conventions.Add(new ApplicationModelConvention(_controllerPredicate, new ControllerModelConvention(_actionPredicate, convention)));
                return this;
            }

            public IActionModelConventionBuilder Apply(Action<ActionModel> action)
            {
                return Apply(new ActionModelConvention(action));
            }

            public IParameterModelConventionBuilder Apply(IParameterModelConvention convention)
            {
                _conventions.Add(new ApplicationModelConvention(controllerPredicate: _controllerPredicate, controllerConvention: new ControllerModelConvention(actionPredicate: _actionPredicate, actionConvention: new ActionModelConvention(parameterPredicate: null, parameterConvention: convention))));
                return this;
            }

            public IParameterModelConventionBuilder Apply(Action<ParameterModel> action)
            {
                return Apply(new ParameterModelConvention(action));
            }

            public IParameterModelConventionBuilder ForParameters(Func<ParameterModel, bool> predicate)
            {
                return new ParameterModelConventionBuilder(_conventions, _controllerPredicate, _actionPredicate, predicate);
            }
        }

        private class ParameterModelConventionBuilder : IParameterModelConventionBuilder
        {
            private readonly IList<IApplicationModelConvention> _conventions;
            private readonly Func<ControllerModel, bool> _controllerPredicate;
            private readonly Func<ActionModel, bool> _actionPredicate;
            private readonly Func<ParameterModel, bool> _parameterPredicate;

            public ParameterModelConventionBuilder(IList<IApplicationModelConvention> conventions, Func<ControllerModel, bool> controllerPredicate, Func<ActionModel, bool> actionPredicate, Func<ParameterModel, bool> parameterPredicate)
            {
                _conventions = conventions;
                _controllerPredicate = controllerPredicate;
                _actionPredicate = actionPredicate;
                _parameterPredicate = parameterPredicate;
            }

            public IParameterModelConventionBuilder Apply(IParameterModelConvention convention)
            {
                _conventions.Add(new ApplicationModelConvention(_controllerPredicate, new ControllerModelConvention(_actionPredicate, new ActionModelConvention(_parameterPredicate, convention))));
                return this;
            }

            public IParameterModelConventionBuilder Apply(Action<ParameterModel> action)
            {
                return Apply(new ParameterModelConvention(action));
            }
        }

        private class ApplicationModelConvention : IApplicationModelConvention
        {
            private Action<ApplicationModel> _applicationAction;

            private Func<ControllerModel, bool> _controllerPredicate;
            private IControllerModelConvention _controllerConvention;

            public ApplicationModelConvention(Action<ApplicationModel> applicationAction)
            {
                _applicationAction = applicationAction;
            }

            public ApplicationModelConvention(Func<ControllerModel, bool> controllerPredicate, IControllerModelConvention controllerConvention)
            {
                _controllerPredicate = controllerPredicate;
                _controllerConvention = controllerConvention;
            }

            public void Apply(ApplicationModel application)
            {
                if (_applicationAction != null)
                {
                    _applicationAction(application);
                }

                if (_controllerConvention != null)
                {
                    foreach (var controller in application.Controllers)
                    {
                        if (_controllerPredicate == null || _controllerPredicate(controller))
                        {
                            _controllerConvention.Apply(controller);
                        }
                    }
                }
            }
        }

        private class ControllerModelConvention : IControllerModelConvention
        {
            private Action<ControllerModel> _controllerAction;

            private Func<ActionModel, bool> _actionPredicate;
            private IActionModelConvention _actionConvention;

            public ControllerModelConvention(Action<ControllerModel> controllerAction)
            {
                _controllerAction = controllerAction;
            }

            public ControllerModelConvention(Func<ActionModel, bool> actionPredicate, IActionModelConvention actionConvention)
            {
                _actionPredicate = actionPredicate;
                _actionConvention = actionConvention;
            }

            public void Apply(ControllerModel controller)
            {
                if (_controllerAction != null)
                {
                    _controllerAction(controller);
                }

                if (_actionConvention != null)
                {
                    foreach (var action in controller.Actions)
                    {
                        if (_actionPredicate == null || _actionPredicate(action))
                        {
                            _actionConvention.Apply(action);
                        }
                    }
                }
            }
        }

        private class ActionModelConvention : IActionModelConvention
        {
            private Action<ActionModel> _actionAction;

            private Func<ParameterModel, bool> _parameterPredicate;
            private IParameterModelConvention _parameterConvention;

            public ActionModelConvention(Action<ActionModel> actionAction)
            {
                _actionAction = actionAction;
            }

            public ActionModelConvention(Func<ParameterModel, bool> parameterPredicate, IParameterModelConvention parameterConvention)
            {
                _parameterPredicate = parameterPredicate;
                _parameterConvention = parameterConvention;
            }

            public void Apply(ActionModel action)
            {
                if (_actionAction != null)
                {
                    _actionAction(action);
                }

                if (_parameterConvention != null)
                {
                    foreach (var parameter in action.Parameters)
                    {
                        if (_parameterPredicate == null || _parameterPredicate(parameter))
                        {
                            _parameterConvention.Apply(parameter);
                        }
                    }
                }
            }
        }

        private class ParameterModelConvention : IParameterModelConvention
        {
            private Action<ParameterModel> _parameterAction;

            private Func<ParameterModel, bool> _parameterPredicate;
            private IParameterModelConvention _parameterConvention;

            public ParameterModelConvention(Action<ParameterModel> parameterAction)
            {
                _parameterAction = parameterAction;
            }

            public void Apply(ParameterModel parameter)
            {
                if (_parameterAction != null)
                {
                    _parameterAction(parameter);
                }
            }
        }
    }
}